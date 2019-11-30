using AutoMapper;
using Product.API.Dtos;

namespace Product.API.AutoMapper
{
    public class ProductMapperProfile : Profile
    {
        public ProductMapperProfile()
        {
            CreateMap<AddProductDto, Entities.Product>();

            CreateMap<Entities.Product, ProductDto>();
        }
    }

    public static class ProductMapper
    {
        internal static IMapper Mapper { get; }

        static ProductMapper()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ProductMapperProfile>())
                .CreateMapper();
        }

        public static Entities.Product ToEntity(this AddProductDto dto)
        {
            return Mapper.Map<Entities.Product>(dto);
        }

        public static Entities.Product ToEntity(this AddProductDto dto, Entities.Product entity)
        {
            return Mapper.Map(dto, entity);
        }

        public static ProductDto ToDto(this Entities.Product entity)
        {
            return Mapper.Map<ProductDto>(entity);
        }
    }
}
