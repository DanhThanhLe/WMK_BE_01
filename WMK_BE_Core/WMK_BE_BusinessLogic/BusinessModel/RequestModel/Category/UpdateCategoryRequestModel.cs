using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.Category
{
	public class UpdateCategoryRequestModel
	{
		public Guid Id { get; set; }
		public string? Name { get; set; } = string.Empty;
		public string? Description { get; set; } = string.Empty;
		public CategoryStatus? Status { get; set; }

	}
}
