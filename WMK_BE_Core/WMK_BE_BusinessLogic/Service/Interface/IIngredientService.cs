using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.IngredientModel;
using WMK_BE_BusinessLogic.ResponseObject;

namespace WMK_BE_BusinessLogic.Service.Interface
{
    public interface IIngredientService
    {
        public Task<ResponseObject<IngredientResponse>> GetIngredients();
        public Task<ResponseObject<IngredientResponse>> GetIngredientById(string id);
        public Task<ResponseObject<IngredientResponse>> GetIngredientByName(string name);
        public Task<ResponseObject<IngredientResponse>> CreateIngredient(CreateIngredientRequest ingredient);
        public Task<ResponseObject<IngredientResponse>> UpdateIngredient(IngredientRequest ingredient);
        public Task<ResponseObject<IngredientResponse>> ChangeStatus(UpdateStatusIngredientrequest ingredient);
        public Task<ResponseObject<IngredientResponse>> DeleteIngredientById(IdIngredientRequest ingredient);//xoa han
        public Task<ResponseObject<IngredientResponse>> RemoveIngredientById(IdIngredientRequest ingredient);//chinh status ve Unavailable

    }
}
