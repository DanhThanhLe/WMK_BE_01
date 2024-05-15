using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.BusinessModel.ResponseModel.Recipe
{
    public class RecipeResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ServingSize { get; set; } = string.Empty;
        public LevelOfDifficult Difficulty { get; set; }
        public string? Description { get; set; }
        public string? ImageLink { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ApprovedAt { get; set; }
        public string? ApprovedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; } = string.Empty;
        public int Popularity { get; set; }
        public ProcessStatus ProcessStatus { get; set; }

        //public List<RecipeAmount> recipeAmounts { get; set; }
    }
}
