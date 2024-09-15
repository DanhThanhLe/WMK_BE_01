using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.FeedbackModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.FeedbackModel;
using WMK_BE_BusinessLogic.ResponseObject;

namespace WMK_BE_BusinessLogic.Service.Interface
{
	public interface IFeedbackService
	{
		Task<ResponseObject<List<FeedbackResponse>>> Get(string? orderId);
		
		Task<ResponseObject<FeedbackResponse>> CreateFeedback(string userId , CreateFeedbackRequest request);
		
		Task<ResponseObject<FeedbackResponse>> UpdateFeedback(Guid id , string userId , CreateFeedbackRequest request);
	}
}
