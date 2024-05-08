using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_RecipesAndPlans_DataAccess.Models
{
    public class RecipesAndPlansContext:DbContext
    {
        public RecipesAndPlansContext(DbContextOptions<RecipesAndPlansContext> options) : base(options) { }
        public DbSet<Category> Categories { get; set; }
        public DbSet<WeeklyPlan> WeeklyPlans { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Nutrition> Nutritions { get; set;}
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeAmount> RecipeAmounts { get; set;}
        public DbSet<RecipeCategory> RecipeCategories { get; set; }
        public DbSet<RecipePLan> RecipePLans { get; set;}
        public DbSet<RecipeStep> RecipeSteps { get; set; }

       

    }
}
