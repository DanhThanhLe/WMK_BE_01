using WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.IngredientModel;
using WMK_BE_BusinessLogic.ResponseObject;

namespace WMK_BE_BusinessLogic.Service.Interface
{
	public interface IIngredientService
    {
		#region Get
		public Task<ResponseObject<List<IngredientResponse>>> GetAllAync(string? userId, GetAllIngredientsRequest? model);
        public Task<ResponseObject<IngredientResponse>> GetIngredientById(Guid id);
        public Task<ResponseObject<List<IngredientResponse>>> GetIngredientsByNameAsync(string name);
		#endregion

        public Task<ResponseObject<IngredientResponse>> CreateIngredient(string createdBy,CreateIngredientRequest ingredient);
        
        public Task<ResponseObject<IngredientResponse>> UpdateIngredient(Guid id, CreateIngredientRequest ingredient);
        
        public Task<ResponseObject<IngredientResponse>> DeleteIngredientById(Guid id, string userId);
        
        public Task<ResponseObject<IngredientResponse>> ChangeStatus(Guid id, UpdateStatusIngredientRequest ingredient);
        
        public Task<ResponseObject<IngredientResponse>> RemoveIngredientById(Guid id);//chinh status

    }
}
