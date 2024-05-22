using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WMK_BE_BusinessLogic.ValidationModel
{
	public interface IExpendValidator
	{
		bool BeValidEmail(string email);
		bool BeValidDateOfBirth(DateTime? dateOfBirth);
		bool BeValidGuid(string id);
	}
	public class ExpendValidator : IExpendValidator
	{
		public bool BeValidEmail(string email)
		{
			if ( string.IsNullOrEmpty(email) )
				return true; // Do nothing if email is null or empty

			string emailRegexPattern = @"^[^@\s]+@[^@\d\s]+\.(com|vn)$";

			//check domain
			return Regex.IsMatch(email , emailRegexPattern);
		}
		public bool BeValidDateOfBirth(DateTime? dateOfBirth)
		{
			if ( !dateOfBirth.HasValue )
			{
				return true;
			}

			var today = DateTime.Today;
			var minDate = today.AddYears(-6);
			return dateOfBirth.Value < minDate;
		}

		public bool BeValidGuid(string id)
		{
			return Guid.TryParse(id , out _);
		}
	}
}
