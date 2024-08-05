using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.Recipe;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.RecipeModel;
using WMK_BE_BusinessLogic.Service.Interface;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.Recipe;

namespace WMK_BE_RecipesAndPlans_Controller.Controllers
{
	[Route("api/recipes")]
	[ApiController]
	public class RecipeController : ControllerBase
	{
		private readonly IRecipeService _recipeService;

		public RecipeController(IRecipeService recipeService)
		{
			_recipeService = recipeService;
		}
		#region Get
		[HttpGet("get-all")]
		public async Task<IActionResult> GetAll([FromQuery]GetAllRecipesRequest? model)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var result = await _recipeService.GetAllRecipesAsync(userId, model);
			return StatusCode(result.StatusCode , result);
		}

		[HttpGet("get-by-id/{id}")]
		public async Task<IActionResult> GetById(string id)
		{
			Guid recipeId;
			if ( Guid.TryParse(id , out recipeId) )
			{
				var result = await _recipeService.GetRecipeById(recipeId.ToString());
				return StatusCode(result.StatusCode , result);
			}
			else
			{
				return BadRequest(new
				{
					StatusCode = 400 ,
					Message = "Invalid GUID format! Please provide a valid GUID!"
				});
			}
		}

		//
		[HttpGet("get-by-name/{name?}/{status?}")]
		public async Task<IActionResult> GetByRecipeName(
		[SwaggerParameter("Name of the recipe" , Required = false)] string name = "" ,
		[SwaggerParameter("Status of the recipe" , Required = false)] bool status = true)
		{
			if ( name == "{name}" || name == null )
			{
				name = ""; // Đặt giá trị mặc định nếu không được cung cấp
			}
			var result = await _recipeService.GetRecipesByNameStatusAsync(name , status);
			return StatusCode(result.StatusCode , result);
		}
		#endregion

		#region Create
		//
		[HttpPost("create-new")]
		[Authorize]
		public async Task<IActionResult> Create([FromBody] CreateRecipeRequest model)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if ( userId == null )
			{
				return Unauthorized(new { Message = "Invalid token, user ID not found" });
			}
			string createdBy = userId.ToString();
			var result = await _recipeService.CreateRecipeAsync(createdBy , model);
			return StatusCode(result.StatusCode , result);
		}
		#endregion

		#region Update
		//
		[HttpPut("update/{id}")]
		[Authorize]
		public async Task<IActionResult> Update(Guid id , [FromBody] CreateRecipeRequest model)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if ( userId == null )
			{
				return Unauthorized(new { Message = "Invalid token, user ID not found" });
			}
			string updatedBy = userId.ToString();
			var result = await _recipeService.UpdateRecipeAsync(updatedBy, id , model);
			return StatusCode(result.StatusCode , result);
		}
		//
		[HttpPut("change-status/{id}")]
		[Authorize]
		public async Task<IActionResult> UpdateStatusRecipe(Guid id , [FromBody] ChangeRecipeStatusRequest request)
		{
			Guid recipeId;
			if ( Guid.TryParse(id.ToString() , out recipeId) )
			{
				var result = await _recipeService.ChangeStatus(id , request);
				return StatusCode(result.StatusCode , result);
			}
			else
			{
				return BadRequest(new
				{
					StatusCode = 400 ,
					Message = "Invalid GUID format! Please provide a valid GUID!"
				});
			}
		}

		[HttpPut("change-base-status/{id}")]
		[Authorize]
		public async Task<IActionResult> UpdateBaseStatusRecipe(Guid id , [FromBody] ChangeRecipeBaseStatusRequest request)
		{
			Guid recipeId;
			if ( Guid.TryParse(id.ToString() , out recipeId) )
			{
				var result = await _recipeService.ChangeBaseStatus(id , request);
				return StatusCode(result.StatusCode , result);
			}
			else
			{
				return BadRequest(new
				{
					StatusCode = 400 ,
					Message = "Invalid GUID format! Please provide a valid GUID!"
				});
			}
		}

		#endregion
		//
		[HttpDelete("delete/{id}")]
		[Authorize]
		public async Task<IActionResult> DeleteById(string id)
		{
			//get info user use
			var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if ( user == null )
			{
				return Unauthorized(new { Message = "Invalid token, user ID not found" });
			}
			Guid recipeId, userId;
			if ( Guid.TryParse(id , out recipeId) && Guid.TryParse(user, out userId))
			{
				var result = await _recipeService.DeleteRecipeById(userId ,recipeId);
				return StatusCode(result.StatusCode , result);
			}
			else
			{
				return BadRequest(new
				{
					StatusCode = 400 ,
					Message = "Invalid GUID format! Please provide a valid GUID!"
				});
			}
		}

		

	}
}
