using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_BusinessLogic.ValidationModel
{
    //public class RecipeValidator : AbstractValidator<RecipeCreateModelRequest>
    //{
    //    public RecipeValidator()
    //    {
    //        RuleFor(x => x.NutriValueId).NotEmpty().WithMessage("Not null at nutrition ID");
    //        RuleFor(x => x.RecipeStyle).NotEmpty().WithMessage("Not null at RecipeStyle");
    //        RuleFor(x => x.CookingMethod).NotEmpty().WithMessage("Not null at Cooking method");
    //        RuleFor(x => x.CategoriesList).NotEmpty().WithMessage("not null for Categories list");
    //        RuleFor(x => x.CookingTime).Must(t => t > 5).WithMessage("Cooking time should be more than 5 minutes");
    //        RuleFor(x => x.ServingSize).Must(t => t >= 1).WithMessage("Must be at leat 1 person to eat");
    //        RuleFor(x => x.Difficult).Must(t => t >= 1).WithMessage("difficult 1 is lowest");
    //        RuleFor(x => x.Status).Must(t => t == 0 || t == 1).WithMessage("0 is show, 1 is not");
    //        RuleFor(x => x.CreatedBy).NotEmpty().WithMessage("Not null at Created By");
    //    }
    //}
}
