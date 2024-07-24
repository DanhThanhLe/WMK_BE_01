using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_RecipesAndPlans_DataAccess.Models
{
    public class WeMealKitContext : DbContext
    {
        public WeMealKitContext(DbContextOptions<WeMealKitContext> options) : base(options) { }
        public DbSet<Category> Categories { get; set; }
        public DbSet<WeeklyPlan> WeeklyPlans { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<IngredientCategory> IngredientCategorys { get; set; }
        public DbSet<IngredientNutrient> IngredientNutrients { get; set; }
        public DbSet<RecipeNutrient> RecipeNutrients { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeIngredient> RecipeAmounts { get; set; }
        public DbSet<RecipeCategory> RecipeCategories { get; set; }
        public DbSet<RecipePLan> RecipePLans { get; set; }
        public DbSet<RecipeStep> RecipeSteps { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderGroup> OrderGroups { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<RecipeIngredientOrderDetail> RecipeIngredientOrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed admin user
            var adminUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@gmail.com",
                UserName = "admin",
                PasswordHash = GetSignature256("Admin123@"),
                FirstName = "Admin",
                LastName = "No 1",
                Gender = Gender.Male,
                Phone = "",
                Role = Role.Admin,
                AccessFailedCount = 0,
                Status = BaseStatus.Available,
                EmailConfirm = EmailConfirm.Confirm,
            };

            var staff = new User
            {
                Id = Guid.NewGuid(),
                Email = "staff01@gmail.com",
                UserName = "staff",
                PasswordHash = GetSignature256("Admin123@"),
                FirstName = "Staff01",
                LastName = "No 1",
                Gender = Gender.Male,
                Phone = "",
                Role = Role.Staff,
                AccessFailedCount = 0,
                Status = BaseStatus.Available,
                EmailConfirm = EmailConfirm.Confirm,
            };

            var manager = new User
            {
                Id = Guid.NewGuid(),
                Email = "manager01@gmail.com",
                UserName = "manager",
                PasswordHash = GetSignature256("Admin123@"),
                FirstName = "Manager01",
                LastName = "No 1",
                Gender = Gender.Male,
                Phone = "",
                Role = Role.Manager,
                AccessFailedCount = 0,
                Status = BaseStatus.Available,
                EmailConfirm = EmailConfirm.Confirm,
            };

            modelBuilder.Entity<User>().HasData(adminUser);
            modelBuilder.Entity<User>().HasData(staff);
            modelBuilder.Entity<User>().HasData(manager);



            //===Ingredient(IngredientNutrient, IngredientCategory) | Recipe(Category, RecipeNutrient, RecipeIngredient)====================================

            ////quan hệ 1 - 1 giữa Ingredint và IngredientNutrient
            //modelBuilder.Entity<Ingredient>()
            //    .HasOne(i => i.IngredientNutrient)
            //    .WithOne(n => n.Ingredient)
            //    .HasForeignKey<IngredientNutrient>(n => n.IngredientID)
            //    .OnDelete(DeleteBehavior.Cascade);

            //// Quan hệ 1 - 1 giữa Recipe và RecipeNutrient
            //modelBuilder.Entity<Recipe>()
            //    .HasOne(r => r.RecipeNutrient)
            //    .WithOne(rn => rn.Recipe)
            //    .HasForeignKey<RecipeNutrient>(rn => rn.RecipeID)
            //    .OnDelete(DeleteBehavior.Cascade);

            //// Quan hệ 1 - nhiều giữa Recipe và RecipeStep
            //modelBuilder.Entity<Recipe>()
            //    .HasMany(r => r.RecipeSteps)
            //    .WithOne(rs => rs.Recipe)
            //    .HasForeignKey(rs => rs.RecipeId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //// Quan hệ 1 - nhiều giữa Recipe và RecipeIngredient
            //modelBuilder.Entity<Recipe>()
            //    .HasMany(r => r.RecipeIngredients)
            //    .WithOne(ri => ri.Recipe)
            //    .HasForeignKey(ri => ri.RecipeId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //// Quan hệ 1 - nhiều giữa Recipe và RecipeCategory
            modelBuilder.Entity<Recipe>()
                .HasMany(r => r.RecipeCategories)
                .WithOne(ri => ri.Recipe)
                .HasForeignKey(ri => ri.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            //// Quan hệ nhiều - nhiều giữa Recipe và Ingredient thông qua RecipeIngredient
            //modelBuilder.Entity<RecipeIngredient>()
            //    .HasOne(ri => ri.Recipe)
            //    .WithMany(r => r.RecipeIngredients)
            //    .HasForeignKey(ri => ri.RecipeId);

            //modelBuilder.Entity<RecipeIngredient>()
            //    .HasOne(ri => ri.Ingredient)
            //    .WithMany(i => i.RecipeIngredients)
            //    .HasForeignKey(ri => ri.IngredientId);

            ////===Ingredient(IngredientNutrient, IngredientCategory) | Recipe(Category, RecipeNutrient, RecipeIngredient, RecipeStep)====================================



            ////===Recipe() | WeeklyPlan()=============================================================================================

            //// Quan hệ nhiều - nhiều giữa Recipe và WeeklPlan thông qua RecipePlan
            //modelBuilder.Entity<RecipePLan>()
            //    .HasOne(rp => rp.Recipe)
            //    .WithMany(r => r.RecipePlans)
            //    .HasForeignKey(rp => rp.RecipeId);

            //modelBuilder.Entity<RecipePLan>()
            //    .HasOne(rp => rp.WeeklyPlan)
            //    .WithMany(wp => wp.RecipePLans)
            //    .HasForeignKey(rp => rp.StandardWeeklyPlanId);

            ////// CustomPlan and RecipePlan relationship
            ////modelBuilder.Entity<RecipePLan>()
            ////    .HasOne(rp => rp.CustomPlan)
            ////    .WithMany(cp => cp.RecipePlans)
            ////    .HasForeignKey(rp => rp.CustomPlanId);

            //// Quan hệ 1 - nhiều của 


            ////===Recipe() | WeeklyPlan()=============================================================================================

            ////===recipe - order - customPlan============================================================================
            ////modelBuilder.Entity<CustomPlan>()
            ////    .HasOne(cp => cp.Recipe)
            ////    .WithMany(r => r.CustomPlans)
            ////    .HasForeignKey(cp => cp.RecipeId)
            ////    .OnDelete(DeleteBehavior.Restrict);//xoa recipe thi

            //modelBuilder.Entity<WeeklyPlan>()
            //    .HasMany(w => w.RecipePLans)
            //    .WithOne(rp => rp.WeeklyPlan)
            //    .HasForeignKey(rp => rp.StandardWeeklyPlanId)
            //    .OnDelete(DeleteBehavior.Cascade);






            //===recipe - weeklyPlan - order - customPlan=============================================================================

        }

        private string GetSignature256(String text)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));

                // Convert the byte array to a string representation.
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseLazyLoadingProxies();


        }

    }

}
