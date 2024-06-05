using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WMK_BE_BusinessLogic.Service.Interface;

namespace WMK_BE_RecipesAndPlans_Controller.Controllers
{
    [Route("api/recipeamounts")]
    [ApiController]
    public class RecipeAmountController : ControllerBase
    {
        private readonly IRecipeAmountService _recipeAmountService;
        public RecipeAmountController(IRecipeAmountService recipeAmountService)
        {
            _recipeAmountService = recipeAmountService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _recipeAmountService.GetAll();
            return Ok(result);
        }

        [HttpGet("get-by-recipe-id")]
        public async Task<IActionResult> GetListByRecipeId([FromQuery] string recipeId)
        {
            Guid id;
            if(Guid.TryParse(recipeId, out id))
            {
                var result = await _recipeAmountService.GetListByRecipeId(id);
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
