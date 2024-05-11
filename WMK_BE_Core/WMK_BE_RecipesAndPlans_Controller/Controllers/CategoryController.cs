﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.Category;
using WMK_BE_BusinessLogic.Service.Interface;

namespace WMK_BE_RecipesAndPlans_Controller.Controllers
{
	[Route("api/category")]
	[ApiController]
	public class CategoryController : ControllerBase
	{
		private readonly ICategoryService _categoryService;

		public CategoryController(ICategoryService categoryService)
		{
			_categoryService = categoryService;
		}

		[HttpGet("get-all")]
		public async Task<IActionResult> Get()
		{
			var result = await _categoryService.GetAllAsync();
			return Ok(result);
		}

		[HttpGet("get")]
		public async Task<IActionResult> GetById(string id)
		{
			if ( Guid.TryParse(id , out var categoryId) )
			{
				var result = await _categoryService.GetByIdAsync(categoryId);
				return Ok(result);
			}
			else
			{
				return BadRequest("Wrong Id fortmat!");
			}
		}

		[HttpPost("create")]
		public async Task<IActionResult> Create([FromBody] CreateCategoryRequestModel model)
		{
			var result = await _categoryService.CreateCategoryAsync(model);
			return Ok(result);
		}
		[HttpPut("update")]
		public async Task<IActionResult> Update([FromBody] UpdateCategoryRequestModel model)
		{
			var result = await _categoryService.UpdateCategoryAsync(model);
			return Ok(result);
		}
		[HttpDelete("create")]
		public async Task<IActionResult> Delete([FromBody] DeleteCategoryRequestModel model)
		{
			var result = await _categoryService.DeleteCategoryAsync(model);
			return Ok(result);
		}
	}
}