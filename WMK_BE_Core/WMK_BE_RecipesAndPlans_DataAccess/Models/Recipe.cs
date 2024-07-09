using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_RecipesAndPlans_DataAccess.Models
{
    [Table("Recipes")]
    public class Recipe
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
		public int ServingSize { get; set; }
		public LevelOfDifficult Difficulty { get; set; }
        public string? Description { get; set; }
        public string? Img { get; set; }
        public double Price { get; set; }
        public int Popularity { get; set; }
        public ProcessStatus ProcessStatus { get; set; }//thong tin ve viec duoc duyet hay chua.
                                                        //approve là đc duyet va co the hien thi tren app, deny hoac processing thi ko hien thi
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ApprovedAt { get; set; }
        public string? ApprovedBy { get; set; } = string.Empty;


        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; } = string.Empty;

        //reference
        public List<RecipePLan> RecipePlans { get; set; }
        public virtual List<RecipeIngredient> RecipeIngredients { get; set; }
        public virtual List<RecipeCategory> RecipeCategories { get; set; }
        public virtual List<RecipeStep> RecipeSteps { get; set; }
        public virtual RecipeNutrient RecipeNutrient { get; set; }
    }
}
