using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.OrderModel;

namespace WMK_BE_BusinessLogic.ValidationModel
{
	public class CreateOrderModelValidator : AbstractValidator<CreateOrderRequest>
	{
		private readonly IExpendValidator _expendValidator;
		public CreateOrderModelValidator()
		{
			_expendValidator = new ExpendValidator();
			RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required!")
				.Must(_expendValidator.BeValidGuid).WithMessage("UserId invalid fortmat GUID!");
			RuleFor(x => x.Address).NotEmpty().WithMessage("Address is required!");
			//RuleFor(x => x.TotalPrice).NotEmpty().WithMessage("TotalPrice is required!");
			//RuleFor(x => x.ShipDate).NotEmpty().WithMessage("ShipDate is required!")
				//.Must(shipDate => shipDate > DateTime.Now)
				//.WithMessage("ShipDate must be after the current date.");
		}
	}
	public class UpdateOrderModelValidator : AbstractValidator<UpdateOrderRequest>
	{
		private readonly IExpendValidator _expendValidator;
		public UpdateOrderModelValidator()
		{
			_expendValidator = new ExpendValidator();
			RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required!")
				.Must(_expendValidator.BeValidGuid).WithMessage("Id invalid fortmat GUID!");
			RuleFor(x => x.ShipDate).NotEmpty().WithMessage("ShipDate is required!")
				.Must(shipDate => shipDate > DateTime.Now)
				.WithMessage("ShipDate must be after the current date.");
			RuleFor(x => x.TotalPrice).GreaterThan(0).WithMessage("TotalPrice must be greater than 0!");
		}
	}
}
