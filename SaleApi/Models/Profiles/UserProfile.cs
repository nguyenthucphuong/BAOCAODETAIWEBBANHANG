using AutoMapper;
using SaleApi.Models.Extended;
using SaleApi.Models.Users;
using Common.Utilities;

namespace SaleApi.Models.Profiles
{
	public class UserProfile : Profile
	{
		public UserProfile()
		{
			CreateMap<InputUser, UserEx>()
				.ForMember(dest => dest.Password, opt => opt.MapFrom(src => BCrypt.Net.BCrypt.HashPassword(src.Password)))
				.ForMember(dest => dest.Filter, opt => opt.MapFrom(src => Utility.Filter(src.UserName, src.Email)));
		}
	}
}
