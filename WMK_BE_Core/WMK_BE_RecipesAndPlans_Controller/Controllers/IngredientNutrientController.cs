using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientNutrientModel;
using WMK_BE_BusinessLogic.Service.Implement;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_RecipesAndPlans_Controller.Controllers
{
	[Route("api/ingredientnutrients")]
	[ApiController]
	public class IngredientNutrientController : ControllerBase
	{
		private readonly IIngredientNutrientService _ingredientNutrientService;
		public IngredientNutrientController(IIngredientNutrientService ingredientNutrientService)
		{
			_ingredientNutrientService = ingredientNutrientService;
		}

		[HttpGet("get-all")]
		public async Task<IActionResult> GetAll()
		{
			var result = await _ingredientNutrientService.GetAll();
			return StatusCode(result.StatusCode , result);
		}

		[HttpGet("get-by-ingredient-id/{ingredientId}")]
		public async Task<IActionResult> GetByIngredientId(string ingredientId)
		{
			Guid convertId;
			if ( Guid.TryParse(ingredientId , out convertId) )
			{
				var result = await _ingredientNutrientService.GetByIngredientId(convertId);
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

		[HttpGet("get-by-id/{id}")]
		public async Task<IActionResult> GetById(string id)
		{
			Guid convertId;
			if ( Guid.TryParse(id , out convertId) )
			{
				var result = await _ingredientNutrientService.GetById(convertId);
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

		//[HttpPost("create-witn-ingredient-id")]
		//public async Task<IActionResult> Create([FromBody] CreateIngredientNutrientRequest request)
		//{
		//    //var result = _ingredientNutrientService.Create(request);
		//    //return Ok(result);
		//    Guid convertId;
		//    if (Guid.TryParse(request.IngredientID.ToString(), out convertId))
		//    {
		//        var result = await _ingredientNutrientService.Create(request);
		//        return Ok(result);
		//    }
		//    else
		//    {
		//        return BadRequest(new
		//        {
		//            StatusCode = 400,
		//            Message = "Invalid ingredient GUID format! Please provide a valid GUID!"
		//        });
		//    }
		//}

		[HttpPut("update-ingredient-nutrient/{id}")]
		[Authorize]
		public async Task<IActionResult> Update(Guid id , [FromBody] IngredientNutrientRequest request)
		{
			Guid convertId;
			//Guid convertIngredientId;
			if ( Guid.TryParse(id.ToString() , out convertId))
			{
				var result = await _ingredientNutrientService.Update(id , request);
				return StatusCode(result.StatusCode , result);
			}
			else
			{
				return BadRequest(new
				{
					StatusCode = 400 ,
					Message = "Invalid ingredient GUID format! Please provide valid GUID!"
				});
			}
		}

		[HttpDelete("delete-with-id/{id}")]
		[Authorize]
		public async Task<IActionResult> Delete(string id)
		{
			Guid convertId;
			if ( Guid.TryParse(id , out convertId) )
			{
				var result = await _ingredientNutrientService.DeleteById(convertId);
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
