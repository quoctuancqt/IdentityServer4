using AutoMapper;
using IdentityServer.Dtos;
using IdentityServer.Models;

namespace IdentityServer.AutoMapper
{
    public class AccountMapperProfile : Profile
    {
        public AccountMapperProfile()
        {
            #region Dto to Entity
            CreateMap<AddUserDto, ApplicationUser>();
            #endregion
        }
    }
    public static class AccountMapper
    {
        static AccountMapper()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<AccountMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        public static ApplicationUser ToEntity(this AddUserDto dto)
        {
            return Mapper.Map<ApplicationUser>(dto);
        }
    }
}
