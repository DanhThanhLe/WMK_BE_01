using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Security.Claims;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.IngredientModel;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Implement;
using WMK_BE_BusinessLogic.Service.Interface;

namespace WMK_BE_RecipesAndPlans_Controller.Controllers
{
	[Route("api/ingredients")]
	[ApiController]
	public class IngredientController : ControllerBase
	{
		private readonly IIngredientService _ingredientService;
		public IngredientController(IIngredientService ingredientService)
		{
			_ingredientService = ingredientService;
		}

		#region Get
		[HttpGet("get-all")]
		public async Task<IActionResult> GetAll([FromQuery] GetAllIngredientsRequest? model)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var result = await _ingredientService.GetAllAync(userId , model);
			return StatusCode(result.StatusCode , result);
		}

		[HttpGet("get/{id}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var result = await _ingredientService.GetIngredientById(id);
			return StatusCode(result.StatusCode , result);
		}

		#endregion

		[HttpPost("create-new")]
		[Authorize]
		public async Task<IActionResult> CreateNew([FromBody] CreateIngredientRequest model)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if ( userId == null )
			{
				return Unauthorized(new { Message = "Invalid token, user ID not found" });
			}
			string createdBy = userId.ToString();
			var result = await _ingredientService.CreateIngredient(createdBy , model);
			return StatusCode(result.StatusCode , result);
		}
		#region Update
		[HttpPut("update/{id}")]
		[Authorize]
		public async Task<IActionResult> Update(Guid id , CreateIngredientRequest model)
		{
			var result = await _ingredientService.UpdateIngredient(id , model);
			return StatusCode(result.StatusCode , result);
		}

		[HttpPut("update-status/{id}")]
		[Authorize]
		public async Task<IActionResult> UpdateStatus(Guid id , [FromBody] UpdateStatusIngredientRequest model)
		{
			var result = await _ingredientService.ChangeStatus(id , model);
			return StatusCode(result.StatusCode , result);
		}
		#endregion

		[HttpDelete("delete/{id}")]
		[Authorize]
		public async Task<IActionResult> DeleteById(string id)
		{
			Guid ingredientId;
			if ( Guid.TryParse(id , out ingredientId) )
			{
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				if ( userId == null )
				{
					return Unauthorized(new { Message = "Invalid token, user ID not found" });
				}
				var result = await _ingredientService.DeleteIngredientById(ingredientId , userId.ToString());
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
