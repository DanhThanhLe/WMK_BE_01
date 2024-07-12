using WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.IngredientModel;
using WMK_BE_BusinessLogic.ResponseObject;

namespace WMK_BE_BusinessLogic.Service.Interface
{
    public interface IIngredientService
    {
        public Task<ResponseObject<List<IngredientResponse>>> GetIngredients();
        public Task<ResponseObject<IngredientResponse>> GetIngredientById(Guid id);
        public Task<ResponseObject<IngredientResponse>> GetIngredientByName(string name);
        public Task<ResponseObject<IngredientResponse>> CreateIngredient(CreateIngredientRequest ingredient);
        public Task<ResponseObject<IngredientResponse>> UpdateIngredient(IngredientRequest ingredient);
        public Task<ResponseObject<IngredientResponse>> ChangeStatus(UpdateStatusIngredientRequest ingredient);
        public Task<ResponseObject<IngredientResponse>> DeleteIngredientById(Guid id);//xoa han
        public Task<ResponseObject<IngredientResponse>> RemoveIngredientById(Guid id);//chinh status

    }
}
