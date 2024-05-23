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
				.NotEmpty().WithMessage("Email is required!")
				.EmailAddress().When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("Invalid email format!")
				.Must(_expendValidator.BeValidEmail).When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("Invalid email domain!");
			RuleFor(x => x.Role).NotEmpty().WithMessage("Role is required!");
			RuleFor(x => x.FirstName).NotEmpty().WithMessage("FirstName is required!");
			RuleFor(x => x.LastName).NotEmpty().WithMessage("LastName is required!");
		}
	}

	public class UpdateModelValidator : AbstractValidator<UpdateUserRequest>
	{
		private readonly IExpendValidator _expendValidator;
		public UpdateModelValidator()
		{
			_expendValidator = new ExpendValidator();
			RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required!")
				.Must(_expendValidator.BeValidGuid).WithMessage("Id must be a valid GUID!");
			RuleFor(x => x.Email)
			.Empty().When(x => x.Email == null).WithMessage("Email should be null.")
			.EmailAddress().When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("Invalid email format!")
			.Must(_expendValidator.BeValidEmail).When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("Invalid email domain.");
		}
	}
	public class IdUserModelValidator : AbstractValidator<IdUserRequest>
	{
		private readonly IExpendValidator _expendValidator;
		public IdUserModelValidator()
		{
			_expendValidator = new ExpendValidator();
			RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required!")
				.Must(_expendValidator.BeValidGuid).WithMessage("Id must be a valid GUID!");
		}
	}
	public class ChangeRoleUserModelValidator : AbstractValidator<ChangeRoleUserRequest>
	{
		private readonly IExpendValidator _expendValidator;
		public ChangeRoleUserModelValidator()
		{
			_expendValidator = new ExpendValidator();
			RuleFor(x => x.Id).NotEmpty().WithMessage("ID is required!")
				.Must(_expendValidator.BeValidGuid).WithMessage("Id must be a valid GUID!");
			RuleFor(x => x.NewRole).NotEmpty().WithMessage("NewRole is required!");
		}
	}
}
