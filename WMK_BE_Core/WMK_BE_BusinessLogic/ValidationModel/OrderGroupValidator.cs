using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.OrderGroupModel;

namespace WMK_BE_BusinessLogic.ValidationModel
{
	public class CreateOrderGroupModelValidator : AbstractValidator<CreateOrderGroupRequest>
	{
		private readonly IExpendValidator _expendValidator;
		public CreateOrderGroupModelValidator()
		{
			_expendValidator = new ExpendValidator();
			RuleFor(o => o.ShipperId).NotEmpty().WithMessage("ID shipper is required!")
					.Must(_expendValidator.BeValidGuid).WithMessage("ID invalid fortmat GUID!"); 
			RuleFor(o => o.AsignBy).NotEmpty().WithMessage("Asign by is required!")
					.Must(_expendValidator.BeValidGuid).WithMessage("ID invalid fortmat GUID!");
			RuleFor(o => o.Location).NotEmpty().WithMessage("Location is required!");
		}
	}
	public class UpdateOrderGroupModelValidator : AbstractValidator<UpdateOrderGroupRequest>
	{
		private readonly IExpendValidator _expendValidator;
		public UpdateOrderGroupModelValidator()
		{
			_expendValidator = new ExpendValidator();
			RuleFor(o => o.Latitude).NotEmpty().WithMessage("Latitude is required!");
			RuleFor(o => o.Longitude).NotEmpty().WithMessage("Longitude is required!");
			RuleFor(o => o.ShipperId).NotEmpty().WithMessage("ID shipper is required!")
					.Must(_expendValidator.BeValidGuid).WithMessage("ID invalid fortmat GUID!"); 
			RuleFor(o => o.Id).NotEmpty().WithMessage("ID is required!")
					.Must(_expendValidator.BeValidGuid).WithMessage("ID invalid fortmat GUID!");
			RuleFor(o => o.Location).NotEmpty().WithMessage("Location is required!");
		}
	}
}
