using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.OrderModel;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_BusinessLogic.Service.Implement
{
    public class CustomPLanService : ICustomPlanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomPLanService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }



        #region create (tạo ra khi order của khách ko nhận id của weekly plan có sẵn mà là custome plan của khách)
        public Task<ResponseObject<List<CreateCustomPlanRequest>?>> CreateCustomPlanAsync(Guid orderId, List<CreateCustomPlanRequest> RecipeList)
        {
            throw new NotImplementedException();
        }
        #endregion


    }
}
