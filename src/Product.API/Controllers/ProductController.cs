using Core.Api;
using Microsoft.AspNetCore.Mvc;
using Product.API.AutoMapper;
using Product.API.Dtos;
using Product.API.Repository;
using System.Threading.Tasks;

namespace Product.API.Controllers
{
    public class ProductController : BaseApi
    {
        private readonly IProductRepository _repository;

        public ProductController(IProductRepository repository)
        {
            _repository = repository;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _repository.FindAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            return Ok(await _repository.FindByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddProductDto product)
        {
            var entity = product.ToEntity();

            _repository.Add(entity);

            await _repository.UnitOfWork.SaveChangesAsync();

            return Ok(entity.ToDto());
        }
    }
}
