using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.AuthModel
{
	public class CheckEmailConfirmRequest
	{
        public string Email { get; set; } = string.Empty;
		public string Code { get; set; } = string.Empty;
    }
}
