using FluentValidation.Validators;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.AuthModel;

namespace WMK_BE_BusinessLogic.ValidationModel
{
	public class LoginModelValidator : AbstractValidator<LoginModel>
	{
		//private readonly IExpendValidator _expendValidator;
		public LoginModelValidator()
		{
			//_expendValidator = new ExpendValidator();
			RuleFor(x => x.EmailOrUserName)
				.NotEmpty().WithMessage("Email hay Tên người dùng không để trống!");//check email or username
																					//.EmailAddress().When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("Invalid email format!")
																					//.Must(_emailValidator.BeValidEmail).When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("Invalid email domain!");
			RuleFor(x => x.Password).NotEmpty().WithMessage("Mật khẩu không được để trống!");
				//.Matches(@"^(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*()])(?=.{6,})")
				//.WithMessage("Mật khẩu chứa ít nhất 1 kí tự in hoa, 1 kí tự thường, 1 số, dài ít nhất 6 kí tự!");
		}
	}

	public class RegisterModelValidator : AbstractValidator<RegisterModel>
	{
		private readonly IExpendValidator _expendValidator;
		public RegisterModelValidator()
		{
			_expendValidator = new ExpendValidator();
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email không được để trống!")
				.EmailAddress().When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("Email không đúng!")
				.Must(_expendValidator.BeValidEmail).When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("Email không đúng chuẩn!");
			RuleFor(x => x.FirstName).NotEmpty().WithMessage("Tên không được để trống!");
			RuleFor(x => x.LastName).NotEmpty().WithMessage("Họ không được để trống!");
			// RuleFor(x => x.Role).NotEmpty().WithMessage("Role is required!");
			RuleFor(x => x.Password).NotEmpty().WithMessage("Mật khẩu không để trống!")
				.Matches(@"^(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*()])(?=.{6,})")
				.WithMessage("Mật khẩu chứa ít nhất 1 kí tự in hoa, 1 kí tự thường, 1 số, dài ít nhất 6 kí tự!");
			RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("ConfirmPassword is required!");
			//RuleFor(x => x.Dob)
			//.Must(dob =>
			//{
			//	if ( dob == null )
			//		return true;
			//	var today = DateTime.Today;
			//	var minDate = today.AddYears(-6);
			//	return dob < minDate;
			//}).WithMessage("Date of birth must be before 6 years!");
		}
	}
	public class ResetPasswordModelValidator : AbstractValidator<ResetPasswordRequest>
	{
		private readonly IExpendValidator _expendValidator;
		public ResetPasswordModelValidator()
		{
			_expendValidator = new ExpendValidator();
			RuleFor(x => x.Id).NotEmpty().WithMessage("Id không được để trống!")
				.Must(_expendValidator.BeValidGuid).WithMessage("Id không đúng chuẩn!");
			RuleFor(x => x.OldPassword).NotEmpty().WithMessage("Mật khẩu cũ không được để trống!");
			RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Mật khẩu xác nhận không được để trống!");
			RuleFor(x => x.NewPassword).NotEmpty().WithMessage("Mật khẩu mới không được để trống!")
							.Matches(@"^(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*()])(?=.{6,})")
							.WithMessage("Mật khẩu chứa ít nhất 1 kí tự in hoa, 1 kí tự thường, 1 số, dài ít nhất 6 kí tự!");
		}
	}
	public class ResetPasswordByEmailModelValidator : AbstractValidator<ResetPasswordByEmailRequest>
	{
		private readonly IExpendValidator _expendValidator;
		public ResetPasswordByEmailModelValidator()
		{
			_expendValidator = new ExpendValidator();
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Id không được để trống!")
				.EmailAddress().When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("Id không đúng chuẩn!")
				.Must(_expendValidator.BeValidEmail).When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("Id không đúng chuẩn!");
			RuleFor(x => x.CodeReset).NotEmpty().WithMessage("Codekhông được để trống!");
			RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Mật khẩu cũ không được để trống!");
			RuleFor(x => x.NewPassword).NotEmpty().WithMessage("Mật khẩu mới không được để trống!")
							.Matches(@"^(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*()])(?=.{6,})")
							.WithMessage("Mật khẩu chứa ít nhất 1 kí tự in hoa, 1 kí tự thường, 1 số, dài ít nhất 6 kí tự!");
		}
	}
	public class SendCodeByEmailModelValidator : AbstractValidator<SendCodeByEmailRequest>
	{
		private readonly IExpendValidator _expendValidator;

		public SendCodeByEmailModelValidator()
		{
			_expendValidator = new ExpendValidator();
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email không được để trống!")
				.EmailAddress().When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("Email không đúng chuẩn!")
				.Must(_expendValidator.BeValidEmail).When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("Email không đúng chuẩn!");
		}
	}
}
