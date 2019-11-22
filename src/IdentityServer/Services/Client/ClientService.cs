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

namespace IdentityServer.Services
{
    //https://www.scottbrady91.com/Identity-Server/Getting-Started-with-IdentityServer-4

    public class ClientService : ClientStore, IClientService
    {
        public ClientService(IConfigurationDbContext context, ILogger<ClientStore> logger) : base(context, logger)
        {
        }

        public Task CreateApiResourceAsync()
        {
            throw new NotImplementedException();
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

    }
}
