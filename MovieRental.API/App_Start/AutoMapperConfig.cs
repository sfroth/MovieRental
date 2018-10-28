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
			});
		}
	}
}