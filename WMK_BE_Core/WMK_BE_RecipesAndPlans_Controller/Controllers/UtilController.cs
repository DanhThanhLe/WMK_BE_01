using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.UtilModel;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Implement;

namespace WMK_BE_RecipesAndPlans_Controller.Controllers
{
    [ApiController]
    [Route("api/util")]
    public class UtilController : ControllerBase
    {
        // GET: UtilController  
        private readonly IRedisService _redisService;
        public UtilController(IRedisService redisService)
        {
            _redisService = redisService;
        }


        [HttpPost("UploadFile")]
        public async Task<ActionResult> PostFile( FileUploadRequest model)
        {
            var fileName = model.File.FileName;
            
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
              
                using (var client = new AmazonS3Client(Decrypt("K+7kO0wn3pXdD6bhJfB+/8Fs245UtxIzd4BTX9X0MwTwTK47i03iHnPvYk6LBPFo"),Decrypt("L4X7FKufD3jWMBSaz88L6HKZjTu9G0Lr8lTHcBXi76RfJSHGjP0ZKyMgRNLY/AUEqJ7TXfraaWKGUntRAmUJq+X1oMmKeT6uI2D6KCMS5fszKThtdAmwCEovW/K0kXVM") , RegionEndpoint.APSoutheast1))
                {
                    
                        var uploadRequest = new TransferUtilityUploadRequest
                        {
                            InputStream = stream,
                            Key = fileName,
                            BucketName = "wemealkit",
                            CannedACL = S3CannedACL.PublicRead
                        };

                        var fileTransferUtility = new TransferUtility(client);
                        await fileTransferUtility.UploadAsync(uploadRequest);
                    //get full path to send back to client
                    return Ok("https://wemealkit.s3.amazonaws.com/" + fileName);
                    }
                
            }

            //using (var client = new WebClient())
            //{
            //    client.Credentials = new NetworkCredential("administrator", "000000Long@");

            //    client.UploadFile("ftp://wemealkit.shop/images/" + fileName, WebRequestMethods.Ftp.UploadFile, filePath);
            //}
            return Ok("https://cdn.wemealkit.shop/"+ fileName);
        }

        [HttpGet("ClearRedisCache")]
        public async Task<ActionResult> ClearRedisCache()
        {
            var listKey = new string[] { "WeeklyPlanList" };
            foreach (var key in listKey)
            {
                await _redisService.RemoveAsync(key);
            }
          return Ok("Clear cache success");
        }

        //private simple encypt and decrypt
        private string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[]
                {
                    0x49,
                    0x76,
                    0x61,
                    0x6e,
                    0x20,
                    0x4d,
                    0x65,
                    0x64,
                    0x76,
                    0x65,
                    0x64,
                    0x65,
                    0x76
                });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        private string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[]
                {
                    0x49,
                    0x76,
                    0x61,
                    0x6e,
                    0x20,
                    0x4d,
                    0x65,
                    0x64,
                    0x76,
                    0x65,
                    0x64,
                    0x65,
                    0x76
                });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
    }
}
