﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientCategoryModel;
using WMK_BE_BusinessLogic.Service.Implement;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_RecipesAndPlans_Controller.Controllers
{
	[Route("api/ingredientcategories")]
	[ApiController]
	public class IngredientCategoryController : ControllerBase
	{
		private readonly IIngredientCategoryService _ingredientCategoryService;
		public IngredientCategoryController(IIngredientCategoryService ingredientCategoryService)
		{
			_ingredientCategoryService = ingredientCategoryService;
		}
		
		[HttpGet("get-all")]
		[Authorize]
		public async Task<IActionResult> GetAll([FromQuery] GetAllIngredientCategoriesRequest? model)
		{
			var result = await _ingredientCategoryService.GetAllAsync(model);
			return StatusCode(result.StatusCode , result);
		}

		[HttpPost("create-new")]
		[Authorize(Roles = "Admin,Manager,Staff")]
		public async Task<IActionResult> Create([FromBody] CreateIngredientCategoryRequest request)
		{
			var result = await _ingredientCategoryService.CreateNew(request);
			return StatusCode(result.StatusCode , result);
		}
		
		[HttpPut("update/{id}")]
		[Authorize(Roles = "Admin,Manager,Staff")]
		public async Task<IActionResult> Update(Guid id , [FromBody] FullIngredientCategoryRequest request)
		{
			var result = await _ingredientCategoryService.UpdateCategory(id , request);
			return StatusCode(result.StatusCode , result);
		}
		
		[HttpPut("change-status/{id}")]
		[Authorize(Roles = "Admin,Manager,Staff")]
		public async Task<IActionResult> ChangeStatus(Guid id , ChangeStatusIngredientCategoryRequest request)
		{
			var result = await _ingredientCategoryService.ChangeStatusIngredientCategoryAsync(id , request);
			return StatusCode(result.StatusCode , result);
		}

		[HttpDelete("delete-by-id/{id}")]
		[Authorize(Roles = "Admin,Manager,Staff")]
		public async Task<IActionResult> DeleteById(string id)
		{
			Guid convertID;
			if ( Guid.TryParse(id , out convertID) )
			{
				var result = await _ingredientCategoryService.DeleteById(convertID);
				return StatusCode(result.StatusCode , result);
			}
			else
			{
				return BadRequest(new
				{
					StatusCode = 400 ,
					Message = "Sai ID format"
				});
			}
		}
	}
}
