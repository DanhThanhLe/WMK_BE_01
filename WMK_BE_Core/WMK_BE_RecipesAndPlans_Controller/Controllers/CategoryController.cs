﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.CategoryModel;
using WMK_BE_BusinessLogic.Service.Interface;

namespace WMK_BE_RecipesAndPlans_Controller.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> Get()
        {
            var result = await _categoryService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetById([FromQuery] string id)
        {
            if (Guid.TryParse(id, out var categoryId))
            {
                var result = await _categoryService.GetByIdAsync(categoryId);
                return Ok(result);
            }
            else
            {
                return BadRequest("Wrong Id fortmat!");
            }
        }

        [HttpGet("get-by-type")]
        public async Task<IActionResult> GetByType([FromQuery] string type)
        {
            if (type != null)
            {
                var result = await _categoryService.GetCategoryByType(type);
                return Ok(result);
            }
            else
            {
                return BadRequest("Empty request");
            }
        }

        [HttpGet("get-by-name")]
        public async Task<IActionResult> GetByName([FromQuery] string name)
        {
            if (name != null)
            {
                var result = await _categoryService.GetcategoryByName(name);
                return Ok(result);
            }
            else
            {
                return BadRequest("Empty request");
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequestModel model)
        {
            var result = await _categoryService.CreateCategoryAsync(model);
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UpdateCategoryRequestModel model)
        {
            var result = await _categoryService.UpdateCategoryAsync(model);
            return Ok(result);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteCategoryRequestModel model)
        {
            var result = await _categoryService.DeleteCategoryAsync(model);
            return Ok(result);
        }
    }
}
