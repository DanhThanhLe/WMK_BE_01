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
            return StatusCode(result.StatusCode , result);
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            Guid recipeId;
            if (Guid.TryParse(id, out recipeId))
            {
                var result = await _recipeService.GetRecipeById(recipeId.ToString());
                return StatusCode(result.StatusCode , result);
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

        [HttpGet("get-by-category/{categoryId}")]
        public async Task<IActionResult> GetWithCategoryId(string categoryId)
        {
            Guid categoryIdConvert;
            if (Guid.TryParse(categoryId,out categoryIdConvert))
            {
                var result = await _recipeService.GetListByCategoryId(categoryIdConvert);
                return StatusCode(result.StatusCode , result);
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

        [HttpGet("get-by-name/{name}/{status}")]//status true là tìm recipe cho đặt hàng, false là tìm để coi thôi
        public async Task<IActionResult> GetByRecipeName(string name = "", bool status = true)
        {
            var result = await _recipeService.GetRecipeByName(name, status);
            return StatusCode(result.StatusCode , result);
        }

        [HttpPost("create-new")]
        public async Task<IActionResult> Create([FromBody] CreateRecipeRequest model)
        {
            var result = await _recipeService.CreateRecipeAsync(model);
            return StatusCode(result.StatusCode , result);
        }
        
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateRecipeRequest model)
        {
            var result = await _recipeService.CreateRecipeAsync(model);
            return StatusCode(result.StatusCode , result);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteById(string id)
        {
            Guid recipeId;
            if (Guid.TryParse(id, out recipeId))
            {
                var result = await _recipeService.DeleteRecipeById(recipeId);
                return StatusCode(result.StatusCode , result);
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

        [HttpPut("change-status/{id}")]
        public async Task<IActionResult> UpdateStatusRecipe(Guid id,[FromQuery] ChangeRecipeStatusRequest request)
        {
            Guid recipeId;
            if (Guid.TryParse(id.ToString(), out recipeId))
            {
                var result = await _recipeService.ChangeStatus(id,request);
                return StatusCode(result.StatusCode , result);
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
