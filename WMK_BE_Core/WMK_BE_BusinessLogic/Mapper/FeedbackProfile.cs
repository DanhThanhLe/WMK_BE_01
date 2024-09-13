using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.FeedbackModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.FeedbackModel;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.Mapper
{
    public class FeedbackProfile : Profile
    {
        public FeedbackProfile()
        {
            CreateMap<Feedback, CreateFeedbackRequest>().ReverseMap();
            CreateMap<Feedback, FeedbackResponse>().ReverseMap();
        }
    }
}
