using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_BusinessLogic.BusinessModel.ResponseModel.UserModel
{
	public class BaseUserResponse
	{
		public Guid Id { get; set; }
		public string Email { get; set; } = string.Empty;
	}
}
