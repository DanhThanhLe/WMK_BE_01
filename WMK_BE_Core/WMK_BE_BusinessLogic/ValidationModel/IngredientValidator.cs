using FluentValidation;
using FluentValidation.AspNetCore;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientCategoryModel;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientModel;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientNutrientModel;

namespace WMK_BE_BusinessLogic.ValidationModel
{
    public class IngredientValidator : AbstractValidator<CreateIngredientRequest>
    {
        public IngredientValidator()
        {
            RuleFor(x => x.IngredientCategoryId).NotEmpty().WithMessage("Category khong de trong");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Ten khong de trong");
            RuleFor(x => x.Img).NotEmpty().WithMessage("Image link must not empty");
            RuleFor(x => x.Unit).NotEmpty().WithMessage("Don vi san pham ko de trong (g; kg; ml; l)");
            RuleFor(x => x.Price).NotEmpty().Must(p => p > 100).WithMessage("Gia khong de trong va phai lon hon 100 VND");
            //RuleFor(x => x.CreatedAt).NotEmpty().WithMessage("CreatedAt la ngay them moi san pham");
            //RuleFor(x => x.CreatedBy).NotEmpty().WithMessage("CreateddBy la ten nguoi dung da thao tac len san pham moi");
            //RuleFor(x => x.UpdatedAt).NotEmpty().WithMessage("UpdateddAt field must not empty");
            //RuleFor(x => x.UpdatedBy).NotEmpty().WithMessage("UpdatedBy must not empty");
        }
    }
    //public class UpdateIngredientValidator : AbstractValidator<IngredientRequest>
    //{
    //    public UpdateIngredientValidator()
    //    {
    //        RuleFor(x => x.IngredientCategoryId).NotEmpty().WithMessage("Category khong de trong");
    //        RuleFor(x => x.Name).NotEmpty().WithMessage("Ten khong de trong");
    //        RuleFor(x => x.Img).NotEmpty().WithMessage("Image link khong de trong");
    //        RuleFor(x => x.Unit).NotEmpty().WithMessage("Don vi san pham");
    //        RuleFor(x => x.Price).NotEmpty().Must(p => p > 100).WithMessage("Don vi san pham ko de trong (g; kg; ml; l)");
    //        //RuleFor(x => x.CreatedAt).NotEmpty().WithMessage("CreatedAt la ngay them moi san pham");
    //        //RuleFor(x => x.CreatedBy).NotEmpty().WithMessage("CreateddBy la ten nguoi dung da thao tac len san pham moi");
    //        RuleFor(x => x.UpdatedAt).NotEmpty().WithMessage("UpdateddAt field must not empty");
    //        RuleFor(x => x.UpdatedBy).NotEmpty().WithMessage("UpdatedBy must not empty");
    //        RuleFor(x => x.Status).NotNull().WithMessage("Status la can thiet")
    //            .IsInEnum().WithMessage("phai la available hoac unavailable");
    //    }
    //}

    public class UpdateStatusIngredientValidator : AbstractValidator<UpdateStatusIngredientRequest>
    {
        public UpdateStatusIngredientValidator()
        {
            RuleFor(x => x.Status).NotNull().WithMessage("Status la can thiet")
                .IsInEnum().WithMessage("phai la available hoac unavailable");
        }
    }

    public class CreateIngredientCategoryValdator : AbstractValidator<CreateIngredientCategoryRequest>
    {
        public CreateIngredientCategoryValdator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Khong de trong ten cua Category cho ingredient");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Khong de trong mo ta danh cho categor cua ingredient");
            RuleFor(x => x.Status).NotNull().WithMessage("Status la can thiet")
                .IsInEnum().WithMessage("phai la available hoac unavailable");
        }
    }

    public class FullIngredientCategoryValdator : AbstractValidator<FullIngredientCategoryRequest>
    {
        public FullIngredientCategoryValdator()
        {
            //RuleFor(x => x.Id).NotEmpty().WithMessage("Guid khong de trong");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Khong de trong ten cua Category cho ingredient");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Khong de trong mo ta danh cho categor cua ingredient");
        }
    }

    public class CreateIngredientNutrientValidator : AbstractValidator<CreateIngredientNutrientRequest>
    {
        public CreateIngredientNutrientValidator()
        {
            //RuleFor(x => x.IngredientID).NotEmpty().WithMessage("Id khong de trong");
            RuleFor(x => x.Calories).GreaterThanOrEqualTo(0).WithMessage("Khong de trong cot Calories");
            RuleFor(x => x.Fat).GreaterThanOrEqualTo(0).WithMessage("Khong de trong cot Fat");
            RuleFor(x => x.SaturatedFat).GreaterThanOrEqualTo(0).WithMessage("Khong de trong cot SaturatedFat");
            RuleFor(x => x.Sugar).GreaterThanOrEqualTo(0).WithMessage("Khong de trong cot Sugar");
            RuleFor(x => x.Carbonhydrate).GreaterThanOrEqualTo(0).WithMessage("Khong de trong cot Carbonhydrate");
            RuleFor(x => x.DietaryFiber).GreaterThanOrEqualTo(0).WithMessage("Khong de trong cot DietaryFiber");
            RuleFor(x => x.Protein).GreaterThanOrEqualTo(0).WithMessage("Khong de trong cot Protein");
            RuleFor(x => x.Sodium).GreaterThanOrEqualTo(0).WithMessage("Khong de trong cot Sodium");
        }
    }

    public class FullIngredientNutrientValidator : AbstractValidator<IngredientNutrientRequest>
    {
        public FullIngredientNutrientValidator()
        {
            //RuleFor(x => x.Id).NotEmpty().WithMessage("Guid khong de trong");
            //RuleFor(x => x.IngredientID).NotEmpty().WithMessage("Id khong de trong");
            RuleFor(x => x.Calories).GreaterThanOrEqualTo(0).WithMessage("Khong de trong cot Calories");
            RuleFor(x => x.Fat).GreaterThanOrEqualTo(0).WithMessage("Khong de trong cot Fat");
            RuleFor(x => x.SaturatedFat).GreaterThanOrEqualTo(0).WithMessage("Khong de trong cot SaturatedFat");
            RuleFor(x => x.Sugar).GreaterThanOrEqualTo(0).WithMessage("Khong de trong cot Sugar");
            RuleFor(x => x.Carbonhydrate).GreaterThanOrEqualTo(0).WithMessage("Khong de trong cot Carbonhydrate");
            RuleFor(x => x.DietaryFiber).GreaterThanOrEqualTo(0).WithMessage("Khong de trong cot DietaryFiber");
            RuleFor(x => x.Protein).GreaterThanOrEqualTo(0).WithMessage("Khong de trong cot Protein");
            RuleFor(x => x.Sodium).GreaterThanOrEqualTo(0).WithMessage("Khong de trong cot Sodium");
        }
    }
}
