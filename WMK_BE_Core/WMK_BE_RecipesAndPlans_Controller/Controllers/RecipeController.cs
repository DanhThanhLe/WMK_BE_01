using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.Recipe;
using WMK_BE_BusinessLogic.Service.Interface;

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

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _recipeService.GetRecipes();
            return Ok(result);
        }

        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetById([FromQuery] string id)
        {
            Guid recipeId;
            if (Guid.TryParse(id, out recipeId))
            {
                var result = await _recipeService.GetRecipeById(recipeId.ToString());
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

        [HttpGet("get-by-category")]
        public async Task<IActionResult> GetWithCategoryId([FromQuery] string id)
        {
            Guid categoryId;
            if (Guid.TryParse(id,out categoryId))
            {
                var result = await _recipeService.GetListByCategoryId(categoryId);
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

        [HttpGet("get-by-name")]//status true là tìm recipe cho đặt hàng, false là tìm để coi thôi
        public async Task<IActionResult> GetByRecipeName([FromQuery] string name = "", bool status = true)
        {
            var result = await _recipeService.GetRecipeByName(name, status);
            return Ok(result);
        }

        [HttpPost("create-new")]
        public async Task<IActionResult> Create([FromBody] CreateRecipeRequest model)
        {
            var result = await _recipeService.CreateRecipeAsync(model);
            return Ok(result);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteById([FromQuery] string id)
        {
            Guid recipeId;
            if (Guid.TryParse(id, out recipeId))
            {
                var result = await _recipeService.DeleteRecipeById(recipeId);
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
