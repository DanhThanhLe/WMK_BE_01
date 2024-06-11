using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.UtilModel;

namespace WMK_BE_RecipesAndPlans_Controller.Controllers
{
    [ApiController]
    [Route("api/util")]
    public class UtilController : ControllerBase
    {
        // GET: UtilController


        [HttpPost("UploadFile")]
        public async Task<ActionResult> PostFile( FileUploadRequest model)
        {
         
            var fileName = DateTime.Now.Microsecond+ model.File.FileName;
            
            var tempDir = Path.Combine(Directory.GetCurrentDirectory(), "tempfiles");
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }

            var filePath = Path.Combine(tempDir, fileName);
            //create temp folder

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
              await  model.File.CopyToAsync(stream);
            }

            using (var client = new WebClient())
            {
                client.Credentials = new NetworkCredential("administrator", "000000Long@");

                client.UploadFile("ftp://wemealkit.shop/images/" + fileName, WebRequestMethods.Ftp.UploadFile, filePath);
            }
            return Ok("https://cdn.wemealkit.shop/"+ fileName);
        }


    }
}
