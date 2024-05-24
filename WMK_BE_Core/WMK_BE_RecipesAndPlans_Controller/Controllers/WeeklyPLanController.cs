using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.WeeklyPlanModel;
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

        [HttpGet("get-all")]
        public async Task<IActionResult> Get()
        {
            var result = await _weeklyPLanService.GetAllAsync();
            return Ok(result);  
        }
        [HttpGet("get-id")]
        public async Task<IActionResult> GetId(Guid id)
        {
            var result = await _weeklyPLanService.GetByIdAsync(id);
            return Ok(result);  
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateWeeklyPlanRequest model) 
        {
            var result = await _weeklyPLanService.CreateWeeklyPlanAsync(model);
            return Ok(result);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody]UpdateWeeklyPlanRequestModel model) 
        {
            var result = await _weeklyPLanService.UpdateWeeklyPlanAsync(model);
            return Ok(result);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody]DeleteWeeklyPlanRequestModel model) 
        {
            var result = await _weeklyPLanService.DeleteWeeklyPlanAsync(model);
            return Ok(result);
        }
    }
}
