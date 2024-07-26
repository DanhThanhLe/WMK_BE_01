﻿using FluentValidation;
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
            RuleFor(x => x.OrderId).NotEmpty().WithMessage("Order id is required!");
            RuleFor(x => x.OrderPrice).NotEmpty().WithMessage("Price is required!")
                .Must(x => x > 0).WithMessage("Price is more than 0!");
        }
    }
}