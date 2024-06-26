﻿using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> GetAll()
        {
            var result = await _recipeCategoryService.GetAll();
            return Ok(result);
        }

        [HttpGet("get-by-recipe-id")]
        public async Task<IActionResult> GetListByRecipeId([FromQuery] string recipeId)
        {
            Guid id;
            if (Guid.TryParse(recipeId, out id))
            {
                var result = await _recipeCategoryService.GetListByRecipeId(id);
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
