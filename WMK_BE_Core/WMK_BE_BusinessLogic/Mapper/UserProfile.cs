using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.UserModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.UserModel;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.Mapper
{
	public class UserProfile : Profile
	{
        public UserProfile()
        {
            CreateMap<User , UserResponse>().ReverseMap();
            CreateMap<User , UsersResponse>().ReverseMap();
            CreateMap<User , CreateUserRequest>().ReverseMap();
        }
    }
}
