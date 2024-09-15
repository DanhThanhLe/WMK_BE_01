using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.CategoryModel;
using WMK_BE_BusinessLogic.Service.Interface;

namespace WMK_BE_RecipesAndPlans_Controller.Controllers
{
	[Route("api/categories")]
	[ApiController]
	public class CategoryController : ControllerBase
	{
		private readonly ICategoryService _categoryService;

		public CategoryController(ICategoryService categoryService)
		{
			_categoryService = categoryService;
		}
		#region Get
		//
		[HttpGet("get-all")]
		public async Task<IActionResult> Get([FromQuery] GetAllCategoriesRequest? model)
		{
			var result = await _categoryService.GetAllAsync(model);
			return StatusCode(result.StatusCode , result);
		}

		[HttpGet("get/{id}")]
		[Authorize]
		public async Task<IActionResult> GetById(string id)
		{
			if ( Guid.TryParse(id , out var categoryId) )
			{
				var result = await _categoryService.GetByIdAsync(categoryId);
				return StatusCode(result.StatusCode , result);
			}
			else
			{
				return BadRequest("Wrong Id fortmat!");
			}
		}
		#endregion
		
		[HttpPost("create")]
		[Authorize(Roles = "Admin,Manager")]
		public async Task<IActionResult> Create([FromBody] CreateCategoryRequest model)
		{
			var result = await _categoryService.CreateCategoryAsync(model);
			return StatusCode(result.StatusCode , result);
		}
		
		#region Update
		//
		[HttpPut("update/{id}")]
		[Authorize(Roles = "Admin,Manager")]
		public async Task<IActionResult> Update(Guid id , [FromBody] UpdateCategoryRequest model)
		{
			var result = await _categoryService.UpdateCategoryAsync(id , model);
			return StatusCode(result.StatusCode , result);
		}
		//
		[HttpPut("change-status/{id}")]
		[Authorize(Roles = "Admin,Manager")]
		public async Task<IActionResult> ChangeStatus(Guid id , ChangeCategoryRequest model)
		{
			var result = await _categoryService.ChangeCategoryStatusAsync(id , model);
			return StatusCode(result.StatusCode , result);
		}
		#endregion

		[HttpDelete("delete/{id}")]
		[Authorize(Roles = "Admin,Manager")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var result = await _categoryService.DeleteCategoryAsync(id);
			return StatusCode(result.StatusCode , result);
		}
	}
}
