using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.TransactionModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.TransactionModel;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.Mapper
{
	public class TransactionProfile : Profile
	{
        public TransactionProfile()
        {
            CreateMap<Transaction, ZaloPayCreatePaymentRequest>().ReverseMap();
            CreateMap<Transaction, TransactionResponse>().ReverseMap();
            CreateMap<Transaction, CreatePaymentRequest>().ReverseMap();
            CreateMap<CreatePaymentRequest, RefundZaloPayResponse>().ReverseMap();
        }
    }
}
