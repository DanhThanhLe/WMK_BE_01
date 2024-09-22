using AutoMapper;
using Azure.Core;
using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientCategoryModel;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientModel;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientNutrientModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.IngredientCategoryModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.IngredientModel;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_BusinessLogic.ValidationModel;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_BusinessLogic.Service.Implement
{
	public class IngredientService : IIngredientService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly IngredientValidator _validator;
		private readonly UpdateStatusIngredientValidator _updateStatusValidator;
		private readonly IIngredientNutrientService _ingredientNutrientService;
		private readonly IRecipeService _recipeService;
		public IngredientService(IMapper mapper , IUnitOfWork unitOfWork , IIngredientNutrientService ingredientNutrientService , IRecipeService recipeService)
		{
			_mapper = mapper;
			_unitOfWork = unitOfWork;
			_validator = new IngredientValidator();
			//_updateValidator = new UpdateIngredientValidator();
			_updateStatusValidator = new UpdateStatusIngredientValidator();
			_ingredientNutrientService = ingredientNutrientService;
			_recipeService = recipeService;
		}
		#region Get all
		public async Task<ResponseObject<List<IngredientResponse>>> GetAllAync(string? userId , GetAllIngredientsRequest? model)
		{
			var result = new ResponseObject<List<IngredientResponse>>();
			var ingredients = new List<Ingredient>();
			var ingredientsResponse = new List<IngredientResponse>();
			if ( model != null && !model.Name.IsNullOrEmpty() )
			{
				var ingredientsByname = await GetIngredientsByNameAsync(model.Name);
				if ( ingredientsByname != null && ingredientsByname.Data != null )
				{
					ingredientsResponse.AddRange(ingredientsByname.Data);
				}
			}
			else
			{
				ingredients = await _unitOfWork.IngredientRepository.GetAllAsync();
				ingredientsResponse = _mapper.Map<List<IngredientResponse>>(ingredients);
			}
			if ( !ingredientsResponse.Any() )
			{
				result.StatusCode = 404;
				result.Message = "Dữ liệu trống";
				result.Data = [];
				return result;
			}
			//user exist by customer
			var userExist = await _unitOfWork.UserRepository.GetByIdAsync(userId);
			if ( userExist != null && userExist.Role == Role.Customer || userExist == null )
			{
				//chỉ hiển thị các recipes đã approve
				ingredientsResponse = ingredientsResponse.Where(r => r.Status == BaseStatus.Available.ToString()).ToList();
			}
			result.StatusCode = 200;
			result.Message = "Tìm thấy "+ ingredientsResponse.Count;
			result.Data = ingredientsResponse.OrderBy(i => i.Name).ToList();
			return result;
		}
		public async Task<ResponseObject<List<IngredientResponse>>> GetIngredientsByNameAsync(string name)
		{
			var result = new ResponseObject<List<IngredientResponse>>();
			var ingredientList = await _unitOfWork.IngredientRepository.GetAllAsync();
			if ( ingredientList != null && ingredientList.Count() > 0 )
			{
				var foundList = ingredientList.Where(x => x.Name.RemoveDiacritics().ToLower().Contains(name.RemoveDiacritics().ToLower())).ToList();
				if ( foundList.Count == 0)
				{
					result.StatusCode = 404;
					result.Message = "Không tìm thấy bản ghi";
					return result;

				}
				else
				{
					var returnData = _mapper.Map<List<IngredientResponse>>(foundList);
					foreach ( var item in returnData )
					{
						string userName = null;
						Guid idConvert;
						//tim ten cho CreatedBy
						if ( item.CreatedBy != null )
						{
							Guid.TryParse(item.CreatedBy , out idConvert);
							userName = _unitOfWork.UserRepository.GetUserNameById(idConvert);
						}
						if ( userName != null )
						{
							item.CreatedBy = userName;
						}
					}
					result.StatusCode = 200;
					result.Message = "Tìm thấy "+foundList.Count;
					result.Data = returnData;
				}
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "Không tìm thấy bản ghi";
				return result;
			}
			return result;
		}
		#endregion
		
		#region Get by ID
		public async Task<ResponseObject<IngredientResponse>> GetIngredientById(Guid id)
		{
			var result = new ResponseObject<IngredientResponse>();
			var ingredients = await _unitOfWork.IngredientRepository.GetByIdAsync(id.ToString());
			if ( ingredients != null )
			{
				result.StatusCode = 200;
				result.Message = "Tìm thấy";
				IngredientResponse response = _mapper.Map<IngredientResponse>(ingredients);
				result.Data = response;
				return result;
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "Không tìm thấy bản ghi";
				return result;
			}
		}
		#endregion

		#region Create
		public async Task<ResponseObject<IngredientResponse>> CreateIngredient(string createdBy , CreateIngredientRequest ingredient)
		{
			var result = new ResponseObject<IngredientResponse>();
			var currentList = await _unitOfWork.IngredientRepository.GetAllAsync();
			var validateResult = _validator.Validate(ingredient);
			if ( !validateResult.IsValid )
			{
				var error = validateResult.Errors.Select(e => e.ErrorMessage).ToList();
				result.StatusCode = 500;
				result.Message = string.Join(" - " , error);
				return result;
			}
			var duplicateName = currentList.FirstOrDefault(i => i.Name == ingredient.Name);//check trung ten voi ingrdient available
			if ( duplicateName != null && duplicateName.Status == BaseStatus.Available )
			{
				result.StatusCode = 400;
				result.Message = "Trùng tên";
				return result;
			}
			var checkIngredientCategory = await _unitOfWork.IngredientCategoryRepository.GetByIdAsync(ingredient.IngredientCategoryId.ToString());
			if ( checkIngredientCategory == null )
			{
				result.StatusCode = 400;
				result.Message = "Ingredient category không tồn tại";
				return result;
			}
			//var found = currentList.FirstOrDefault(i => i.Name == ingredient.Name);
			//if ( found != null )
			//{
			//	result.StatusCode = 400;
			//	result.Message = "Existed with ID: " + found.Id;
			//	return result;
			//}
			Ingredient newIngredient = _mapper.Map<Ingredient>(ingredient);
			newIngredient.CreatedBy = createdBy;
			newIngredient.CreatedAt = DateTime.UtcNow.AddHours(7);
			newIngredient.IngredientCategory = checkIngredientCategory;
			var createResult = await _unitOfWork.IngredientRepository.CreateAsync(newIngredient);
			if ( createResult )
			{
				await _unitOfWork.CompleteAsync();
				//bat dau tao IngredientNutrient
				var createIngredientNutrient = await _ingredientNutrientService.Create(newIngredient.Id , ingredient.NutrientInfo);
				if ( createIngredientNutrient.StatusCode == 200 && createIngredientNutrient.Data != null )
				{
					//await _unitOfWork.CompleteAsync(); //ko can cai nay
					result.StatusCode = 200;
					result.Message = "Tạo thành công";
					return result;
				}
				else
				{
					await _unitOfWork.IngredientRepository.DeleteAsync(newIngredient.Id.ToString());
					await _unitOfWork.CompleteAsync();
					result.StatusCode = createIngredientNutrient.StatusCode;
					result.Message = createIngredientNutrient.Message;
					return result;
				}
			}
			else
			{
				result.StatusCode = 500;
				result.Message = "Lỗi bất định";
				return result;
			}
		}
		#endregion

		#region Update 
		public async Task<ResponseObject<IngredientResponse>> UpdateIngredient( Guid id , CreateIngredientRequest ingredient)
		{
			var result = new ResponseObject<IngredientResponse>();
			try
			{
				var validateResult = _validator.Validate(ingredient);
				var currentList = await _unitOfWork.IngredientRepository.GetAllAsync();
				if ( !validateResult.IsValid )//validate
				{
					var error = validateResult.Errors.Select(e => e.ErrorMessage).ToList();
					result.StatusCode = 400;
					result.Message = string.Join(" - " , error);
					return result;
				}
				var ingredientExist = await _unitOfWork.IngredientRepository.GetByIdAsync(id.ToString());
				if ( ingredientExist == null )//check exist
				{
					result.StatusCode = 404;
					result.Message = "Không tìm thấy bản ghi";
					return result;
				}
				else//bat dau update
				{
					var duplicateName = currentList.FirstOrDefault(i => i.Name == ingredient.Name);//check trung ten voi ingrdient available
					if ( duplicateName != null && duplicateName.Status == BaseStatus.Available && !duplicateName.Id.Equals(id) )
					{
						result.StatusCode = 400;
						result.Message = "Không tìm thấy bản ghi";
						return result;
					}
					else
					{
						var updateNutrient = _mapper.Map<IngredientNutrientRequest>(ingredient.NutrientInfo);
						_mapper.Map(ingredient , ingredientExist);
						//update ingredient nutrient
						var updateNutrientResult = await _ingredientNutrientService.Update(ingredientExist.IngredientNutrient.Id , updateNutrient);//truyen id cua nutrient va model de update
						if ( updateNutrientResult != null && updateNutrientResult.StatusCode != 200 )
						{
							result.StatusCode = updateNutrientResult.StatusCode;
							result.Message = updateNutrientResult.Message;
							return result;
						}
						//gọi lại hàm để cập nhập các recipe có liên quan đến ingredient
						var recipeIngredients = ingredientExist.RecipeIngredients.Where(ri => ri.IngredientId == ingredientExist.Id).ToList();
						foreach ( var recipeIngredient in recipeIngredients )
						{
							//update lại recipe nào có chứa ingredient
							var autoUpdateRecipe = await _recipeService.AutoUpdateRecipeAsync(recipeIngredient.RecipeId);
							if ( !autoUpdateRecipe )
							{
								result.StatusCode = 500;
								result.Message = "Tự cập nhật công thức liên quan thất bại.";//"Auto update recipe unsuccess! Can't delete success";
								return result;
							}
						}
						//update recipe info
						var updateRecipeInfo = await _recipeService.UpdateRecipeByIngredient(ingredientExist.Id);
						if ( updateRecipeInfo != null && updateRecipeInfo.StatusCode != 200 )
						{
							result.StatusCode = updateRecipeInfo.StatusCode;
							result.Message = updateRecipeInfo.Message;
							return result;
						}
						var updateResult = await _unitOfWork.IngredientRepository.UpdateAsync(ingredientExist);
						if ( updateResult )
						{
							await _unitOfWork.CompleteAsync();
							result.StatusCode = 200;
							result.Message = "Cập nhật thành công";
							return result;
						}
						else
						{
							result.StatusCode = 500;
							result.Message = "Cập nhật không thành công";
							return result;
						}
					}
				}
			}
			catch ( Exception ex )
			{
				result.StatusCode = 500;
				result.Message = ex.Message;
				return result;
			}
		}
		#endregion

		#region Delete from Database
		public async Task<ResponseObject<IngredientResponse>> DeleteIngredientById(Guid id , string userId)
		{
			//chỉ có chính user tạo ra ingredient mới được quyền xóa, nếu xóa rồi thì trên recipe sẽ mất
			var result = new ResponseObject<IngredientResponse>();
			var ingredientExist = await _unitOfWork.IngredientRepository.GetByIdAsync(id.ToString());
			if ( ingredientExist == null )
			{
				result.StatusCode = 404;
				result.Message = "Không tìm thấy bản ghi";
				return result;
			}

			//check user exist
			var userExist = await _unitOfWork.UserRepository.GetByIdAsync(userId);

			if ( userExist != null )
			{
				if ( userExist.Role == Role.Admin )
				{
					// Admin: delete luôn ingredient
					var deleteResult = await _unitOfWork.IngredientRepository.DeleteAsync(id.ToString());
					if ( deleteResult )
					{
						//gọi lại hàm để cập nhập các recipe có liên quan đến ingredient
						var recipeIngredients = ingredientExist.RecipeIngredients.Where(ri => ri.IngredientId == ingredientExist.Id).ToList();
						foreach ( var recipeIngredient in recipeIngredients )
						{
							var autoUpdateRecipe = await _recipeService.AutoUpdateRecipeAsync(recipeIngredient.RecipeId);
							if ( !autoUpdateRecipe )
							{
								result.StatusCode = 500;
								result.Message = "Cập nhật tự động công thức không thành công";//"Auto update recipe unsuccess! Can't delete success";
								return result;
							}
						}
						result.StatusCode = 200;
						result.Message = "Xóa thành công";
					}
					else
					{
						result.StatusCode = 500;
						result.Message = "Xóa không thành công";
					}
				}
				else if ( userExist.Id.ToString() == ingredientExist.CreatedBy )
				{
					//kiểm tra xem ingredient có tồn tại trong recipe chưa nếu chưa thì xóa còn có rồi thì chỉ đổi trạng thái
					var recipeIngredientExist = _unitOfWork.RecipeIngredientRepository.Get(ri => ri.IngredientId == ingredientExist.Id).FirstOrDefault();

					if ( recipeIngredientExist != null )
					{
						//just change status
						ingredientExist.Status = BaseStatus.UnAvailable;
						var updateRecipe = await _recipeService.UpdateRecipeByIngredient(id);

						var updateIngredientResult = await _unitOfWork.IngredientRepository.UpdateAsync(ingredientExist);
						if ( updateIngredientResult )
						{
							//gọi lại hàm để cập nhập các recipe có liên quan đến ingredient
							var recipeIngredients = ingredientExist.RecipeIngredients.Where(ri => ri.IngredientId == ingredientExist.Id).ToList();
							foreach ( var recipeIngredient in recipeIngredients )
							{
								var autoUpdateRecipe = await _recipeService.AutoUpdateRecipeAsync(recipeIngredient.RecipeId);
								if ( !autoUpdateRecipe )
								{
									result.StatusCode = 500;
									result.Message = "Cập nhật tự động công thức không thành công";
									return result;
								}
							}
							result.StatusCode = 200;
							result.Message = "Chuyển trạng thái thành công";
						}
						else
						{
							result.StatusCode = 500;
							result.Message = "Chuyển trạng thái không thành công";
						}
					}
					else
					{
						//delete
						var deleteResult = await _unitOfWork.IngredientRepository.DeleteAsync(id.ToString());
						if ( deleteResult )
						{
							//gọi lại hàm để cập nhập các recipe có liên quan đến ingredient
							var recipeIngredients = ingredientExist.RecipeIngredients.Where(ri => ri.IngredientId == ingredientExist.Id).ToList();
							foreach ( var recipeIngredient in recipeIngredients )
							{
								var autoUpdateRecipe = await _recipeService.AutoUpdateRecipeAsync(recipeIngredient.RecipeId);
								if ( !autoUpdateRecipe )
								{
									result.StatusCode = 500;
									result.Message = "Cập nhật tự động công thức không thành công";
									return result;
								}
							}
							result.StatusCode = 200;
							result.Message = "Xóa thành công";
						}
						else
						{
							result.StatusCode = 500;
							result.Message = "Xóa thành công";
						}
					}
				}
				else
				{
					result.StatusCode = 400;
					result.Message = "Người dùng không có quyền thực hiện thao tác";
				}
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "Người dùng không tồn tại. Không có quyền thực hiện";
			}
			return result;
		}

		#endregion

		#region Remove from app
		public async Task<ResponseObject<IngredientResponse>> RemoveIngredientById(Guid id)
		{
			var result = new ResponseObject<IngredientResponse>();
			if ( id.ToString() == null )
			{
				result.StatusCode = 400;
				result.Message = "Yêu cầu rỗng";
				return result;
			}
			var found = await _unitOfWork.IngredientRepository.GetByIdAsync(id.ToString());
			if ( found == null )
			{
				result.StatusCode = 400;
				result.Message = "Không tìm thấy bản ghi";
				return result;
			}
			else
			{
				found.Status = BaseStatus.UnAvailable;
				var removeResult = await _unitOfWork.IngredientRepository.UpdateAsync(found);
				if ( removeResult )
				{
					await _unitOfWork.CompleteAsync();
					result.StatusCode = 200;
					result.Message = "Dời thành công";
					return result;
				}
				else
				{
					result.StatusCode = 500;
					result.Message = "Lỗi bất định";
					return result;
				}
			}
		}
		#endregion

		#region Change status
		public async Task<ResponseObject<IngredientResponse>> ChangeStatus(Guid id , UpdateStatusIngredientRequest ingredient)
		{
			var result = new ResponseObject<IngredientResponse>();
			var validateResult = _updateStatusValidator.Validate(ingredient);
			if ( !validateResult.IsValid )
			{
				var error = validateResult.Errors.Select(e => e.ErrorMessage).ToList();
				result.StatusCode = 400;
				result.Message = string.Join(" - " , error);
				return result;
			}
			var found = await _unitOfWork.IngredientRepository.GetByIdAsync(id.ToString());
			if ( found == null )
			{
				result.StatusCode = 400;
				result.Message = "Không tìm thấy bản ghi";
				return result;
			}
			var changeResult = await _unitOfWork.IngredientRepository.ChangeStatusAsync(id , ingredient.Status);
			if ( changeResult )
			{
				await _unitOfWork.CompleteAsync();
				result.StatusCode = 200;
				result.Message = "Chuyển trạng thái thành công";
				return result;
			}
			else
			{
				result.StatusCode = 400;
				result.Message = "Chuyển trạng thái không thành công";
				return result;
			}
		}
		#endregion
	}
}
