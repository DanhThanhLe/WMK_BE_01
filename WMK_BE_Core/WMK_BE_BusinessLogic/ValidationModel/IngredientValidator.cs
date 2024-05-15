using FluentValidation;
using FluentValidation.AspNetCore;

using WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientModel;

namespace WMK_BE_BusinessLogic.ValidationModel
{
    public class IngredientValidator : AbstractValidator<CreateIngredientRequest>
    {
        public IngredientValidator()
        {
            RuleFor(x => x.Category).NotEmpty().WithMessage("Category khong de trong");

            RuleFor(x => x.Name).NotEmpty().WithMessage("Ten khong de trong");

            RuleFor(x => x.PricebyUnit)
                .NotEmpty().WithMessage("Price per Unit khong nen de trong")
                .GreaterThan(1000).WithMessage("Gia don vi phai it nhat 1000 VND");
            //RuleFor(x => x.ImageLink).NotEmpty().WithMessage("Image link must not empty");

            RuleFor(x => x.Unit).NotEmpty().WithMessage("Don vi san pham");
            //RuleFor(x => x.AmountUishow).NotEmpty().WithMessage("AmountUiShow must not empty");
            RuleFor(x => x.CreatedAt).NotEmpty().WithMessage("CreatedAt la ngay them moi san pham");
            RuleFor(x => x.CreatedBy).NotEmpty().WithMessage("CreateddBy la ten nguoi dung da thao tac len san pham moi");
            //RuleFor(x => x.UpdatedAt).NotEmpty().WithMessage("UpdateddAt field must not empty");
            //RuleFor(x => x.UpdatedBy).NotEmpty().WithMessage("UpdatedBy must not empty");
        }
    }
    public class UpdateIngredientValidator : AbstractValidator<IngredientRequest>
    {
        public UpdateIngredientValidator()
        {
            RuleFor(x => x.Category).NotEmpty().WithMessage("Category khong de trong");

            RuleFor(x => x.Name).NotEmpty().WithMessage("Ten khong de trong");

            RuleFor(x => x.PricebyUnit)
                .NotEmpty().WithMessage("Price per Unit khong nen de trong")
                .GreaterThan(1000).WithMessage("Gia don vi phai it nhat 1000 VND");
            //RuleFor(x => x.ImageLink).NotEmpty().WithMessage("Image link must not empty");

            RuleFor(x => x.Unit).NotEmpty().WithMessage("Don vi san pham");
            //RuleFor(x => x.AmountUishow).NotEmpty().WithMessage("AmountUiShow must not empty");
            RuleFor(x => x.CreatedAt).NotEmpty().WithMessage("CreatedAt la ngay them moi san pham");
            RuleFor(x => x.CreatedBy).NotEmpty().WithMessage("CreateddBy la ten nguoi dung da thao tac len san pham moi");
            RuleFor(x => x.UpdatedAt).NotEmpty().WithMessage("UpdateddAt field must not empty");
            RuleFor(x => x.UpdatedBy).NotEmpty().WithMessage("UpdatedBy must not empty");
        }
    }

    public class UpdateStatusIngredientValidator : AbstractValidator<UpdateStatusIngredientRequest>
    {
        public UpdateStatusIngredientValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("ID khong de trong");
            RuleFor(x => x.Status).NotNull().WithMessage("Status la can thiet")
                .IsInEnum().WithMessage("must be available or unavailable");
        }
    }

    public class IdValidator : AbstractValidator<IdIngredientRequest>
    {
        public IdValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("ID khong de trong");
        }
    }
}
