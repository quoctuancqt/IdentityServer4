using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.ContextFactory;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Product.API.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly IContextFactory<ProductContext> _contextFactory;
        public IUnitOfWork UnitOfWork => _contextFactory.Context;

        public ProductRepository(IContextFactory<ProductContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public Entities.Product Add(Entities.Product product)
        {
            return _contextFactory.Context.Products.Add(product).Entity;
        }

        public async Task<IEnumerable<Entities.Product>> FindAllAsync()
        {
            return await _contextFactory.Context.Products.AsNoTracking().ToListAsync();
        }

        public async Task<Entities.Product> FindByIdAsync(string id)
        {
            return await _contextFactory.Context.Products.
                SingleOrDefaultAsync(x => x.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        public Entities.Product Update(Entities.Product buyer)
        {
            return _contextFactory.Context.Products
                    .Update(buyer)
                    .Entity;
        }
    }
}
