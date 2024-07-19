using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.RecipeStepModel.RecipeStep
{
    public class RecipeStepRequest
    {
        public Guid Id { get; set; }
        public Guid RecipeId { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
        public string? MediaURL { get; set; } = string.Empty;
        public string? ImageLink { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
    }

    public class CreateRecipeStepRequest
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string? MediaURL { get; set; } = string.Empty;
        public string? ImageLink { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
    }
}
