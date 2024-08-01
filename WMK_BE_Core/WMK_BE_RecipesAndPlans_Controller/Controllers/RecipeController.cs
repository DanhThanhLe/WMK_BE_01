using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.Recipe;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.RecipeModel;
using WMK_BE_BusinessLogic.Service.Interface;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
        [Authorize]
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

        //[HttpGet("get-by-name/{name?}/{status?}")]//status true là tìm recipe cho đặt hàng, false là tìm để coi thôi
        //public async Task<IActionResult> GetByRecipeName([FromRoute] string name = "", [FromRoute] bool status = true)
        //{
        //    var result = await _recipeService.GetRecipeByName(name, status);
        //    return Ok(result);
        //}
        [HttpGet("get-by-name/{name?}/{status?}")]
        public async Task<IActionResult> GetByRecipeName(
        [SwaggerParameter("Name of the recipe", Required = false)] string name = "",
        [SwaggerParameter("Status of the recipe", Required = false)] bool status = true)
        {
            if (name == "{name}" || name == null)
            {
                name = ""; // Đặt giá trị mặc định nếu không được cung cấp
            }
            var result = await _recipeService.GetRecipeByName(name, status);
            return StatusCode(result.StatusCode , result);
        }

        [HttpPost("create-new")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateRecipeRequest model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized(new { Message = "Invalid token, user ID not found" });
            }
            string createdBy = userId.ToString();
            var result = await _recipeService.CreateRecipeAsync(createdBy,model);
            return StatusCode(result.StatusCode , result);
        }
        
        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRecipeRequest model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized(new { Message = "Invalid token, user ID not found" });
            }
            string updatedBy = userId.ToString();
            var result = await _recipeService.UpdateRecipeAsync(id, model, updatedBy);
            return StatusCode(result.StatusCode , result);
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
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
        [Authorize]
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
