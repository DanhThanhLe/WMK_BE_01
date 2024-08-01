using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.Recipe;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.RecipeModel;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.RecipeStepModel.RecipeStep;
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
		public RecipeService(IUnitOfWork unitOfWork , IMapper mapper , IRecipeIngredientService recipeAmountService , IRecipeCategoryService recipeCategoryService , IRecipeNutrientService recipeNutrientService , IRecipeIngredientService recipeIngredientService , IRecipeStepService recipeStepService)
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
		public async Task<ResponseObject<List<RecipeResponse>>> GetRecipes()
		{
			var result = new ResponseObject<List<RecipeResponse>>();
			var currentList = await GetAllToProcess();
			var responseList = currentList.Where(x => x.ProcessStatus == ProcessStatus.Approved);
			if ( currentList != null && currentList.Count() > 0 )
			{
				result.StatusCode = 200;
				result.Message = "OK. Recipe list " + currentList.Count();
				var mapUser = _mapper.Map<List<RecipeResponse>>(currentList);
				foreach ( var item in mapUser )
				{
					var userCreate = await _unitOfWork.UserRepository.GetByIdAsync(item.CreatedBy);
					var userUpdate = await _unitOfWork.UserRepository.GetByIdAsync(item.UpdatedBy);
					var userAprove = await _unitOfWork.UserRepository.GetByIdAsync(item.ApprovedBy);
					if ( userCreate != null )
					{
						item.CreatedBy = userCreate.FirstName + " " + userCreate.LastName;
					}
					if ( userUpdate != null )
					{
						item.UpdatedBy = userUpdate.FirstName + " " + userUpdate.LastName;
					}
					if ( userAprove != null )
					{
						item.ApprovedBy = userAprove.FirstName + " " + userAprove.LastName;
					}
				}
				result.Data = mapUser;
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
				result.StatusCode = 200;
				result.Message = "Recipe with Id " + id + ":";
				RecipeResponse response = _mapper.Map<RecipeResponse>(recipe);
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
		public async Task<ResponseObject<List<RecipeResponse>>> GetRecipeByName(string name = "" , bool status = true)//tim bang ten cua recipe lan cua ca category
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


				if ( foundList == null )
				{
					result.StatusCode = 404;
					result.Message = "Not found. No such recipe in collection contain keyword: " + name;
					return result;
				}
				else
				{
					//var returnList = foundList.Where(x => x.ProcessStatus == ProcessStatus.Approved).ToList();
					result.StatusCode = 200;
					result.Message = "Recipe list found by name";
					result.Data = _mapper.Map<List<RecipeResponse>>(foundList);
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
		public async Task<ResponseObject<RecipeResponse>> CreateRecipeAsync(CreateRecipeRequest recipe)
		{
			var result = new ResponseObject<RecipeResponse>();
			var currentList = await GetAllToProcess();
			try
			{
				var validateResult = _validator.Validate(recipe);
				if ( !validateResult.IsValid )
				{
					var error = validateResult.Errors.Select(e => e.ErrorMessage).ToList();
					result.StatusCode = 500;
					result.Message = "Say from CreateRecipeAsync - RecipeService./n" + string.Join(" - /n" , error);
					return result;
				}
				//mapper
				Recipe newRecipe = _mapper.Map<Recipe>(recipe);
				newRecipe.Popularity = 0;
				newRecipe.CreatedAt = DateTime.Now;
				newRecipe.ProcessStatus = ProcessStatus.Processing;
				var checkDuplicateName = currentList.FirstOrDefault(x => x.Name.ToLower().Equals(recipe.Name.ToLower()));
				if ( checkDuplicateName != null )
				{
					result.StatusCode = 400;
					result.Message = "Duplicate name with ID " + checkDuplicateName.Id.ToString();
					return result;
				}
				//tao gia co ban cho recipe dua vao gia don vi cua nguyen lieu
				foreach ( var item in recipe.RecipeIngredientsList )
				{
					var ingredientFound = await _unitOfWork.IngredientRepository.GetByIdAsync(item.IngredientId.ToString());
					if ( ingredientFound != null )
					{
						newRecipe.Price += ingredientFound.Price * item.amount;
					}
					else
					{
						result.StatusCode = 400;
						result.Message = "Ingredient ID " + item.IngredientId + " not found.";
						return result;
					}
				}

				var createResult = await _unitOfWork.RecipeRepository.CreateAsync(newRecipe);
				if ( !createResult )
				{
					result.StatusCode = 500;
					result.Message = "Create Recipe unsuccessfully! Say from CreateRecipeAsync - RecipeService.";
					return result;
				}
				await _unitOfWork.CompleteAsync();

				//bat dau tao cac thanh phan lien quan

				//create RecipeCategory
				var checkCreateRecipeCategory = await _recipeCategoryService.Create(newRecipe.Id , recipe.CategoryIds);

				//create RecipeIngredient
				var checkCreateRecipeIngredient = await _recipeIngredientService.CreateRecipeIngredientAsync(newRecipe.Id , recipe.RecipeIngredientsList);

				//create RecipeStep
				var checkCreateRecipeStep = await _recipeStepService.CreateRecipeSteps(newRecipe.Id , recipe.Steps);

				//create RecipeNutrient
				var checkCreateRecipeNutrient = await _recipeNutrientService.Create(newRecipe.Id , recipe.RecipeIngredientsList);

				if (//1 trong 3 cai ko tao dc thi xoa thong tin hien hanh cua recipe moi dang tao
					checkCreateRecipeCategory.StatusCode != 200 || checkCreateRecipeCategory.Data == null
					|| checkCreateRecipeIngredient.StatusCode != 200 || checkCreateRecipeIngredient.Data == null
					|| checkCreateRecipeStep.StatusCode != 200 || checkCreateRecipeStep.Data == null
					|| checkCreateRecipeNutrient.StatusCode != 200 || checkCreateRecipeNutrient.Data == null
					)
				{
					resetRecipe(newRecipe.Id);
					result.StatusCode = 500;
					result.Message = checkCreateRecipeCategory.Message
						+ " | " + checkCreateRecipeIngredient.Message
						+ " | " + checkCreateRecipeIngredient.Message
						+ " | " + checkCreateRecipeNutrient.Message;
					return result;
				}
				else//ko co loi va hoan thanh tao moi
				{
					await _unitOfWork.CompleteAsync();
					result.StatusCode = 200;
					result.Message = "Create Recipe successfully.";
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
		private async void resetRecipe(Guid recipeId)
		{
			await _unitOfWork.RecipeRepository.DeleteAsync(recipeId.ToString());
			await _unitOfWork.CompleteAsync();
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
			var found = await _unitOfWork.RecipeRepository.GetByIdAsync(id.ToString());
			if ( found == null )
			{
				result.StatusCode = 404;
				result.Message = "Not found recipe id " + id + "!";
				return result;
			}
			found.ProcessStatus = recipe.ProcessStatus;
			if ( recipe.Notice.IsNullOrEmpty() || recipe.Notice.Equals("string") )
			{
				found.Notice = "Not have";
			}
			found.Notice = recipe.Notice;
			var changeResult = await _unitOfWork.RecipeRepository.UpdateAsync(found);
			if ( changeResult )
			{
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
		public async Task<ResponseObject<RecipeResponse>> DeleteRecipeById(Guid request)
		{
			var result = new ResponseObject<RecipeResponse>();
			var found = await _unitOfWork.RecipeRepository.GetByIdAsync(request.ToString());
			if ( found == null )
			{
				result.StatusCode = 404;
				result.Message = "Not found recipe!";
				return result;
			}


			//check recipe exist in weekly plan - if have, just change status -> cancel
			var deleteResult = await _unitOfWork.RecipeRepository.DeleteAsync(request.ToString());
			if ( deleteResult )
			{
				await _unitOfWork.CompleteAsync();
				result.StatusCode = 200;
				result.Message = "Success";
				return result;
			}
			else
			{
				result.StatusCode = 500;
				result.Message = "Error at delete RECIPE";
				return result;
			}
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
		public async Task<ResponseObject<List<RecipeNutrientResponse>>> AutoUpdate(Guid IngredientId)
		{
			var result = new ResponseObject<List<RecipeNutrientResponse>>();
			//lay thong tin cua tat ca recipe
			//tu do lay thong tin cua recipeIngredient
			//lay thong tin cua ingredient
			//lay thong tin cua nutrient trong ingredient
			//tinh lai thong tin do
			//lenh update
			//luu thong tin
			var currentListRecipe = _unitOfWork.RecipeRepository.Get(x => x.ProcessStatus == ProcessStatus.Approved && x.RecipeIngredients.Any(x => x.IngredientId.ToString().ToLower().Equals(IngredientId.ToString().ToLower()))).ToList();
			if ( currentListRecipe.Any() )
			{
				foreach ( var item in currentListRecipe )
				{
					double updatePrice = 0;
					RecipeNutrient nutrientInfor = _unitOfWork.RecipeNutrientRepository.Get(x => x.RecipeID.ToString().ToLower().Equals(item.Id.ToString().ToLower())).FirstOrDefault();
					RecipeNutrient temp = new RecipeNutrient();
					temp.RecipeID = item.Id;
					if ( nutrientInfor != null && nutrientInfor.RecipeID.ToString() != null )
					{
						temp.Id = nutrientInfor.Id;
						foreach ( var ri in item.RecipeIngredients )
						{
							temp.Calories += ri.Ingredient.IngredientNutrient.Calories * ri.Amount;
							temp.Fat += ri.Ingredient.IngredientNutrient.Fat * ri.Amount;
							temp.SaturatedFat += ri.Ingredient.IngredientNutrient.SaturatedFat * ri.Amount;
							temp.Sugar += ri.Ingredient.IngredientNutrient.Sugar * ri.Amount;
							temp.Carbonhydrate += ri.Ingredient.IngredientNutrient.Carbonhydrate * ri.Amount;
							temp.DietaryFiber += ri.Ingredient.IngredientNutrient.DietaryFiber * ri.Amount;
							temp.Protein += ri.Ingredient.IngredientNutrient.Protein * ri.Amount;
							temp.Sodium += ri.Ingredient.IngredientNutrient.Sodium * ri.Amount;
							updatePrice += ri.Amount * ri.Ingredient.Price;
						}
						item.Price = updatePrice;
						_unitOfWork.RecipeRepository.DetachEntity(item);
						_unitOfWork.RecipeNutrientRepository.DetachEntity(nutrientInfor);
						var updateRecipePrice = await _unitOfWork.RecipeRepository.UpdateAsync(item);
						if ( updateRecipePrice )
						{
							await _unitOfWork.CompleteAsync();
						}
						else
						{
							result.Message = "Update price false";
							return result;
						}
						_unitOfWork.RecipeNutrientRepository.DetachEntity(temp);
						var updateResult = await _unitOfWork.RecipeNutrientRepository.UpdateAsync(temp);
						if ( updateResult )
						{
							await _unitOfWork.CompleteAsync();
						}
						else
						{
							result.Message = "Update nutrient false";
							return result;
						}
					}
					else
					{
						result.Message = "Not found nutreint infor";
						return result;
					}
				}
				result.StatusCode = 200;
				result.Message = "ok";
				return result;
			}
			else
			{
				result.Message = "Not found preference recipe. Its ok";
				return result;
			}
		}

		public async Task<ResponseObject<RecipeResponse>> UpdateRecipeAsync(Guid idRecipe , CreateRecipeRequest recipe)
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
				_mapper.Map(recipeExist , recipe);
				recipeExist.UpdatedAt = DateTime.Now;
				recipeExist.ProcessStatus = ProcessStatus.Processing;
				//tao gia co ban cho recipe dua vao gia don vi cua nguyen lieu
				if ( recipe.RecipeIngredientsList != null )
				{
					foreach ( var item in recipe.RecipeIngredientsList )
					{
						var ingredientFound = await _unitOfWork.IngredientRepository.GetByIdAsync(item.IngredientId.ToString());
						if ( ingredientFound != null )
						{
							recipeExist.Price += ingredientFound.Price * item.amount;
						}
						else
						{
							result.StatusCode = 400;
							result.Message = "Ingredient ID " + item.IngredientId + " not found.";
							return result;
						}
					}
				}

				var updateResult = await _unitOfWork.RecipeRepository.UpdateAsync(recipeExist);
				if ( !updateResult )
				{
					result.StatusCode = 500;
					result.Message = "Update Recipe unsuccessfully! Say from CreateRecipeAsync - RecipeService.";
					return result;
				}
				await _unitOfWork.CompleteAsync();
				//xóa các thành phần liên quan
				var checkDeleteRecipeCategory = await _recipeCategoryService.DeleteByRcipe(recipeExist.Id);
				var checkDeleteRecipeIngredient = await _recipeIngredientService.DeleteRecipeIngredientByRecipeAsync(recipeExist.Id);
				if (//1 trong 2 cai ko tao dc thi xoa thong tin hien hanh cua recipe moi dang tao
					checkDeleteRecipeCategory.StatusCode != 200
					|| checkDeleteRecipeIngredient.StatusCode != 200
					)
				{
					resetRecipe(recipeExist.Id);
					result.StatusCode = 500;
					result.Message = checkDeleteRecipeCategory.Message
						+ " | " + checkDeleteRecipeIngredient.Message;
					return result;
				}
				//bat dau update cac thanh phan lien quan

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
					var createStepModel = _mapper.Map<List<CreateRecipeStepRequest>>(recipeSteps);
					var checkCreateRecipeStep = await _recipeStepService.CreateRecipeSteps(recipeExist.Id , createStepModel);
					if ( checkCreateRecipeStep.StatusCode != 200 )
					{
						resetRecipe(recipeExist.Id);
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
									resetRecipe(recipeExist.Id);
									result.StatusCode = 500;
									result.Message = checkUpdateStep.Message;
									return result;
								}
							}
							else
							{
								//delete
								var checkDeleteStep = await _recipeStepService.DeleteAsync(step.Id);
								if ( checkDeleteStep.StatusCode != 200 )
								{
									resetRecipe(recipeExist.Id);
									result.StatusCode = 500;
									result.Message = checkDeleteStep.Message;
									return result;
								}
							}
						}
					}
				}

				//create RecipeCategory
				if ( recipe.CategoryIds != null && recipe.RecipeIngredientsList != null && recipe.Steps != null )
				{
					var checkCreateRecipeCategory = await _recipeCategoryService.Create(recipeExist.Id , recipe.CategoryIds);
					//create RecipeIngredient
					var checkCreateRecipeIngredient = await _recipeIngredientService.CreateRecipeIngredientAsync(recipeExist.Id , recipe.RecipeIngredientsList);

					//update RecipeNutrient có thể gọi hàm tự động update vô đây

					if (//1 trong 2 cai ko tao dc thi xoa thong tin hien hanh cua recipe moi dang tao
						checkCreateRecipeCategory.StatusCode != 200 || checkCreateRecipeCategory.Data == null
						|| checkCreateRecipeIngredient.StatusCode != 200 || checkCreateRecipeIngredient.Data == null
						)
					{
						resetRecipe(recipeExist.Id);
						result.StatusCode = 500;
						result.Message = checkCreateRecipeCategory.Message
							+ " | " + checkCreateRecipeIngredient.Message;
						return result;
					}
					else//ko co loi va hoan thanh update
					{
						await _unitOfWork.CompleteAsync();
						result.StatusCode = 200;
						result.Message = "Update Recipe successfully.";
						return result;
					}
				}
				result.StatusCode = 500;
				result.Message = "Invalid input! Category, ingredient, step is required!";
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

			return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
		}
	}
}
