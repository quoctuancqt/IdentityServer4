using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text;
using System;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer.Dtos;
using IdentityServer.Dtos.Base;
using IdentityServer.Exceptions;
using IdentityServer.Data;
using Microsoft.Extensions.Options;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using DistributedCache.Models;
using DistributedCache;

namespace IdentityServer.Services
{
    //https://www.scottbrady91.com/Identity-Server/Getting-Started-with-IdentityServer-4

    public class ClientService : ClientStore, IClientService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<ClientConfiguration> _clientConfiguration;
        private readonly TenantConfig _tenantConfig;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDistributedCacheService _distributedCache;

        public ClientService(IConfigurationDbContext context, ILogger<ClientStore> logger,
            ApplicationDbContext dbContext, IOptions<TenantConfig> tenantConfig,
            IDistributedCacheService distributedCache, UserManager<ApplicationUser> userManager) : base(context, logger)
        {
            _dbContext = dbContext;
            _clientConfiguration = _dbContext.ClientConfigurations;
            _tenantConfig = tenantConfig.Value;
            _userManager = userManager;
            _distributedCache = distributedCache;
        }

        public async Task CreateApiResourceAsync(string[] values)
        {
            if (!await Context.ApiResources.AnyAsync(x => values.Contains(x.Name)))
            {
                var entities = values.Select(x => new ApiResource(x, x).ToEntity());

                Context.ApiResources.AddRange(entities);

                await Context.SaveChangesAsync();
            }
        }

        public async Task<string[]> GetApiResourceAsync()
        {
            return await Context.ApiResources.Select(x => x.Name).ToArrayAsync();
        }

        public async Task<object> CreateAsync(AddClientDto dto)
        {
            await CheckClientNameExistingAsync(dto.ClientName);

            var entity = dto.GenerateClient().ToEntity();

            entity.ClientUri = _tenantConfig.ParseClientUriBySubDomain(dto.ClientName);

            await CreateApiResourceAsync(dto.AllowedScopes.ToArray());

            Context.Clients.Add(entity);

            await Context.SaveChangesAsync();

            AddClientConfiguration(entity);

            var user = new ApplicationUser { UserName = $"{dto.ClientName}@yopmail.com" };

            await _userManager.CreateAsync(user, "Admin@1234");

            await _userManager.AddToRoleAsync(user, "Administrator");

            _dbContext.UserClients.Add(new UserClient(user.Id, entity.ClientId));

            var strategy = _dbContext.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                using var transaction = _dbContext.Database.BeginTransaction();

                await _dbContext.SaveChangesAsync();

                await SetCacheTenantProfileAsync(entity, user);

                //TO DO: Create DB in each microservices

                transaction.Commit();
            });

            return new { entity.ClientId, Secret = entity.Description };
        }

        public async Task<object> UpdateAsync(string clientId, EditClientDto dto)
        {
            var entity = await Context.Clients.SingleOrDefaultAsync(x => x.ClientId.Equals(clientId));

            if (entity == null) throw new BadRequestException($"Not found {clientId}");

            var model = entity.ToModel();

            model.AllowedScopes = dto.AllowedScopes;

            model.RedirectUris = dto.RedirectUris;

            model.PostLogoutRedirectUris = dto.PostLogoutRedirectUris;

            Context.Clients.Update(model.ToEntity());

            await Context.SaveChangesAsync();

            AddClientConfiguration(entity);

            var user = await _userManager.FindByNameAsync($"{entity.ClientName}@yopmail.com");

            var strategy = _dbContext.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                using var transaction = _dbContext.Database.BeginTransaction();

                await _dbContext.SaveChangesAsync();

                await SetCacheTenantProfileAsync(entity, user);

                //TO DO: Create DB in each microservices

                transaction.Commit();
            });

            return new { entity.ClientId, Secret = entity.Description };
        }

        public Task CreateIdentityResourceAsync()
        {
            throw new NotImplementedException();
        }

        public async Task CreateResourceOwnerPasswordAsync(AddClientResourceOwnerPasswordDto dto)
        {
            await CheckClientNameExistingAsync(dto.ClientName);

            var entity = dto.GenerateClient().ToEntity();

            Context.Clients.Add(entity);

            await Context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string clientId)
        {
            var entity = await Context.Clients.SingleOrDefaultAsync(x => x.ClientId.Equals(clientId));

            if (entity == null) throw new BadRequestException($"Not found {clientId}");

            await _dbContext.ExecuteCommandAsync("DELETE ClientScopes Where ClientId=@clientId", new { clientId = entity.Id });

            await _dbContext.ExecuteCommandAsync("DELETE ClientRedirectUris Where ClientId=@clientId", new { clientId = entity.Id });

            await _dbContext.ExecuteCommandAsync("DELETE ClientPostLogoutRedirectUris Where ClientId=@clientId", new { clientId = entity.Id });

            await _dbContext.ExecuteCommandAsync("DELETE ClientSecrets Where ClientId=@clientId", new { clientId = entity.Id });

            await _dbContext.ExecuteCommandAsync("DELETE ClientGrantTypes Where ClientId=@clientId", new { clientId = entity.Id });

            await _dbContext.ExecuteCommandAsync("DELETE Clients Where Id=@clientId", new { clientId = entity.Id });
        }

        public override Task<Client> FindClientByIdAsync(string clientId)
        {
            return base.FindClientByIdAsync(clientId);
        }

        public async Task<Client> FindEnabledClientByIdAsync(string clientId)
        {
            var client = await FindClientByIdAsync(clientId);

            if (client != null && client.Enabled == true) return client;

            return null;
        }

        public async Task<string> GenerateHeaderCredentialAsync(string clientId)
        {
            var client = await Context.Clients.SingleOrDefaultAsync(x => x.ClientId.Equals(clientId));

            if (client == null) new BadRequestException($"Not found {clientId}");

            var credentials = string.Format("{0}:{1}", client.ClientId, client.Description);

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));
        }

        public async Task<bool> IsPkceClientAsync(string clientId)
        {
            if (!string.IsNullOrWhiteSpace(clientId))
            {
                var client = await FindEnabledClientByIdAsync(clientId);

                return client?.RequirePkce == true;
            }

            return false;
        }

        public async Task<PageResultDto<Client>> SearchAsync(QuerySearchDefault @param)
        {
            var query = Context.Clients.AsQueryable();

            if (!string.IsNullOrEmpty(@param.SearchKey))
            {
                query = query.Where(x => EF.Functions.Like(x.ClientName, $"%{@param.SearchKey}%"));
            }

            int totalRecord = await query.CountAsync();

            var result = await query
                .OrderByDescending(x => x.Created)
                .Skip(@param.GetSkip())
                .Take(@param.GetTake())
                .ToListAsync();

            return new PageResultDto<Client>(totalRecord,
                @param.GetTake(),
                result.Select(x => x.ToModel()));
        }

        private async Task CheckClientNameExistingAsync(string clientName)
        {
            if (await Context.Clients.AnyAsync(x => x.ClientName.Equals(clientName)))
                new BadRequestException("Name is already existing.");
        }

        private async Task<TenantProfileModel> SetCacheTenantProfileAsync(IdentityServer4.EntityFramework.Entities.Client entity, ApplicationUser user)
        {
            await _distributedCache.RemoveAsync(entity.ClientId);

            var tenant = new TenantProfileModel
            {
                Id = entity.ClientId,
                ClientName = entity.ClientName,
                ClientUri = entity.ClientUri,
                SqlServer = _tenantConfig.SqlServer,
                SqlDatabase = _tenantConfig.GetTenantDbName(entity.ClientName),
                SqlUserName = _tenantConfig.SqlUserName,
                SqlPassword = _tenantConfig.SqlPassword,
                MongoDbServer = _tenantConfig.MongoDbServer,
                MongoDbDatabase = _tenantConfig.GetTenantDbName(entity.ClientName),
                MongoDbUserName = _tenantConfig.MongoDbUserName,
                MongoDbPassword = _tenantConfig.MongoDbPassword
            };

            await _distributedCache.SetAsync(entity.ClientId, tenant);

            return tenant;
        }

        private void AddClientConfiguration(IdentityServer4.EntityFramework.Entities.Client entity)
        {
            #region SQl

            _clientConfiguration.Add(new ClientConfiguration(entity.ClientId, "SqlServer", _tenantConfig.SqlServer));

            _clientConfiguration.Add(new ClientConfiguration(entity.ClientId, "SqlDatabase", _tenantConfig.GetTenantDbName(entity.ClientName)));

            _clientConfiguration.Add(new ClientConfiguration(entity.ClientId, "SqlUserName", _tenantConfig.SqlUserName));

            _clientConfiguration.Add(new ClientConfiguration(entity.ClientId, "SqlPassword", _tenantConfig.SqlPassword));

            #endregion SQl

            #region MongoDB

            _clientConfiguration.Add(new ClientConfiguration(entity.ClientId, "MongoDbServer", _tenantConfig.MongoDbServer));

            _clientConfiguration.Add(new ClientConfiguration(entity.ClientId, "MongoDbDatabase", _tenantConfig.GetTenantDbName(entity.ClientName)));

            _clientConfiguration.Add(new ClientConfiguration(entity.ClientId, "MongoDbUserName", _tenantConfig.MongoDbUserName));

            _clientConfiguration.Add(new ClientConfiguration(entity.Id.ToString(), "MongoDbPassword", _tenantConfig.MongoDbPassword));

            #endregion MongoDB
        }
    }
}
