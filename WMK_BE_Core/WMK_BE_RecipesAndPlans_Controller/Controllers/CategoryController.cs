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

		[HttpGet("get-all")]
		public async Task<IActionResult> Get()
		{
			var result = await _categoryService.GetAllAsync();
			return StatusCode(result.StatusCode , result);
		}

		[HttpGet("get/{id}")]
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

		[HttpGet("get-by-type/{type}")]
		public async Task<IActionResult> GetByType(string type)
		{
			if ( type != null )
			{
				var result = await _categoryService.GetCategoryByType(type);
				return StatusCode(result.StatusCode , result);
			}
			else
			{
				return BadRequest("Empty request");
			}
		}

		[HttpGet("get-by-name/{name}")]
		public async Task<IActionResult> GetByName(string name)
		{
			if ( name != null )
			{
				var result = await _categoryService.GetcategoryByName(name);
				return StatusCode(result.StatusCode , result);
			}
			else
			{
				return BadRequest("Empty request");
			}
		}

		[HttpPost("create")]
		[Authorize]
		public async Task<IActionResult> Create([FromBody] CreateCategoryRequestModel model)
		{
			var result = await _categoryService.CreateCategoryAsync(model);
			return StatusCode(result.StatusCode , result);
		}

		[HttpPut("update/{id}")]
		[Authorize]
		public async Task<IActionResult> Update(Guid id , [FromBody] UpdateCategoryRequestModel model)
		{
			var result = await _categoryService.UpdateCategoryAsync(id , model);
			return StatusCode(result.StatusCode , result);
		}

		[HttpPut("change-status/{id}")]
		[Authorize]
		public async Task<IActionResult> ChangeStatus(Guid id , ChangeCategoryRequest model)
		{
			var result = await _categoryService.ChangeCategoryAsync(id , model);
			return StatusCode(result.StatusCode , result);
		}

		[HttpDelete("delete/{id}")]
		[Authorize]
		public async Task<IActionResult> Delete(Guid id)
		{
			var result = await _categoryService.DeleteCategoryAsync(id);
			return StatusCode(result.StatusCode , result);
		}
	}
}
