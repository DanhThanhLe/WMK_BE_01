using AutoMapper;
using Azure.Core;
using FluentValidation;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientCategoryModel;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientModel;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientNutrientModel;
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
		//private readonly UpdateIngredientValidator _updateValidator;
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
				result.Message = "Not found ingredient";
				return result;
			}
			var changeResult = await _unitOfWork.IngredientRepository.ChangeStatusAsync(id , ingredient.Status);
			if ( changeResult )
			{
				await _unitOfWork.CompleteAsync();
				result.StatusCode = 200;
				result.Message = "Change Ingredient " + id + " status Successfully";
				return result;
			}
			else
			{
				result.StatusCode = 400;
				result.Message = "Change Ingredient " + id + " status Unsuccessfully";
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
			var checkIngredientCategory = await _unitOfWork.IngredientCategoryRepository.GetByIdAsync(ingredient.IngredientCategoryId.ToString());
			if ( checkIngredientCategory == null )
			{
				result.StatusCode = 400;
				result.Message = "Ingredient category with id: " + ingredient.IngredientCategoryId + " not exist";
				return result;
			}
			var found = currentList.FirstOrDefault(i => i.Name == ingredient.Name);
			if ( found != null )
			{
				result.StatusCode = 400;
				result.Message = "Existed with ID: " + found.Id;
				return result;
			}
			Ingredient newIngredient = _mapper.Map<Ingredient>(ingredient);
			newIngredient.CreatedBy = createdBy;
			newIngredient.CreatedAt = DateTime.UtcNow;
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
					result.Message = "Create successfully";
					return result;
				}
				else
				{
					await _unitOfWork.IngredientRepository.DeleteAsync(newIngredient.Id.ToString());
					await _unitOfWork.CompleteAsync();
					result.StatusCode = 500;
					result.Message = "Error at create. Say from CreateIngredient - IngredientService";
					return result;
				}
			}
			else
			{
				result.StatusCode = 500;
				result.Message = "Error at create. Say from CreateIngredient - IngredientService";
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
				result.Message = "Ingredient not exist!";
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
								result.Message = "Auto update recipe unsuccess! Can't delete success";
								return result;
							}
						}
						result.StatusCode = 200;
						result.Message = "Admin delete ingredient success";
					}
					else
					{
						result.StatusCode = 500;
						result.Message = "Admin delete ingredient unsuccess!";
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
									result.Message = "Auto update recipe unsuccess! Can't delete success";
									return result;
								}
							}
							result.StatusCode = 200;
							result.Message = "Jusst change ingredient status success";
						}
						else
						{
							result.StatusCode = 500;
							result.Message = "Update ingredient status unsuccess!";
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
									result.Message = "Auto update recipe unsuccess! Can't delete success";
									return result;
								}
							}
							result.StatusCode = 200;
							result.Message = "Delete ingredient success";
						}
						else
						{
							result.StatusCode = 500;
							result.Message = "Delete ingredient unsuccess!";
						}
					}
				}
				else
				{
					result.StatusCode = 400;
					result.Message = "You do not have permission to use this feature!";
				}
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "User not found!";
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
				result.Message = "OK. Ingredients with id " + id;
				IngredientResponse response = _mapper.Map<IngredientResponse>(ingredients);
				result.Data = response;
				return result;
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "Not found. Data not found or wrong id";
				return result;
			}
		}
		#endregion

		#region Get by name
		public async Task<ResponseObject<List<IngredientResponse>>> GetIngredientByName(string name)
		{
			var result = new ResponseObject<List<IngredientResponse>>();
			var ingredientList = await _unitOfWork.IngredientRepository.GetAllAsync();
			if ( ingredientList != null && ingredientList.Count() > 0 )
			{
				//var foundList = ingredientList.Where(x => x.Name.Contains(name)).ToList();
				var foundList = ingredientList.Where(x => x.Name.ToLower().Contains(name.ToLower())).ToList();
				if ( foundList == null )
				{
					result.StatusCode = 404;
					result.Message = "Not found. No such ingredient in collection contain keyword: " + name;
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
						//tim ten cho approvedBy
						if ( item.UpdatedBy != null )
						{
							Guid.TryParse(item.UpdatedBy , out idConvert);
							userName = _unitOfWork.UserRepository.GetUserNameById(idConvert);
						}
						if ( userName != null )
						{
							item.UpdatedBy = userName;
						}
					}
					result.StatusCode = 200;
					result.Message = "Ingredient list found by name";
					result.Data = returnData;
				}
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "Not found. Empty list or Data not found";
				return result;
			}
			return result;
		}
		#endregion

		#region Get all
		public async Task<ResponseObject<List<IngredientResponse>>> GetIngredients()
		{
			var result = new ResponseObject<List<IngredientResponse>>();
			var ingredients = await _unitOfWork.IngredientRepository.GetAllAsync();
			var responseList = ingredients.ToList();
			if ( responseList != null && responseList.Count() > 0 )
			{
				result.StatusCode = 200;
				result.Message = "OK. Ingredients list";
				var returnResult = _mapper.Map<List<IngredientResponse>>(responseList);
				foreach ( var item in returnResult )
				{
					var userCreate = await _unitOfWork.UserRepository.GetByIdAsync(item.CreatedBy.ToString());
					if ( item.UpdatedBy != null )
					{
						var userUpdate = await _unitOfWork.UserRepository.GetByIdAsync(item.UpdatedBy.ToString());
					}
					if ( userCreate != null )
					{
						item.CreatedBy = userCreate.FirstName + " " + userCreate.LastName;
					}
				}
				result.Data = returnResult;
				#region old
				//foreach ( var ingredientResponse in result.List )
				//{
				//	var ingredient = responseList.FirstOrDefault(i => i.Id == ingredientResponse.Id);
				//	if ( ingredient != null )
				//	{
				//		ingredientResponse.IngredientNutrient = _mapper.Map<IngredientNutrientResponse>(ingredient.IngredientNutrient);
				//                    ingredientResponse.IngredientCategory = _mapper.Map<IngredientCategoryResponse>(ingredient.IngredientCategory);
				//                }
				//}
				#endregion
				return result;
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "Not found. Empty list or Data not found";
				return result;
			}
		}
		#endregion

		#region Remove from app
		public async Task<ResponseObject<IngredientResponse>> RemoveIngredientById(Guid id)
		{
			var result = new ResponseObject<IngredientResponse>();
			if ( id.ToString() == null )
			{
				result.StatusCode = 400;
				result.Message = "Empty request. ingredientId is empty";
				return result;
			}
			var found = await _unitOfWork.IngredientRepository.GetByIdAsync(id.ToString());
			if ( found == null )
			{
				result.StatusCode = 400;
				result.Message = "Not found ingredient";
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
					result.Message = "Remove Success";
					return result;
				}
				else
				{
					result.StatusCode = 500;
					result.Message = "Error at remvove ingredient";
					return result;
				}
			}
		}
		#endregion

		#region Update 
		public async Task<ResponseObject<IngredientResponse>> UpdateIngredient(string updateBy , Guid id , CreateIngredientRequest ingredient)
		{
			var result = new ResponseObject<IngredientResponse>();
			try
			{
				//var validateResult = _updateValidator.Validate(ingredient);
				var currentList = await _unitOfWork.IngredientRepository.GetAllAsync();
				//if (!validateResult.IsValid)//validate
				//{
				//    var error = validateResult.Errors.Select(e => e.ErrorMessage).ToList();
				//    result.StatusCode = 400;
				//    result.Message = string.Join(" - ", error);
				//    return result;
				//}
				var ingredientExist = await _unitOfWork.IngredientRepository.GetByIdAsync(id.ToString());
				if ( ingredientExist == null )//check exist
				{
					result.StatusCode = 404;
					result.Message = "Not found with ID: " + id;
					return result;
				}
				else//bat dau update
				{
					var duplicateName = currentList.FirstOrDefault(i => i.Name == ingredient.Name);//check trung ten voi ingrdient available
					if ( duplicateName != null && duplicateName.Status == BaseStatus.Available && !duplicateName.Id.Equals(id) )
					{
						result.StatusCode = 400;
						result.Message = "Name ingredient existed!";
						return result;
					}
					else
					{
						var updateNutrient = _mapper.Map<IngredientNutrientRequest>(ingredient.NutrientInfo);
						_mapper.Map(ingredient , ingredientExist);
						ingredientExist.UpdatedAt = DateTime.Now;
						ingredientExist.UpdatedBy = updateBy;
						//update ingredient nutrient
						var updateNutrientResult = await _ingredientNutrientService.Update(ingredientExist.IngredientNutrient.Id , updateNutrient);//truyen id cua nutrient va model de update
						if ( updateNutrientResult != null && updateNutrientResult.StatusCode != 200 )
						{
							result.StatusCode = updateNutrientResult.StatusCode;
							result.Message = updateNutrientResult.Message;
							return result;
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
							result.Message = "Update ingredient successfully.";
							return result;
						}
						else
						{
							result.StatusCode = 500;
							result.Message = "Update error";
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
	}
}
