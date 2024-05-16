using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipeStepModel.RecipeStep;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_BusinessLogic.Service.Implement
{
    public class RecipeStepService : IRecipeStepService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RecipeStepService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Get all
        public async Task<ResponseObject<RecipeStepRespone>> GetRecipeSteps()
        {
            var result = new ResponseObject<RecipeStepRespone>();
            var recipeStepList = await _unitOfWork.RecipeStepRepository.GetAllAsync();
            if (recipeStepList != null && recipeStepList.Count > 0)
            {
                result.StatusCode = 200;
                result.Message = "List all recipe steps";
                result.List = _mapper.Map<List<RecipeStepRespone>>(recipeStepList);
                return result;
            }
            else
            {
                result.StatusCode = 404;
                result.Message = "Recipe steps not found";
                return result;
            }
        }
        #endregion
    }
}
