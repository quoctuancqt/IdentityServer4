﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Net.Http;
using DistributedCache;
using DistributedCache.Models;
using Application.Exceptions;
using Infrastructure;

namespace Application.ContextFactory
{
    public class TenantFactory : ITenantFactory
    {
        private ConcurrentDictionary<string, DbContextOptionsBuilder<DbContext>> dbContextOptionsBuilders;

        private readonly IDistributedCacheService _distributedCache;

        private readonly IConfiguration _configuration;

        public TenantFactory(IDistributedCacheService distributedCache,
            IConfiguration configuration)
        {
            _distributedCache = distributedCache;

            _configuration = configuration;

            dbContextOptionsBuilders = new ConcurrentDictionary<string, DbContextOptionsBuilder<DbContext>>();
        }

        public async Task<TenantProfileModel> GetTenantByTenantIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ForbiddenException();

            try
            {
                var tenant = await _distributedCache.GetAsync<TenantProfileModel>(id);

                if (tenant == null) throw new BadRequestException($"Not found tenant {id}");

                return tenant;

            }
            catch
            {
                return await GetTenantByHtppAsync(id);
            }

        }

        public T GetTenantContext<T>(string id) where T : DbContext
        {
            if (!dbContextOptionsBuilders.TryGetValue(id, out DbContextOptionsBuilder<DbContext> options))
            {
                var tenant = GetTenantByTenantIdAsync(id).GetAwaiter().GetResult();

                if (tenant == null) throw new BadRequestException($"Not found tenantId {id}");

                options = new DbContextOptionsBuilder<DbContext>();

                options.UseSqlServer(tenant.GetSqlConnectionString(_configuration.GetConnectionString("DefaultConnectionString")));

                dbContextOptionsBuilders.TryAdd(id, options);
            }

            return (T)Activator.CreateInstance(typeof(T), options.Options);

        }

        public async Task CreateAsync(string tenantId)
        {
            using var context = GetTenantContext<ApplicationContext>(tenantId);

            await context.Database.MigrateAsync();
        }

        public async Task UpdateAsync(string tenantId)
        {
            await _distributedCache.RefreshAsync(tenantId);

            await CreateAsync(tenantId);
        }

        private async Task<TenantProfileModel> GetTenantByHtppAsync(string tenantId)
        {
            using var client = new HttpClient
            {
                BaseAddress = new Uri(_configuration.GetValue<string>("Idsr4:IssuerUri"))
            };

            var response = await client.GetAsync($"api/clients/refresh-cache/{tenantId}");

            return await response.Content.ReadAsAsync<TenantProfileModel>();
        }
    }
}
