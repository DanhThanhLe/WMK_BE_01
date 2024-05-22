﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_BusinessLogic.Helpers;

namespace WMK_BE_BusinessLogic.Service.Implement
{
	public class SendMailService : ISendMailService
	{
		public bool SendMail(string to , string subject , string body)
		{
			try
			{
				MailMessage msg = new MailMessage();
				msg.From = new MailAddress(ConstantHelper.emailSender , "WeMealKit");
				msg.To.Add(to);
				msg.Subject = subject;
				msg.IsBodyHtml = true;
				msg.Body = body;

				SmtpClient client = new SmtpClient();
				client.Port = 587;
				client.Host = ConstantHelper.hostEmail;
				client.EnableSsl = true;
				client.UseDefaultCredentials = false;
				client.Credentials = new NetworkCredential(ConstantHelper.emailSender , ConstantHelper.passwordSender);
				client.DeliveryMethod = SmtpDeliveryMethod.Network;
				client.Send(msg);
			}
			catch ( Exception )
			{
				return false;
			}
			return true;
		}
	}
}
