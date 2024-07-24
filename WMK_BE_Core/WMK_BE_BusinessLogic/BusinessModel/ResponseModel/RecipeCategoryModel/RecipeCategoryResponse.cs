using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.CategoryModel;

namespace WMK_BE_BusinessLogic.BusinessModel.ResponseModel.RecipeCategoryModel
{
    public class RecipeCategoryResponse
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public Guid RecipeId { get; set; }
        public CategoryResponseModel Category { get; set; }
    }
}
/*
 public Guid Id { get; set; }
        [ForeignKey(nameof(Category))]
        public Guid CategoryId {  get; set; }
        [ForeignKey(nameof(Recipe))]
        public Guid RecipeId { get; set; }
 */