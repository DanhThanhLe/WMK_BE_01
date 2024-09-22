using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.TransactionModel;

namespace WMK_BE_BusinessLogic.ValidationModel
{
	public class CreateZaloPayValidator : AbstractValidator<ZaloPayCreatePaymentRequest>
	{
        public CreateZaloPayValidator()
        {
            RuleFor(x => x.OrderId).NotEmpty().WithMessage("Không để trống");
            RuleFor(x => x.Amount).NotEmpty().WithMessage("Không để trống")
                .Must(x => x > 0).WithMessage("Phải trên 0 đồng");
        }
    }

    public class CreateTransactionValidator : AbstractValidator<CreatePaymentRequest>
    {
        public CreateTransactionValidator()
        {
            RuleFor(x => x.OrderId).NotEmpty().WithMessage("Không để trống");
            RuleFor(x => x.Amount).NotEmpty().WithMessage("Không để trống")
                .Must(x => x > 0).WithMessage("Phải trên 0 đồng!");
            RuleFor(x => x.TransactionType).IsInEnum().WithMessage("Phương thức thanh toán không được chấp nhận");
        }
    }

}
