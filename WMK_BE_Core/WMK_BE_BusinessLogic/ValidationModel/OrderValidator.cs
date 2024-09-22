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
			RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId không bỏ trống")
				.Must(_expendValidator.BeValidGuid).WithMessage("UserId sai fortmat GUID!");
			RuleFor(x => x.Address).NotEmpty().WithMessage("Address không bỏ trống!");
			//RuleFor(x => x.TotalPrice).NotEmpty().WithMessage("TotalPrice is required!");
			//RuleFor(x => x.ShipDate).NotEmpty().WithMessage("ShipDate is required!")
			//.Must(shipDate => shipDate > DateTime.UtcNow.AddHours(7))
			//.WithMessage("ShipDate must be after the current date.");
			RuleFor(x => x.RecipeList).NotNull().WithMessage("Không được bỏ trống danh sách món ăn")
			.NotEmpty().WithMessage("Không được bỏ trống danh sách món ăn");
			RuleForEach(x => x.RecipeList).NotNull().WithMessage("Không được trống thông tin món ăn")
			.NotEmpty().WithMessage("Không bỏ trống món ăn");
			RuleFor(x => x.TransactionType).IsInEnum().WithMessage("Phương thức thanh toán không được chấp nhận");

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
			RuleFor(x => x.Id).NotEmpty().WithMessage("Id không bỏ trống")
					.Must(_expendValidator.BeValidGuid).WithMessage("Id sai format GUID!");
			RuleFor(x => x.Latitude).NotEmpty().WithMessage("Latitude không bỏ trống");
			RuleFor(x => x.Longitude).NotEmpty().WithMessage("Longitude không bỏ trống");
        }
    }

}
