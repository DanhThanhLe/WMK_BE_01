﻿using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientModel;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.Recipe;

namespace WMK_BE_BusinessLogic.ValidationModel
{
    public class RecipeValidator : AbstractValidator<CreateRecipeRequest>
    {
        public RecipeValidator()//cai nay dung cho check tao moi recipe
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Không để trống tên");
            RuleFor(x => x.ServingSize).Must(t => t >= 1).WithMessage("Phải cho ít nhất 1 người ăn");
            //RuleFor(x => x.CreatedBy).NotEmpty().WithMessage("Not null at Created By");

            //RuleFor(x => x.CookingTime).Must(t => t > 5).WithMessage("Cooking time should be more than 5 minutes");
            RuleFor(x => x.Difficulty).IsInEnum().WithMessage("Sai độ khó");
            //RuleFor(x => x.ProcessStatus).IsInEnum().WithMessage("0 is show, 1 is not");
        }
    }

    public class RecipeChangeStatusValidator : AbstractValidator<ChangeRecipeStatusRequest>
    {
        public RecipeChangeStatusValidator()
        {
            //RuleFor(x => x.Id).NotEmpty().WithMessage("Not null id");
            RuleFor(x => x.ProcessStatus).NotNull().WithMessage("Không trống trạng thái xử lí")
                .IsInEnum().WithMessage("Tráng thái từ 0-4");
        }
    }

    public class IdRecipeValidator : AbstractValidator<IdRecipeRequest>
    {
        public IdRecipeValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("ID khong de trong");
        }
    }
}
