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

		[HttpGet("get-all")]
		public async Task<IActionResult> GetAll([FromBody] string? name)
		{
			var result = new ResponseObject<List<IngredientResponse>>();
			if(name != null )
			{
				result = await _ingredientService.GetIngredientByName(name);
			}
			result = await _ingredientService.GetIngredients();
			return StatusCode(result.StatusCode , result);
		}

		[HttpGet("get/{id}")]
		public async Task<IActionResult> GetById(string id)
		{
			Guid ingredientId;
			if ( Guid.TryParse(id , out ingredientId) )
			{
				var result = await _ingredientService.GetIngredientById(ingredientId);
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

		[HttpGet("get-name/{name}")]
		public async Task<IActionResult> GetByName(string name="")
		{
			var result = await _ingredientService.GetIngredientByName(name);
			return StatusCode(result.StatusCode , result);
		}

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

		[HttpPut("update/{id}")]
		[Authorize]
		public async Task<IActionResult> Update(Guid id , CreateIngredientRequest model)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if ( userId == null )
			{
				return Unauthorized(new { Message = "Invalid token, user ID not found" });
			}
			string updatedBy = userId.ToString();
			var result = await _ingredientService.UpdateIngredient(updatedBy , id , model);
			return StatusCode(result.StatusCode , result);
		}

		[HttpPut("update-status/{id}")]
		[Authorize]
		public async Task<IActionResult> UpdateStatus(Guid id , [FromBody] UpdateStatusIngredientRequest model)
		{
			var result = await _ingredientService.ChangeStatus(id , model);
			return StatusCode(result.StatusCode , result);
		}

		//[HttpPut("remove-from-app")]
		//public async Task<IActionResult> RemoveById(string id)//dung de chuyen status thanh Unavailable
		//{
		//    Guid ingredientId;
		//    if (Guid.TryParse(id, out ingredientId))
		//    {
		//        var result = await _ingredientService.RemoveIngredientById(ingredientId);
		//        return Ok(result);
		//    }
		//    else
		//    {
		//        return BadRequest(new
		//        {
		//            StatusCode = 400,
		//            Message = "Invalid GUID format! Please provide a valid GUID!"
		//        });
		//    }
		//}

		[HttpDelete("delete/{id}")]
		[Authorize]
		public async Task<IActionResult> DeleteById(string id)
		{
			Guid ingredientId;
			if ( Guid.TryParse(id , out ingredientId) )
			{
				var result = await _ingredientService.DeleteIngredientById(ingredientId);
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
