﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_BusinessLogic.Service.Interface
{
	public interface ISendMailService
	{
		public bool SendMail(string to , string subject , string body);
	}
}
