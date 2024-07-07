using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientCategoryModel;
using WMK_BE_BusinessLogic.Service.Implement;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_RecipesAndPlans_Controller.Controllers
{
    [Route("api/ingredientcategories")]
    [ApiController]
    public class IngredientCategoryController : ControllerBase
    {
        private readonly IIngredientCategoryService _ingredientCategoryService;
        public IngredientCategoryController(IIngredientCategoryService ingredientCategoryService)
        {
            _ingredientCategoryService = ingredientCategoryService;
        }

        [HttpPost("create-new")]
        public async Task<IActionResult> Create([FromBody] CreateIngredientCategoryRequest request)
        {
            var result = await _ingredientCategoryService.CreateNew(request);
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] FullIngredientCategoryRequest request)
        {
            var result = await _ingredientCategoryService.UpdateCategory(request);
            return Ok(result);
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _ingredientCategoryService.GetAll();
            return Ok(result);
        }

        [HttpGet("get-by-name")]
        public async Task<IActionResult> GetByName([FromQuery] string name)
        {
            var result = await _ingredientCategoryService.GetByName(name);
            return Ok(result);
        }

        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetById([FromQuery] string id)
        {
            Guid convertId;
            if (Guid.TryParse(id, out convertId))
            {
                var result = await _ingredientCategoryService.GetById(convertId);
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

        [HttpDelete("delete-by-id")]
        public async Task<IActionResult> DeleteById([FromQuery] string id)
        {
            Guid convertID;
            if (Guid.TryParse(id, out convertID))
            {
                var result = await _ingredientCategoryService.DeleteById(convertID);
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
