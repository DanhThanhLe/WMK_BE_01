using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_RecipesAndPlans_DataAccess.Models
{
    public class WeMealKitContext:DbContext
    {
        public WeMealKitContext(DbContextOptions<WeMealKitContext> options) : base(options) { }
        public DbSet<Category> Categories { get; set; }
        public DbSet<WeeklyPlan> WeeklyPlans { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Nutrition> Nutritions { get; set;}
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeAmount> RecipeAmounts { get; set;}
        public DbSet<RecipeCategory> RecipeCategories { get; set; }
        public DbSet<RecipePLan> RecipePLans { get; set;}
        public DbSet<RecipeStep> RecipeSteps { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<CustomeWeeklyPlan> CustomeWeeklyPlans { get; set; }
        public DbSet<BankingInfo> BankingInfos { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
    }
}
