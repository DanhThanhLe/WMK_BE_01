using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WMK_BE_BusinessLogic.Service.Interface;

namespace WMK_BE_RecipesAndPlans_Controller.Controllers
{
    [Route("api/recipestep")]
    [ApiController]
    public class RecipeStepController : ControllerBase
    {
        private readonly IRecipeStepService _recipeStepService;
        public RecipeStepController(IRecipeStepService recipeStepService)
        {
            _recipeStepService = recipeStepService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> Get()
        {
            var rs = await _recipeStepService.GetRecipeSteps();
            return Ok(rs);
        }
    }
}
