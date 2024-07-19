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
            return Ok(result);
        }

        [HttpGet("get-by-ingredient-id")]
        public async Task<IActionResult> GetByIngredientId([FromQuery] string ingredientId)
        {
            Guid convertId;
            if (Guid.TryParse(ingredientId, out convertId))
            {
                var result = await _ingredientNutrientService.GetByIngredientId(convertId);
                return Ok(result);
            }
            else
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Invalid GUID format! Please provide a valid GUID!"
                });
            }
        }

        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetById([FromQuery] string id)
        {
            Guid convertId;
            if (Guid.TryParse(id, out convertId))
            {
                var result = await _ingredientNutrientService.GetById(convertId);
                return Ok(result);
            }
            else
            {
                return BadRequest(new
                {
                    StatusCode = 400,
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

        [HttpPut("update-ingredient-nutrient")]
        public async Task<IActionResult> Update([FromBody] FullIngredientNutrientRequest request)
        {
            Guid convertId;
            Guid convertIngredientId;
            if (Guid.TryParse(request.Id.ToString(), out convertId) && Guid.TryParse(request.IngredientID.ToString(), out convertIngredientId))
            {
                var result = await _ingredientNutrientService.Update(request);
                return Ok(result);
            }
            else
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Invalid ingredient GUID format! Please provide valid GUID!"
                });
            }
        }

        [HttpDelete("delete-with-id")]
        public async Task<IActionResult> Delete([FromQuery] string id)
        {
            Guid convertId;
            if (Guid.TryParse(id, out convertId))
            {
                var result = await _ingredientNutrientService.DeleteById(convertId);
                return Ok(result);
            }
            else
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Invalid GUID format! Please provide a valid GUID!"
                });
            }
        }
    }
}
