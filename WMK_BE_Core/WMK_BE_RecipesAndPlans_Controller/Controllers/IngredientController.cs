using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientModel;
using WMK_BE_BusinessLogic.Service.Implement;
using WMK_BE_BusinessLogic.Service.Interface;

namespace WMK_BE_RecipesAndPlans_Controller.Controllers
{
    [Route("api/ingredients")]
    [ApiController]
    public class IngredientController : ControllerBase
    {
        private readonly IIngredientService _ingredientService;
        public IngredientController(IIngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _ingredientService.GetIngredients();
            return Ok(result);
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetById([FromQuery]string id)
        {
            Guid ingredientId;
            if (Guid.TryParse(id, out ingredientId))
            {
                var result = await _ingredientService.GetIngredientById(ingredientId);
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

        [HttpGet("get-name")]
        public async Task<IActionResult> GetByName([FromQuery] string name)
        {
            var result = await _ingredientService.GetIngredientByName(name);
            return Ok(result);
        }

        [HttpPost("create-new")]
        public async Task<IActionResult> CreateNew([FromBody] CreateIngredientRequest model)
        {
            var result = await _ingredientService.CreateIngredient(model);
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] IngredientRequest model)
        {
            var result = await _ingredientService.UpdateIngredient(model);
            return Ok(result);
        }

        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusIngredientRequest model)
        {
            var result = await _ingredientService.ChangeStatus(model);
            return Ok(result);
        }

        [HttpPut("remove-from-app")]
        public async Task<IActionResult> RemoveById([FromQuery] string id)//dung de chuyen status thanh Unavailable
        {
            Guid ingredientId;
            if (Guid.TryParse(id, out ingredientId))
            {
                var result = await _ingredientService.RemoveIngredientById(ingredientId);
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

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteById([FromQuery] string id)
        {
            Guid ingredientId;
            if (Guid.TryParse(id, out ingredientId))
            {
                var result = await _ingredientService.DeleteIngredientById(ingredientId);
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
