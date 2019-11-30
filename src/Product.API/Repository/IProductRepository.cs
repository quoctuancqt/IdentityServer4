using Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Product.API.Repository
{
    public interface IProductRepository : IRepository<Entities.Product>
    {
        Entities.Product Add(Entities.Product product);
        Entities.Product Update(Entities.Product buyer);
        Task<IEnumerable<Entities.Product>> FindAllAsync();
        Task<Entities.Product> FindByIdAsync(string id);
    }
}
