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
			//.Must(shipDate => shipDate > DateTime.UtcNow.AddHours(7))
			//.WithMessage("ShipDate must be after the current date.");
			RuleFor(x => x.RecipeList).NotNull().WithMessage("The list cannot be null.")
			.NotEmpty().WithMessage("The list cannot be empty.");
			RuleForEach(x => x.RecipeList).NotNull().WithMessage("The item cannot be null.")
			.NotEmpty().WithMessage("The item cannot be empty.");
			RuleFor(x => x.TransactionType).IsInEnum().WithMessage("Transaction type not accepted");

		}
	}
	public class UpdateOrderModelValidator : AbstractValidator<UpdateOrderRequest>
	{
		private readonly IExpendValidator _expendValidator;
		public UpdateOrderModelValidator()
		{
			_expendValidator = new ExpendValidator();
			//RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required!")
			//	.Must(_expendValidator.BeValidGuid).WithMessage("Id invalid fortmat GUID!");
			RuleFor(x => x.ShipDate).NotEmpty().WithMessage("ShipDate is required!")
				.Must(shipDate => shipDate > DateTime.UtcNow.AddHours(7))
				.WithMessage("ShipDate must be after the current date.");
			RuleFor(x => x.TotalPrice).GreaterThan(0).WithMessage("TotalPrice must be greater than 0!");
			RuleFor(x => x.Latitude).NotEmpty().WithMessage("Latitude is required!");
			RuleFor(x => x.Longitude).NotEmpty().WithMessage("Longitude is required!");
		}
	}

	public class UpdateOrderByUserModelValidator : AbstractValidator<UpdateOrderByUserRequest>
	{
		private readonly IExpendValidator _expendValidator;
        public UpdateOrderByUserModelValidator()
        {
            _expendValidator= new ExpendValidator();
			RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required!")
					.Must(_expendValidator.BeValidGuid).WithMessage("Id invalid format GUID!");
			RuleFor(x => x.Latitude).NotEmpty().WithMessage("Latitude is required!");
			RuleFor(x => x.Longitude).NotEmpty().WithMessage("Longitude is required!");
        }
    }

}
