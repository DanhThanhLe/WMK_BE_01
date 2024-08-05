using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.NutritionModel;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.Recipe;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.RecipeModel;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.RecipeStepModel.RecipeStep;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.OrderGroupModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.Recipe;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipeNutrientModel;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_BusinessLogic.ValidationModel;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WMK_BE_BusinessLogic.Service.Implement
{
	public class RecipeService : IRecipeService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IRecipeIngredientService _recipeAmountService;
		private readonly RecipeValidator _validator;
		private readonly IMapper _mapper;
		private readonly RecipeChangeStatusValidator _recipeChangeStatusValidator;
		private readonly IdRecipeValidator _idValidator;
		private readonly IRecipeCategoryService _recipeCategoryService;
		private readonly IRecipeStepService _recipeStepService;
		private readonly IRecipeNutrientService _recipeNutrientService;
		private readonly IRecipeIngredientService _recipeIngredientService;
		private readonly IUserService _userService;
		public RecipeService(IUnitOfWork unitOfWork , IMapper mapper , IRecipeIngredientService recipeAmountService , IRecipeCategoryService recipeCategoryService , IRecipeNutrientService recipeNutrientService , IRecipeIngredientService recipeIngredientService , IRecipeStepService recipeStepService , IUserService userService)
		{
			_unitOfWork = unitOfWork;
			_recipeAmountService = recipeAmountService;
			_mapper = mapper;
			_validator = new RecipeValidator();
			_recipeChangeStatusValidator = new RecipeChangeStatusValidator();
			_idValidator = new IdRecipeValidator();
			_recipeCategoryService = recipeCategoryService;
			_recipeNutrientService = recipeNutrientService;
			_recipeIngredientService = recipeIngredientService;
			_recipeStepService = recipeStepService;
			_userService = userService;
		}

		private async Task<List<Recipe>> GetAllToProcess()
		{
			var currentList = await _unitOfWork.RecipeRepository.GetAllAsync();
			if ( currentList.Any() )
			{
				return currentList;
			}
			return new List<Recipe>();
		}


		#region Get all
		public async Task<ResponseObject<List<RecipeResponse>>> GetAllRecipesAsync(string? userId , GetAllRecipesRequest? model)
		{
			var result = new ResponseObject<List<RecipeResponse>>();
			var recipes = new List<Recipe>();
			var recipesResponse = new List<RecipeResponse>();
			if ( model != null && (!model.Name.IsNullOrEmpty()
								|| model.Difficulty != null
								|| !model.Description.IsNullOrEmpty()
								|| model.ServingSize != null) )
			{
				if ( !model.Name.IsNullOrEmpty() )
				{
					var recipesByName = await GetRecipesByNameAsync(model.Name);
					if ( recipesByName != null && recipesByName.Data != null )
					{
						recipesResponse.AddRange(recipesByName.Data);
					}
				}
				if ( model.Difficulty != null )
				{
					var recipesByDiff = await GetRecipesByDifficultyAsync(model.Difficulty);
					if ( recipesByDiff != null && recipesByDiff.Data != null )
					{
						recipesResponse.AddRange(recipesByDiff.Data);
					}
				}
				if ( !model.Description.IsNullOrEmpty() )
				{
					var recipesByDescription = await GetRecipesByDescriptionAsync(model.Description);
					if ( recipesByDescription != null && recipesByDescription.Data != null )
					{
						recipesResponse.AddRange(recipesByDescription.Data);
					}
				}
				if ( model.ServingSize != null )
				{
					var recipesByServingSize = await GetRecipesByServingSizeAsync(model.ServingSize);
					if ( recipesByServingSize != null && recipesByServingSize.Data != null )
					{
						recipesResponse.AddRange(recipesByServingSize.Data);
					}
				}
				// Loại bỏ các phần tử trùng lặp dựa trên Id
				recipesResponse = recipesResponse
					.GroupBy(c => c.Id)
					.Select(g => g.First())
					.ToList();
			}
			else
			{
				recipes = await _unitOfWork.RecipeRepository.GetAllAsync();
				recipesResponse = _mapper.Map<List<RecipeResponse>>(recipes);
			}
			if ( !recipesResponse.Any() )
			{
				result.StatusCode = 404;
				result.Message = "Not have Any recipe!";
				return result;
			}
			foreach ( var item in recipesResponse )
			{
				Guid idConvert;
				if ( item.CreatedBy != null )
				{
					Guid.TryParse(item.CreatedBy , out idConvert);
					item.CreatedBy = _unitOfWork.UserRepository.GetUserNameById(idConvert);
				}
				//tim ten cho approvedBy
				if ( item.ApprovedBy != null )
				{
					Guid.TryParse(item.ApprovedBy , out idConvert);
					item.ApprovedBy = _unitOfWork.UserRepository.GetUserNameById(idConvert);
				}
				if ( item.UpdatedBy != null )
				{
					Guid.TryParse(item.UpdatedBy , out idConvert);
					item.UpdatedBy = _unitOfWork.UserRepository.GetUserNameById(idConvert);
				}
			}
			//user exist by customer
			var userExist = await _unitOfWork.UserRepository.GetByIdAsync(userId);
			if ( userExist != null && userExist.Role == Role.Customer || userExist == null )
			{
				//chỉ hiển thị các recipes đã approve
				recipesResponse = recipesResponse.Where(r => r.BaseStatus == BaseStatus.Available.ToString()).ToList();
			}
			////nếu user là staff thì chỉ hiển thị các recipe của nó
			//if ( userExist != null && userExist.Role == Role.Staff )
			//{
			//	recipesResponse = recipesResponse.Where(r => r.CreatedBy.Equals(userExist.Id)).ToList();
			//}
			result.StatusCode = 200;
			result.Message = "Get Recipe list success (" + recipesResponse.Count() + ")";
			result.Data = recipesResponse;
			return result;
		}

		public async Task<ResponseObject<List<RecipeResponse>>> GetRecipesByServingSizeAsync(int? servingSize)
		{
			var result = new ResponseObject<List<RecipeResponse>>();

			var recipes = await _unitOfWork.RecipeRepository.GetAllAsync();
			var foundList = recipes.Where(r => r.ServingSize == servingSize)
											.ToList();
			if ( foundList != null && foundList.Any() )
			{
				result.StatusCode = 200;
				result.Message = "List recipes by difficult";
				result.Data = _mapper.Map<List<RecipeResponse>>(foundList);
				return result;
			}
			result.StatusCode = 404;
			result.Message = "Not have list recipes by difficult!";
			return result;
		}

		public async Task<ResponseObject<List<RecipeResponse>>> GetRecipesByDescriptionAsync(string? description)
		{
			var result = new ResponseObject<List<RecipeResponse>>();

			var recipes = await _unitOfWork.RecipeRepository.GetAllAsync();
			var foundList = recipes.Where(r => !string.IsNullOrWhiteSpace(r.Description)
											&& !string.IsNullOrWhiteSpace(description)
											&& r.Description.ToLower().RemoveDiacritics()
											.Contains(description.ToLower().RemoveDiacritics()))
											.ToList();
			if ( foundList != null && foundList.Any() )
			{
				result.StatusCode = 200;
				result.Message = "List recipes by difficult";
				result.Data = _mapper.Map<List<RecipeResponse>>(foundList);
				return result;
			}
			result.StatusCode = 404;
			result.Message = "Not have list recipes by difficult!";
			return result;
		}

		public async Task<ResponseObject<List<RecipeResponse>>> GetRecipesByDifficultyAsync(LevelOfDifficult? difficulty)
		{
			var result = new ResponseObject<List<RecipeResponse>>();

			var recipes = await _unitOfWork.RecipeRepository.GetAllAsync();
			var foundList = recipes.Where(r => r.Difficulty == difficulty).ToList();
			if ( foundList != null && foundList.Any() )
			{
				result.StatusCode = 200;
				result.Message = "List recipes by difficult";
				result.Data = _mapper.Map<List<RecipeResponse>>(foundList);
				return result;
			}
			result.StatusCode = 404;
			result.Message = "Not have list recipes by difficult!";
			return result;
		}

		public async Task<ResponseObject<List<RecipeResponse>>> GetRecipesByNameAsync(string name)
		{
			var result = new ResponseObject<List<RecipeResponse>>();

			var recipes = await _unitOfWork.RecipeRepository.GetAllAsync();
			var foundList = recipes.Where(x => x.Name.RemoveDiacritics().ToLower().Contains(name.ToLower().RemoveDiacritics())).ToList();
			if ( foundList != null && foundList.Any() )
			{
				result.StatusCode = 200;
				result.Message = "Recipe list found by name";
				result.Data = _mapper.Map<List<RecipeResponse>>(foundList);
				return result;
			}
			result.StatusCode = 404;
			result.Message = "Recipe list not found by name: " + name;
			return result;
		}
		#endregion

		#region Get all for create custome plan
		public async Task<ResponseObject<List<RecipeResponse>>> GetForCustomPlan()
		{
			var result = new ResponseObject<List<RecipeResponse>>();
			var currentList = await GetAllToProcess();
			var responseList = currentList.ToList().Where(x => x.ProcessStatus == ProcessStatus.Approved && x.BaseStatus == BaseStatus.Available);
			if ( currentList != null && currentList.Count() > 0 )
			{
				result.StatusCode = 200;
				result.Message = "OK. Recipe list " + responseList.Count();
				result.Data = _mapper.Map<List<RecipeResponse>>(responseList);
				return result;
			}
			else
			{
				result.StatusCode = 500;
				result.Message = "Not found. Empty list or Data not found. Say from GetRecipes - RecipeService";
				return result;
			}
		}


		#endregion

		#region Get by ID
		public async Task<ResponseObject<RecipeResponse>> GetRecipeById(string id)
		{
			var result = new ResponseObject<RecipeResponse>();
			var recipe = await _unitOfWork.RecipeRepository.GetByIdAsync(id);
			if ( recipe != null && recipe.Id.ToString() != null )
			{
				string userName = null;
				Guid idConvert;
				//tim ten cho CreatedBy
				if ( recipe.CreatedBy != null )
				{
					Guid.TryParse(recipe.CreatedBy , out idConvert);
					userName = _unitOfWork.UserRepository.GetUserNameById(idConvert);
				}
				if ( userName != null )
				{
					recipe.CreatedBy = userName;
				}
				//tim ten cho approvedBy
				if ( recipe.ApprovedBy != null )
				{
					Guid.TryParse(recipe.ApprovedBy , out idConvert);
					userName = _unitOfWork.UserRepository.GetUserNameById(idConvert);
				}
				if ( userName != null )
				{
					recipe.ApprovedBy = userName;
				}
				RecipeResponse response = _mapper.Map<RecipeResponse>(recipe);
				result.StatusCode = 200;
				result.Message = "Recipe with Id " + id + ":";
				result.Data = response;
				return result;
			}
			else
			{
				result.StatusCode = 404;
				result.Message = "Not found. Data not found or wrong id";
				return result;
			}
			throw new NotImplementedException();
		}
		#endregion

		#region Search
		public async Task<ResponseObject<List<RecipeResponse>>> GetRecipesByNameStatusAsync(string name = "" , bool status = true)//tim bang ten cua recipe lan cua ca category
		{
			name.Trim();
			name = name.RemoveDiacritics();
			var result = new ResponseObject<List<RecipeResponse>>();
			var currentList = await GetAllToProcess();
			if ( currentList != null && currentList.Count() > 0 )
			{

				var foundList = new List<Recipe>();
				if ( status )//search de dat hang
				{
					foundList = currentList.Where(x => (x.Name.Trim().RemoveDiacritics().ToLower().Contains(name.ToLower()) || x.RecipeCategories.Where(y => y.Category.Name.Trim().RemoveDiacritics().ToLower().Contains(name.ToLower())).Any()) && x.ProcessStatus == ProcessStatus.Approved && x.BaseStatus == BaseStatus.Available).ToList();
				}
				else
				{
					foundList = currentList.Where(x => (x.Name.Trim().RemoveDiacritics().ToLower().Contains(name.ToLower()) || x.RecipeCategories.Where(y => y.Category.Name.Trim().RemoveDiacritics().ToLower().Contains(name.ToLower())).Any()) && x.ProcessStatus == ProcessStatus.Approved).ToList();
				}


				if ( foundList.Count == 0 )
				{
					result.StatusCode = 404;
					result.Message = "Not found. No such recipe in collection contain keyword: " + name;
					return result;
				}
				else
				{
					var returnList = _mapper.Map<List<RecipeResponse>>(foundList);
					foreach ( var item in returnList )
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
						if ( item.ApprovedBy != null )
						{
							Guid.TryParse(item.ApprovedBy , out idConvert);
							userName = _unitOfWork.UserRepository.GetUserNameById(idConvert);
						}
						if ( userName != null )
						{
							item.ApprovedBy = userName;
						}
					}
					result.StatusCode = 200;
					result.Message = "Recipe list found by name";
					result.Data = returnList;

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

		#region Create
		public async Task<ResponseObject<RecipeResponse>> CreateRecipeAsync(string createdBy , CreateRecipeRequest recipe)
		{
			var result = new ResponseObject<RecipeResponse>();
			var currentList = await GetAllToProcess();
			try
			{
				var validateResult = _validator.Validate(recipe);
				if ( !validateResult.IsValid )
				{
					var error = validateResult.Errors.Select(e => e.ErrorMessage).ToList();
					result.StatusCode = 400;
					result.Message = "Error validate " + string.Join(" - /n" , error);
					return result;
				}
				//mapper
				Recipe newRecipe = _mapper.Map<Recipe>(recipe);
				newRecipe.CreatedAt = DateTime.Now;
				newRecipe.CreatedBy = createdBy;
				newRecipe.Popularity = 0;
				newRecipe.BaseStatus = BaseStatus.UnAvailable;
				newRecipe.ProcessStatus = ProcessStatus.Processing;
				var checkDuplicateName = currentList.FirstOrDefault(x => x.Name.ToLower().Trim().Equals(recipe.Name.ToLower().Trim()));
				if ( checkDuplicateName != null )
				{
					result.StatusCode = 400;
					result.Message = "Duplicate name: " + checkDuplicateName.Name;
					return result;
				}

				var createResult = await _unitOfWork.RecipeRepository.CreateAsync(newRecipe);
				if ( !createResult )
				{
					result.StatusCode = 500;
					result.Message = "Create recipe unsucess!";
					return result;
				}
				await _unitOfWork.CompleteAsync();

				//bat dau tao cac thanh phan lien quan

				//create RecipeCategory
				var checkCreateRecipeCategory = await _recipeCategoryService.Create(newRecipe.Id , recipe.CategoryIds);
				if ( checkCreateRecipeCategory.StatusCode != 200 && checkCreateRecipeCategory.Data == null )
				{
					await resetRecipe(newRecipe.Id);
					result.StatusCode = 500;
					result.Message = checkCreateRecipeCategory.Message;
					return result;
				}
				//create RecipeIngredient
				var checkCreateRecipeIngredient = await _recipeIngredientService.CreateRecipeIngredientAsync(newRecipe.Id , recipe.RecipeIngredientsList);
				if ( checkCreateRecipeIngredient.StatusCode != 200 && checkCreateRecipeIngredient.Data == null )
				{
					await resetRecipe(newRecipe.Id);
					result.StatusCode = 500;
					result.Message = checkCreateRecipeIngredient.Message;
					return result;
				}
				//create RecipeStep
				var checkCreateRecipeStep = await _recipeStepService.CreateRecipeSteps(newRecipe.Id , recipe.Steps);
				if ( checkCreateRecipeStep.StatusCode != 200 && checkCreateRecipeStep.Data == null )
				{
					await resetRecipe(newRecipe.Id);
					result.StatusCode = 500;
					result.Message = checkCreateRecipeStep.Message;
					return result;
				}
				else//ko co loi va hoan thanh tao moi
				{
					//tạo giá cho recipe
					var updatePrice = await UpdatePrice(newRecipe.Id);
					if ( !updatePrice )
					{
						await resetRecipe(newRecipe.Id);
						result.StatusCode = 500;
						result.Message = "Update recipe price unsuccessfully"!;
						return result;
					}
					//create RecipeNutrient
					var updateNutrientResult = await _recipeNutrientService.AutoUpdateNutrientByRecipe(newRecipe.Id);
					if ( !updateNutrientResult )
					{
						await resetRecipe(newRecipe.Id);
						result.StatusCode = 500;
						result.Message = "Update recipe nutrient unsuccessfully!";
						return result;
					}
					result.StatusCode = 200;
					result.Message = "Create Recipe successfully.";
					result.Data = _mapper.Map<RecipeResponse>(newRecipe);
					return result;
				}
				#region old
				//newRecipe.RecipeCategories = checkCreateRecipeCategory.Data.ToList();
				//create RecipeCategory

				//if ()
				//{
				//    resetRecipe(newRecipe.Id);
				//    result.StatusCode = checkCreateRecipeIngredient.StatusCode;
				//    result.Message = ;
				//    return result;
				//}
				//newRecipe.RecipeIngredients = checkCreateRecipeIngredient.Data.ToList();
				////create RecipeIngredient



				//if ()
				//{
				//    resetRecipe(newRecipe.Id);
				//    result.StatusCode = checkCreateRecipeIngredient.StatusCode;
				//    result.Message = ;
				//    return result;
				//}
				//newRecipe.RecipeSteps = checkCreateRecipeStep.Data.ToList();
				////create RecipeStep


				//if ()
				//{
				//    resetRecipe(newRecipe.Id);
				//    result.StatusCode = checkCreateRecipeIngredient.StatusCode;
				//    result.Message = checkCreateRecipeIngredient.Message;
				//    return result;
				//}
				//else
				//{
				//    await _unitOfWork.CompleteAsync();
				//}
				#endregion
			}
			catch ( Exception ex )
			{
				result.StatusCode = 500;
				result.Message = ex.Message;
				return result;
			}
		}
		#endregion
		//reset Recipe
		private async Task<bool> resetRecipe(Guid recipeId)
		{
			var recipe = await _unitOfWork.RecipeRepository.GetByIdAsync(recipeId.ToString());
			if ( recipe != null )
			{
				await _unitOfWork.RecipeRepository.DeleteAsync(recipe.Id.ToString());
				await _unitOfWork.CompleteAsync();
				return true;
			}
			return false;
		}
		private async Task<bool> deleteRecipeFromWeeklyPlan(Guid recipeId , ProcessStatus status , BaseStatus? baseStatus)
		{
			var recipeExist = await _unitOfWork.RecipeRepository.GetByIdAsync(recipeId.ToString());
			if ( recipeExist != null )
			{
				if ( status == ProcessStatus.Denied || baseStatus != null )
				{
					//Xóa khỏi wpl
					var recipePlans = await _unitOfWork.RecipePlanRepository.GetAllAsync();
					var recipePLansExist = recipePlans.Where(rp => rp.RecipeId == recipeId).ToList();
					if ( recipePLansExist.Count > 0 )
					{
						foreach ( var recipePlan in recipePLansExist )
						{
							//delete recipePLan
							var deleteResult = await _unitOfWork.RecipePlanRepository.DeleteAsync(recipePlan.Id.ToString());
							if ( deleteResult )
							{
								await _unitOfWork.CompleteAsync();
							}
						}
						return true;
					}
				}
			}
			return false;
		}


		#region Update (26/05/2024)
		public async Task<ResponseObject<RecipeResponse>> Update(RecipeRequest updateRecipe)
		{
			/*
			tim recipe
			lay du lieu goc
			thay doi cho cac thong so
			giu nguyen cho cac thong so bo trong
			 */
			var result = new ResponseObject<RecipeResponse>();
			var foundRecipe = await _unitOfWork.RecipeRepository.GetByIdAsync(updateRecipe.Id.ToString());
			if ( foundRecipe != null )
			{
				result.StatusCode = 400;
				result.Message = "Not found recipe";
				return result;
			}
			var currentList = await GetAllToProcess();
			var duplicateName = currentList.FirstOrDefault(x => x.Name == updateRecipe.Name);
			if ( duplicateName != null
				&& duplicateName.Id != updateRecipe.Id
				&& duplicateName.ProcessStatus == ProcessStatus.Processing )
			{
				result.StatusCode = 400;
				result.Message = "Recipe with name: " + updateRecipe.Name + " is already existed";
				return result;
			}
			if ( updateRecipe.Name != null )
			{
				foundRecipe.Name = updateRecipe.Name;
			}
			if ( updateRecipe.ServingSize != null )
			{
				foundRecipe.ServingSize = updateRecipe.ServingSize;
			}
			if ( updateRecipe.Difficulty != null )
			{
				foundRecipe.Difficulty = updateRecipe.Difficulty;
			}
			if ( updateRecipe.Description != null )
			{
				foundRecipe.Description = updateRecipe.Description;
			}
			if ( updateRecipe.ImageLink != null )
			{
				foundRecipe.Img = updateRecipe.ImageLink;
			}
			if ( updateRecipe.Price != null )
			{
				foundRecipe.Price = updateRecipe.Price;
			}
			if ( updateRecipe.ApprovedBy != null )
			{
				foundRecipe.ApprovedBy = updateRecipe.ApprovedBy;
			}
			if ( updateRecipe.ApprovedAt != null )
			{
				foundRecipe.ApprovedAt = updateRecipe.ApprovedAt;
			}
			if ( updateRecipe.UpdatedBy != null )
			{
				foundRecipe.UpdatedBy = updateRecipe.UpdatedBy;
			}
			foundRecipe.UpdatedAt = DateTime.Now;
			if ( updateRecipe.Popularity != null )
			{
				foundRecipe.Popularity = updateRecipe.Popularity;
			}
			if ( updateRecipe.ProcessStatus != null )
			{
				foundRecipe.ProcessStatus = updateRecipe.ProcessStatus;
			}
			var updateResult = await _unitOfWork.RecipeRepository.UpdateAsync(foundRecipe);
			if ( !updateResult )
			{
				result.StatusCode = 500;
				result.Message = "Error when updating recipe in recipe service using updateAsync";
				return result;
			}
			result.StatusCode = 500;
			result.Message = "Update recipe id " + updateRecipe.Id + " done";
			return result;
		}
		#endregion

		#region Change status -- just manager use
		public async Task<ResponseObject<RecipeResponse>> ChangeStatus(Guid id , ChangeRecipeStatusRequest recipe)
		{
			var result = new ResponseObject<RecipeResponse>();
			var validateResult = _recipeChangeStatusValidator.Validate(recipe);
			if ( !validateResult.IsValid )
			{
				var error = validateResult.Errors.Select(e => e.ErrorMessage).ToList();
				result.StatusCode = 400;
				result.Message = string.Join(" - " , error);
				return result;
			}
			var recipeExist = await _unitOfWork.RecipeRepository.GetByIdAsync(id.ToString());
			if ( recipeExist == null )
			{
				result.StatusCode = 404;
				result.Message = "Not found recipe id " + id + "!";
				return result;
			}
			if ( recipe.Notice.IsNullOrEmpty() )
			{
				recipeExist.Notice = "Not have";
			}
			_mapper.Map(recipe , recipeExist);
			if ( recipeExist.ProcessStatus == ProcessStatus.Processing
				|| recipeExist.ProcessStatus == ProcessStatus.Denied
				|| recipeExist.ProcessStatus == ProcessStatus.Cancel )
			{
				recipeExist.BaseStatus = BaseStatus.UnAvailable;
			}
			var changeResult = await _unitOfWork.RecipeRepository.UpdateAsync(recipeExist);
			if ( changeResult )
			{
				//Nếu thay đổi status sang denied thì xóa recipe khỏi wpl
				if ( recipeExist.ProcessStatus == ProcessStatus.Denied && recipeExist.RecipePlans != null )
				{
					var deleteWp = await deleteRecipeFromWeeklyPlan(recipeExist.Id , recipe.ProcessStatus , null);
					if ( !deleteWp )
					{
						result.StatusCode = 500;
						result.Message = "Delete recipe form weeklyplan unsuccess";
						return result;
					}
				}
				await _unitOfWork.CompleteAsync();
				result.StatusCode = 200;
				result.Message = "Change status success";
				return result;
			}
			else
			{
				result.StatusCode = 500;
				result.Message = "Change Recipe " + id + " status Unsuccessfully!";
				return result;
			}
		}


		public async Task<ResponseObject<RecipeResponse>> ChangeBaseStatus(Guid id , ChangeRecipeBaseStatusRequest recipe)
		{
			var result = new ResponseObject<RecipeResponse>();
			//var validateResult = _recipeChangeStatusValidator.Validate(recipe);
			//if ( !validateResult.IsValid )
			//{
			//	var error = validateResult.Errors.Select(e => e.ErrorMessage).ToList();
			//	result.StatusCode = 400;
			//	result.Message = string.Join(" - " , error);
			//	return result;
			//}
			var recipeExist = await _unitOfWork.RecipeRepository.GetByIdAsync(id.ToString());
			if ( recipeExist == null )
			{
				result.StatusCode = 404;
				result.Message = "Not found recipe id " + id + "!";
				return result;
			}
			_mapper.Map(recipe , recipeExist);
			var changeResult = await _unitOfWork.RecipeRepository.UpdateAsync(recipeExist);
			if ( changeResult )
			{
				//Nếu thay đổi status sang unavailable thì xóa recipe khỏi wpl
				if ( recipeExist.BaseStatus == BaseStatus.UnAvailable && recipeExist.RecipePlans != null )
				{
					var deleteWp = await deleteRecipeFromWeeklyPlan(recipeExist.Id , recipeExist.ProcessStatus , recipe.BaseStatus);
					if ( !deleteWp )
					{
						result.StatusCode = 500;
						result.Message = "Change status unsuccess! Delete recipe from weekly plan!";
						return result;

					}
				}
				await _unitOfWork.CompleteAsync();
				result.StatusCode = 200;
				result.Message = "Change status success";
				return result;

			}
			else
			{
				result.StatusCode = 500;
				result.Message = "Change Recipe " + id + " status Unsuccessfully!";
				return result;
			}
		}


		#endregion

		#region Delete
		public async Task<ResponseObject<RecipeResponse>> DeleteRecipeById(Guid userId , Guid request)
		{
			var result = new ResponseObject<RecipeResponse>();
			var recipeExist = await _unitOfWork.RecipeRepository.GetByIdAsync(request.ToString());
			if ( recipeExist != null )
			{
				var userExist = await _unitOfWork.UserRepository.GetByIdAsync(userId.ToString());
				if ( userExist != null && userExist.Role == Role.Admin )
				{
					//delete
					var deleteResult = await _unitOfWork.RecipeRepository.DeleteAsync(recipeExist.Id.ToString());
					if ( deleteResult )
					{
						await _unitOfWork.CompleteAsync();
						result.StatusCode = 200;
						result.Message = "Delete recipe success";
						return result;
					}
					result.StatusCode = 500;
					result.Message = "Delete recipe unsuccess!";
					return result;
				}
				if ( userExist != null && userExist.Id.ToString() == recipeExist.CreatedBy )
				{
					//change status
					recipeExist.ProcessStatus = ProcessStatus.Cancel;
					var updateResult = await _unitOfWork.RecipeRepository.UpdateAsync(recipeExist);
					if ( updateResult )
					{
						await _unitOfWork.CompleteAsync();
						var deleteRecipeFromWPL = deleteRecipeFromWeeklyPlan(request , recipeExist.ProcessStatus , null);
						if ( deleteRecipeFromWeeklyPlan != null )
						{
							await _unitOfWork.CompleteAsync();
							result.StatusCode = 200;
							result.Message = "Just change recipe status into denied success";
							return result;
						}
						result.StatusCode = 500;
						result.Message = "Delete recipe from wpl unsuccess!";
						return result;
					}
					result.StatusCode = 500;
					result.Message = "Just change recipe status unsuccess!";
					return result;
				}
				//don't change anything
				result.StatusCode = 402;
				result.Message = "UnAuthorizator to delete!";
				return result;
			}
			result.StatusCode = 404;
			result.Message = "Not found recipe!";
			return result;
		}
		#endregion

		#region Search recipe list with recipe category id
		public async Task<ResponseObject<List<RecipeResponse>>> GetListByCategoryId(Guid categoryId)
		{
			/*
			 1-lay list recipe category lien quan
				1.1 - xac dinh category co ton tai (category repo - get by id)
				1.2 - tim list recipe category lien quan
					+ - tao list trong
					+ - cho chay vong for gap dung thi add recipe id
			 2-tim list recipe lien quan
				2.1 - tao list trong
				2.2 - cho chay vong for gap dung thi add
			 3- tra ve 200 va list recipe
			 */
			var result = new ResponseObject<List<RecipeResponse>>();
			var checkCategory = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryId.ToString());
			if ( checkCategory == null )
			{
				result.StatusCode = 400;
				result.Message = "Wrong category. Not found Category";
				return result;
			}
			var recipeIdListfound = _recipeCategoryService.GetRecipeIdByCategoryId(categoryId);
			if ( recipeIdListfound == null )//cai nay la coi nhu tim ko co mon an thich hop, chu ko phai loi
			{
				result.StatusCode = 200;
				result.Message = "Not found suitable recipe";
				result.Data = new List<RecipeResponse>();
				return result;
			}
			List<Recipe> listRecipe = new List<Recipe>();
			foreach ( var item in recipeIdListfound )
			{
				var recipe = await _unitOfWork.RecipeRepository.GetByIdAsync(item.ToString());
				if ( recipe != null )
					listRecipe.Add(recipe);
			}
			result.StatusCode = 200;
			result.Message = "Recipe list base on categoryID: ";
			result.Data = _mapper.Map<List<RecipeResponse>>(listRecipe);
			return result;
		}
		#endregion

		#region auto update
		//update price
		private async Task<bool> UpdatePrice(Guid recipeId)
		{
			var recipeExist = _unitOfWork.RecipeRepository.Get(r => r.Id == recipeId).FirstOrDefault();
			if ( recipeExist != null )
			{
				var recipeIngredients = _unitOfWork.RecipeIngredientRepository.Get(ri => ri.RecipeId == recipeExist.Id).ToList();
				var tmpPrice = 0.0;
				if ( recipeIngredients.Any() )
				{
					foreach ( var recipeIngredient in recipeIngredients )
					{
						tmpPrice += recipeIngredient.Ingredient.Price * recipeIngredient.Amount;
					}
				}
				recipeExist.Price = tmpPrice;
				await _unitOfWork.CompleteAsync();
				return true;
			}
			return false;
		}

		public async Task<bool> AutoUpdateRecipeAsync(Guid? recipeId)
		{
			//find recipe
			var recipeExist = await _unitOfWork.RecipeRepository.GetByIdAsync(recipeId.ToString());
			if ( recipeExist != null && recipeExist.RecipeNutrient != null )
			{
				//delete nutrient cũ
				var deleteRecipeNutrient = await _unitOfWork.RecipeNutrientRepository.DeleteAsync(recipeExist.RecipeNutrient.Id.ToString());
				if ( !deleteRecipeNutrient )
				{
					return false;
				}
			}
			else if ( recipeExist == null )
			{
				return false;
			}
			//create recipe nutrient and update price
			var updatePrice = await UpdatePrice(recipeExist.Id);
			if ( !updatePrice )
			{
				return false;
			}
			var createRecipeNutrient = await _recipeNutrientService.CreateRecipeNutrientAsync(recipeExist.Id);
			if ( createRecipeNutrient != null )
			{
				//thành công
				await _unitOfWork.CompleteAsync();
				return true;
			}
			return false;
		}

		public async Task<ResponseObject<List<RecipeNutrientResponse>>> UpdateRecipeByIngredient(Guid ingredientId)
		{
			var result = new ResponseObject<List<RecipeNutrientResponse>>();
			//ingredient exist
			var ingredientExist = await _unitOfWork.IngredientRepository.GetByIdAsync(ingredientId.ToString());
			if ( ingredientExist == null )
			{
				result.StatusCode = 404;
				result.Message = "Ingredient not exist!";
				return result;
			}
			else if ( ingredientExist.Status == BaseStatus.UnAvailable )
			{
				result.StatusCode = 400;
				result.Message = "Ingredient UnAvailable!";
				return result;
			}
			//find recipe by recipeIngredient
			var recipeIngredients = _unitOfWork.RecipeIngredientRepository.Get(ri => ri.IngredientId == ingredientId).ToList();
			if ( recipeIngredients != null )
			{
				foreach ( var recipeIngredient in recipeIngredients )
				{
					//find recipe
					var recipeExist = await _unitOfWork.RecipeRepository.GetByIdAsync(recipeIngredient.RecipeId.ToString());
					if ( recipeExist != null && recipeExist.RecipeNutrient != null )
					{
						//delete nutrient cũ
						var deleteRecipeNutrient = await _unitOfWork.RecipeNutrientRepository.DeleteAsync(recipeExist.RecipeNutrient.Id.ToString());
						if ( !deleteRecipeNutrient )
						{
							result.StatusCode = 500;
							result.Message = "delete old recipe nutrient unsuccessfully!";
							return result;
						}
					}
					else if ( recipeExist == null )
					{
						result.StatusCode = 404;
						result.Message = "Recipe not exist!";
						return result;
					}
					//create recipe nutrient and update price
					var updatePrice = await UpdatePrice(recipeExist.Id);
					if ( !updatePrice )
					{
						result.StatusCode = 500;
						result.Message = "Update recipe price unsuccessfully";
						return result;
					}
					var createRecipeNutrient = await _recipeNutrientService.CreateRecipeNutrientAsync(recipeExist.Id);
					if ( createRecipeNutrient != null )
					{
						//thành công
						result.StatusCode = 200;
						result.Message = "update recipe nutrient successfully";
						return result;
					}
				}
			}
			result.StatusCode = 404;
			result.Message = "Don't have recipe ingredient list!";
			return result;
		}


		public async Task<ResponseObject<RecipeResponse>> UpdateRecipeAsync(string updatedBy , Guid idRecipe , CreateRecipeRequest recipe)
		{
			var result = new ResponseObject<RecipeResponse>();
			var currentList = await GetAllToProcess();
			try
			{
				//check recipe exsit
				var recipeExist = await _unitOfWork.RecipeRepository.GetByIdAsync(idRecipe.ToString());
				if ( recipeExist == null )
				{
					result.StatusCode = 404;
					result.Message = "Recipe not exist!";
					return result;
				}
				//mapper
				_mapper.Map(recipe , recipeExist);
				recipeExist.UpdatedAt = DateTime.Now;
				recipeExist.ProcessStatus = ProcessStatus.Processing;
				recipeExist.UpdatedBy = updatedBy;
				if ( recipeExist.ProcessStatus == ProcessStatus.Processing
				|| recipeExist.ProcessStatus == ProcessStatus.Denied
				|| recipeExist.ProcessStatus == ProcessStatus.Cancel )
				{
					recipeExist.BaseStatus = BaseStatus.UnAvailable;
				}
				//tao gia co ban cho recipe dua vao gia don vi cua nguyen lieu
				var updatePrice = await UpdatePrice(recipeExist.Id);
				if ( !updatePrice )
				{
					result.StatusCode = 500;
					result.Message = "Update recipe price unsuccessfully";
					return result;
				}

				var updateResult = await _unitOfWork.RecipeRepository.UpdateAsync(recipeExist);
				if ( !updateResult )
				{
					result.StatusCode = 500;
					result.Message = "Update Recipe unsuccessfully!";
					return result;
				}
				await _unitOfWork.CompleteAsync();
				//xóa các thành phần liên quan (RecipeCategory,RecipeIngredient)
				var checkDeleteRecipeCategory = await _recipeCategoryService.DeleteByRcipe(recipeExist.Id);
				var checkDeleteRecipeIngredient = await _recipeIngredientService.DeleteRecipeIngredientByRecipeAsync(recipeExist.Id);
				if (
					checkDeleteRecipeCategory.StatusCode != 200
					|| checkDeleteRecipeIngredient.StatusCode != 200
					)
				{
					result.StatusCode = 500;
					result.Message = checkDeleteRecipeCategory.Message
						+ " | " + checkDeleteRecipeIngredient.Message;
					return result;
				}

				//bat dau update cac thanh phan lien quan

				//create RecipeCategory, RecipeIngredient
				if ( recipe.CategoryIds != null && recipe.RecipeIngredientsList != null )
				{
					var checkCreateRecipeCategory = await _recipeCategoryService.Create(recipeExist.Id , recipe.CategoryIds);
					var checkCreateRecipeIngredient = await _recipeIngredientService.CreateRecipeIngredientAsync(recipeExist.Id , recipe.RecipeIngredientsList);

					//update RecipeNutrient có thể gọi hàm tự động update vô đây
					var updateNutrientResult = await _recipeNutrientService.AutoUpdateNutrientByRecipe(recipeExist.Id);
					if ( updateNutrientResult == false )
					{
						result.StatusCode = 500;
						result.Message = "Faild to update nutrient!";
						return result;
					}
					if (
						checkCreateRecipeCategory.StatusCode != 200 || checkCreateRecipeCategory.Data == null
						|| checkCreateRecipeIngredient.StatusCode != 200 || checkCreateRecipeIngredient.Data == null
						)
					{
						result.StatusCode = 500;
						result.Message = checkCreateRecipeCategory.Message
							+ " | " + checkCreateRecipeIngredient.Message;
						return result;
					}
				}

				//kiểm tra xem step nào đã tồn tại, nếu có rồi thì update step, nếu chưa thì delete
				var recipeSteps = _unitOfWork.RecipeStepRepository.GetAll()
					.Where(rc => rc.RecipeId == recipeExist.Id).ToList();
				//nếu recipe.Steps count > recipeSteps thì tạo mới recipestep
				if ( recipe.Steps.Count > recipeSteps.Count )
				{
					//xóa tất cả step có liên quan
					var checkDeleteStep = await _recipeStepService.DeleteRecipeStepsByRecipe(recipeExist.Id);
					if ( checkDeleteStep.StatusCode != 200 )
					{
						result.StatusCode = 500;
						result.Message = checkDeleteStep.Message;
						return result;
					}
					//gọi tới create
					var checkCreateRecipeStep = await _recipeStepService.CreateRecipeSteps(recipeExist.Id , recipe.Steps);
					if ( checkCreateRecipeStep.StatusCode != 200 )
					{
						//tạo không được thì return về lại step cũ
						result.StatusCode = 500;
						result.Message = checkCreateRecipeStep.Message;
						return result;
					}
				}
				else
				{
					foreach ( var recipeStepByRecipe in recipe.Steps )
					{
						//nếu có rồi thì update 
						foreach ( var step in recipeSteps )
						{
							if ( recipeStepByRecipe.Id == step.Id )
							{
								//update
								var checkUpdateStep = await _recipeStepService.UpdateRecipeStepsByRecipe(step.Id , recipeStepByRecipe);
								if ( checkUpdateStep.StatusCode != 200 )
								{
									result.StatusCode = 500;
									result.Message = checkUpdateStep.Message;
									return result;
								}
								break;
							}
							else if ( recipe.Steps.Count < recipeSteps.Count )
							{
								//delete
								var checkDeleteStep = await _recipeStepService.DeleteAsync(step.Id);
								if ( checkDeleteStep.StatusCode != 200 )
								{
									result.StatusCode = 500;
									result.Message = checkDeleteStep.Message;
									return result;
								}
							}
						}
					}
				}

				await _unitOfWork.CompleteAsync();
				result.StatusCode = 200;
				result.Message = "Update Recipe successfully.";
				return result;
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
	public static class StringExtensions
	{
		public static string RemoveDiacritics(this string text)
		{
			string normalizedString = text.Normalize(NormalizationForm.FormD);
			StringBuilder stringBuilder = new StringBuilder();

			foreach ( char c in normalizedString )
			{
				if ( CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark )
				{
					stringBuilder.Append(c);
				}
			}
			var result = stringBuilder.ToString().Normalize(NormalizationForm.FormC).Trim();
			// Loại bỏ dấu câu: , . / ?
			result = Regex.Replace(result , @"[,.\/?@]" , "");
			// Loại bỏ tất cả khoảng trắng
			return result = Regex.Replace(result , @"\s+" , "");
		}
	}
}
