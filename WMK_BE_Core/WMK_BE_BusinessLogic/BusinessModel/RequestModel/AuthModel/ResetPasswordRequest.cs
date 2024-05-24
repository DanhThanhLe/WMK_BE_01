using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.AuthModel
{
	public class ResetPasswordRequest
	{
		public string Id { get; set; } = string.Empty;
		public string OldPassword { get; set; } = string.Empty;
		public string NewPassword { get; set; } = string.Empty;
		public string ConfirmPassword { get; set; } = string.Empty;
	}
}
