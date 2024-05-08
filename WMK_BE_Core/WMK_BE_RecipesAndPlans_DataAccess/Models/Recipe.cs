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
        public string Style { get; set; }
        public string Name { get; set; }
        public string ServingSize { get; set; }
        public string? Difficulty { get; set; }
        public string? Description { get; set; }
        public Guid NutritionId { get; set; }
        public string? ImageLink { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public Guid? ApprovedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public int Popularity { get; set; }
        public ProcessStatus ProcessStatus { get; set; }//thong tin ve viec duoc duyet hay chua.
                                                        //approve là đc duyet va co the hien thi tren app, deny hoac processing thi ko hien thi

        //reference
        public List<RecipePLan> RecipePlans { get; set; }
        public List<RecipeCategory> RecipeCategories { get; set; }
        public virtual Nutrition Nutrition { get; set; }
    }
}
