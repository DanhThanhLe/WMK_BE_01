using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.WeeklyPlanModel;

namespace WMK_BE_BusinessLogic.ValidationModel
{
	public class CreateWeeklyPlanValidator : AbstractValidator<CreateWeeklyPlanRequest> //dung cho create new week plan (staff tao)
	{
		public CreateWeeklyPlanValidator()
		{
			//RuleFor(x => x.CreatedBy).NotEmpty().WithMessage("Create by is required!");
			RuleFor(x => x.BeginDate)
				.NotEmpty()
				.Must(beginDate => beginDate >= DateTime.UtcNow.AddHours(7))
				.WithMessage("Begin date must not be in past");
			RuleFor(x => x.EndDate)
			.NotEmpty().WithMessage("EndDate is required!")
			//.When(x => x.EndDate != null)
			.Must((model , endDate) => endDate > model.BeginDate)
			.WithMessage("EndDate must be after BeginDate!");
		}
	}

	//public class CreateWeeklyPlanForUserValidator : AbstractValidator<CreateWeeklyPlanRequest> //dung cho create new plan (customer tao)
	//{
	//    public CreateWeeklyPlanForUserValidator()
	//    {
	//        RuleFor(x => x.CreatedBy).NotEmpty().WithMessage("CreateBy is required!");
	//    }
	//}

	public class UpdateWeeklyPlanValidator : AbstractValidator<UpdateWeeklyPlanRequestModel>
	{
		public UpdateWeeklyPlanValidator()
		{
			//RuleFor(x => x.Id).NotEmpty().WithMessage("Id by is required!");
			RuleFor(x => x.BeginDate)
				.NotEmpty()
				.Must(beginDate => beginDate >= DateTime.UtcNow.AddHours(7))
				.WithMessage("Begin date must not be in past");
			RuleFor(x => x.EndDate)
			.NotEmpty().WithMessage("EndDate is required!")
			//.When(x => x.EndDate != null)
			.Must((model , endDate) => endDate > model.BeginDate)
			.WithMessage("EndDate must be after BeginDate!");
		}
	}
	public class DeleteWeeklyPlanValidator : AbstractValidator<DeleteWeeklyPlanRequestModel>
	{
		public DeleteWeeklyPlanValidator()
		{
			RuleFor(x => x.Id).NotEmpty().WithMessage("Id by is required!");
		}
	}
	public class ChangeStatusWeeklyPlanValidator : AbstractValidator<ChangeStatusWeeklyPlanRequest>
	{
		public ChangeStatusWeeklyPlanValidator()
		{
			RuleFor(x => x.ProcessStatus).NotNull().WithMessage("Status is required!");
		}
	}
}
