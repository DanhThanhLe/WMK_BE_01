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
            return StatusCode(result.StatusCode , result);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] FullIngredientCategoryRequest request)
        {
            var result = await _ingredientCategoryService.UpdateCategory(id, request);
            return StatusCode(result.StatusCode , result);
		}

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _ingredientCategoryService.GetAll();
            return StatusCode(result.StatusCode , result);
        }

        [HttpGet("get-by-name/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var result = await _ingredientCategoryService.GetByName(name);
            return StatusCode(result.StatusCode , result);
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            Guid convertId;
            if (Guid.TryParse(id, out convertId))
            {
                var result = await _ingredientCategoryService.GetById(convertId);
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

        [HttpDelete("delete-by-id/{id}")]
        public async Task<IActionResult> DeleteById(string id)
        {
            Guid convertID;
            if (Guid.TryParse(id, out convertID))
            {
                var result = await _ingredientCategoryService.DeleteById(convertID);
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
