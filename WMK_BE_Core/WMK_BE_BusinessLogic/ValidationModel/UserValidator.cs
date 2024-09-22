using FluentValidation.Validators;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.UserModel;
using static WMK_BE_BusinessLogic.BusinessModel.RequestModel.UserModel.ChangeRoleUserRequest;

namespace WMK_BE_BusinessLogic.ValidationModel
{
	public class CreateUserModelValidator : AbstractValidator<CreateUserRequest>
	{
		private readonly IExpendValidator _expendValidator;
		public CreateUserModelValidator()
		{
			_expendValidator = new ExpendValidator();
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email không để trống!")
				.EmailAddress().When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("Email sai format!")
				.Must(_expendValidator.BeValidEmail).When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("email sai format!");
			RuleFor(x => x.Role).NotEmpty().WithMessage("Role không để trống");
			RuleFor(x => x.FirstName).NotEmpty().WithMessage("Tên không để trống");
			RuleFor(x => x.LastName).NotEmpty().WithMessage("Họ không để trống");
		}
	}

	public class UpdateModelValidator : AbstractValidator<UpdateUserRequest>
	{
		private readonly IExpendValidator _expendValidator;
		public UpdateModelValidator()
		{
			_expendValidator = new ExpendValidator();
			//RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required!")
			//	.Must(_expendValidator.BeValidGuid).WithMessage("Id must be a valid GUID!");
			RuleFor(x => x.Email)
			.Empty().When(x => x.Email == null).WithMessage("Email không trống")
			.EmailAddress().When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("Sai format email")
			.Must(_expendValidator.BeValidEmail).When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("Sai format email");
		}
	}
	public class IdUserModelValidator : AbstractValidator<IdUserRequest>
	{
		private readonly IExpendValidator _expendValidator;
		public IdUserModelValidator()
		{
			_expendValidator = new ExpendValidator();
			RuleFor(x => x.Id).NotEmpty().WithMessage("Id không để trống")
				.Must(_expendValidator.BeValidGuid).WithMessage("Id sai format GUID!");
		}
	}
	public class ChangeRoleUserModelValidator : AbstractValidator<ChangeRoleUserRequest>
	{
		private readonly IExpendValidator _expendValidator;
		public ChangeRoleUserModelValidator()
		{
			_expendValidator = new ExpendValidator();
			RuleFor(x => x.NewRole).NotNull().WithMessage("NewRole is required!");
		}
	}
}
