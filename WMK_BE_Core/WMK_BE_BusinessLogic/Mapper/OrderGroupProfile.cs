using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.OrderGroupModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.OrderGroupModel;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.Mapper
{
	public class OrderGroupProfile : Profile
	{
        public OrderGroupProfile()
        {
            CreateMap<OrderGroup, OrderGroupsResponse>().ReverseMap();
            CreateMap<OrderGroup, CreateOrderGroupRequest>().ReverseMap();
        }
    }
}
