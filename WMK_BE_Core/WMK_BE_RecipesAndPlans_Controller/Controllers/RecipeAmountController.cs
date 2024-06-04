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
    }
}
