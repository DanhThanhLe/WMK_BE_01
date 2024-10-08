﻿using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.WeeklyPlanModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.WeeklyPlanModel;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Implement;
using WMK_BE_BusinessLogic.Service.Interface;

namespace WMK_BE_RecipesAndPlans_Controller.Controllers
{
	[Route("api/weeklyplan")]
	[ApiController]
	public class WeeklyPLanController : ControllerBase
	{
		private readonly IWeeklyPlanService _weeklyPLanService;
		public WeeklyPLanController(IWeeklyPlanService weeklyPlanService)
		{
			_weeklyPLanService = weeklyPlanService;
		}
		#region Get
		[HttpGet("get-all")]//danh cho mobile
		public async Task<IActionResult> Get(string name = "")
		{
			var result = await _weeklyPLanService.GetAllAsync(name);
			return StatusCode(result.StatusCode , result);
		}

		[HttpGet("get-all-filter")] //danh cho web
		public async Task<IActionResult> GetAllByWeekly([FromQuery] GetAllRequest? model)
		{
			var result = await _weeklyPLanService.GetAllFilterAsync(model);
			return StatusCode(result.StatusCode , result);
		}

		[HttpGet("get-by-customer-id/{id}")]
		[Authorize]
		public async Task<IActionResult> GetByCustomerId(string id)
		{
			Guid convertId;
			if ( Guid.TryParse(id , out convertId) )
			{
				var result = await _weeklyPLanService.GetListByCustomerId(convertId);
				return StatusCode(result.StatusCode , result);
			}
			else
			{
				return BadRequest(new
				{
					StatusCode = 400 ,
					Message = "Invalid GUID format! Please provide a valid GUID!"
				});
			}
		}

		[HttpGet("get-id/{id}")]
		public async Task<IActionResult> GetId(Guid id)
		{
			var result = await _weeklyPLanService.GetByIdAsync(id);
			return StatusCode(result.StatusCode , result);
		}
		#endregion

		#region Create
		//
		[HttpPost("create")]
		[Authorize(Roles = "Staff,Manager,Admin")]
		public async Task<IActionResult> Create([FromBody] CreateWeeklyPlanRequest model)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if ( userId == null )
			{
				return Unauthorized(new { Message = "Invalid token, user ID not found" });
			}
			string createdBy = userId.ToString();
			var result = await _weeklyPLanService.CreateWeeklyPlanAsync(model , createdBy);
			return StatusCode(result.StatusCode , result);
		}
		//
		[HttpPost("create-for-customer")]
		[Authorize]
		public async Task<IActionResult> CreateForUser([FromBody] CreateWeeklyPlanForCustomerRequest model)
		{
			var result = await _weeklyPLanService.CreateForSutomer(model);
			return StatusCode(result.StatusCode , result);

		}
		#endregion

		#region Update
		//
		[HttpPut("update/{id}")]
		[Authorize(Roles = "Staff,Manager,Admin")]
		public async Task<IActionResult> Update(Guid id , [FromBody] UpdateWeeklyPlanRequestModel model)
		{
			var result = await _weeklyPLanService.UpdateWeeklyPlanAsync(id , model);
			return StatusCode(result.StatusCode , result);
		}
		//
		[HttpPut("update-full-info/{id}")]
		[Authorize]
		public async Task<IActionResult> UpdateFullInfo(Guid id , [FromBody] UpdateWeeklyPlanRequest request)
		{
			Guid convertId;
			if ( Guid.TryParse(id.ToString() , out convertId) )
			{
				var result = await _weeklyPLanService.UpdateFullInfo(id , request);
				return StatusCode(result.StatusCode , result);
			}
			else
			{
				return BadRequest(new
				{
					StatusCode = 400 ,
					Message = "Invalid GUID format! Please provide a valid GUID!"
				});
			}
		}
		#endregion

		[HttpPut("change-status/{id}")]
		[Authorize]
		public async Task<IActionResult> ChangeStatus(Guid id , [FromBody] ChangeStatusWeeklyPlanRequest model)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var result = await _weeklyPLanService.ChangeStatusWeeklyPlanAsync(userId , id , model);
			return StatusCode(result.StatusCode , result);
		}
		[HttpPut("change-base-status/{id}")]
		[Authorize]
		public async Task<IActionResult> ChangeBaseStatus(Guid id , [FromBody] ChangeBaseStatusWeeklyPlanRequest model)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var result = await _weeklyPLanService.ChangeBaseStatusWeeklyPlanAsync(id , model);
			return StatusCode(result.StatusCode , result);
		}

		[HttpPut("change-order/{status}")]
		[Authorize(Roles = "Staff,Manager,Admin")]
		public async Task<IActionResult> ChangeOrder(bool status)
		{
			var result = await _weeklyPLanService.OnOffOrderAsync(status);
			return StatusCode(result.StatusCode , result);
		}

		[HttpDelete("delete/{id}")]
		[Authorize(Roles = "Staff,Manager,Admin, Customer")]
		public async Task<IActionResult> Delete(Guid id)
		{
			Guid convertId;
			if ( Guid.TryParse(id.ToString() , out convertId) )
			{
				var result = await _weeklyPLanService.DeleteWeeklyPlanAsync(id);
				return StatusCode(result.StatusCode , result);
			}
			else
			{
				return BadRequest(new
				{
					StatusCode = 400 ,
					Message = "Invalid GUID format! Please provide a valid GUID!"
				});
			}
		}
	}
}
