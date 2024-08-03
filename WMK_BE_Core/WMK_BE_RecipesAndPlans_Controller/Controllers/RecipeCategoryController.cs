using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WMK_BE_BusinessLogic.Service.Implement;
using WMK_BE_BusinessLogic.Service.Interface;

namespace WMK_BE_RecipesAndPlans_Controller.Controllers
{
    [Route("api/recipecategories")]
    [ApiController]
    public class RecipeCategoryController : ControllerBase
    {
        private readonly IRecipeCategoryService _recipeCategoryService;
        public RecipeCategoryController(IRecipeCategoryService recipeCategoryService)
        {
            _recipeCategoryService = recipeCategoryService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll(string name="")
        {
            var result = await _recipeCategoryService.GetAll();
            return StatusCode(result.StatusCode , result);
        }

        [HttpGet("get-by-recipe-id/{recipeId}")]
        public async Task<IActionResult> GetListByRecipeId(string recipeId)
        {
            Guid id;
            if (Guid.TryParse(recipeId, out id))
            {
                var result = await _recipeCategoryService.GetListByRecipeId(id);
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
