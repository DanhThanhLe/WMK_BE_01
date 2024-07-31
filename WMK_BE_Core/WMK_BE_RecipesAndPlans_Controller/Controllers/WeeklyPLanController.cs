using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.WeeklyPlanModel;
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

        [HttpGet("get-all")]
        public async Task<IActionResult> Get()
        {
            var result = await _weeklyPLanService.GetAllAsync();
            return StatusCode(result.StatusCode , result);  
        }

        [HttpGet("get-by-customer-id/{id}")]
        public async Task<IActionResult> GetByCustomerId(string id)
        {
            Guid convertId;
            if (Guid.TryParse(id, out convertId))
            {
                var result = await _weeklyPLanService.GetListByCustomerId(convertId);
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

        [HttpGet("get-id/{id}")]
        public async Task<IActionResult> GetId(Guid id)
        {
            var result = await _weeklyPLanService.GetByIdAsync(id);
			return StatusCode(result.StatusCode , result);
		}

		[HttpPost("create")]
        public async Task<IActionResult> Create([FromBody]CreateWeeklyPlanRequest model) 
        {
            var result = await _weeklyPLanService.CreateWeeklyPlanAsync(model);
			return StatusCode(result.StatusCode , result);
		}

		[HttpPost("create-for-customer")]
        public async Task<IActionResult> CreateForUser([FromBody] CreateWeeklyPlanRequest model)
        {
            var result = await _weeklyPLanService.CreateForSutomer(model);
            return StatusCode(result.StatusCode , result);

		}

		[HttpPut("update/{id}")]
        public async Task<IActionResult> Update(Guid id,[FromBody]UpdateWeeklyPlanRequestModel model) 
        {
            var result = await _weeklyPLanService.UpdateWeeklyPlanAsync(id,model);
            return StatusCode(result.StatusCode , result);
        }

        [HttpPut("update-full-info/{id}")]
        public async Task<IActionResult> UpdateFullInfo(Guid id, [FromBody]UpdateWeeklyPlanRequest request)
        {
            Guid convertId;
            if (Guid.TryParse(id.ToString(), out convertId))
            {
                var result = await _weeklyPLanService.UpdateFullInfo(id,request);
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

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id) 
        {
            Guid convertId;
            if (Guid.TryParse(id.ToString(), out convertId))
            {
                var result = await _weeklyPLanService.DeleteWeeklyPlanAsync(id);
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
