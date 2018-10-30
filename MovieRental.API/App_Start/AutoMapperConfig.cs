using AutoMapper;
using MovieRental.API.Models;
using MovieRental.Entities.Models;

namespace MovieRental.API
{
	public class AutoMapperConfig
	{
		public static void Configure()
		{
			Mapper.Initialize(cfg =>
			{
				cfg.CreateMap<AccountModel, Account>()
					.ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.UserRole) ? "User" : src.UserRole));
				cfg.CreateMap<MovieModel, Movie>();
			    cfg.CreateMap<KioskModel, Address>()
					.ForMember(dest => dest.ID, opt => opt.Ignore());
				cfg.CreateMap<KioskModel, Kiosk>()
					.ForMember(dest => dest.ID, opt => opt.Ignore())
					.ForMember(dest => dest.Address, opt => opt.Ignore())
					.AfterMap((src, dest) => {
						if (dest.Address == null || dest.Address.ID == 0)
						{
							dest.Address = Mapper.Map<Address>(src);
						}
						else
						{
							Mapper.Map(src, dest.Address);
						}
					});
                cfg.CreateMap<Kiosk, KioskModel>()
			        .ForMember(dest => dest.StreetAddress1, opt => opt.MapFrom(src => src.Address != null ? src.Address.StreetAddress1 : null))
                    .ForMember(dest => dest.StreetAddress2, opt => opt.MapFrom(src => src.Address != null ? src.Address.StreetAddress2 : null))
                    .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address != null ? src.Address.City : null))
                    .ForMember(dest => dest.StateProvince, opt => opt.MapFrom(src => src.Address != null ? src.Address.StateProvince : null))
                    .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Address != null ? src.Address.Country : null))
                    .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.Address != null ? src.Address.PostalCode : null));
            });
		}
	}
}