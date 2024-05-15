using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.RecipeCategoryModel
{
    public class CreateRecipeCategoryRequest
    {
        public Guid CategoryId { get; set; }
        public Guid RecipeId { get; set; }
    }
}
