using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.UtilModel
{
    public class FileUploadRequest
    {
       
        public IFormFile? File { get; set; }
    }
}
