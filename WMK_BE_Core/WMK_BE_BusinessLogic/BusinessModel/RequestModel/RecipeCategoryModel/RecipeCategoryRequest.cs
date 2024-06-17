using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.RecipeCategoryModel
{
    public class RecipeCategoryRequest
    {
        //public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public Guid RecipeId { get; set; }
    }
}
