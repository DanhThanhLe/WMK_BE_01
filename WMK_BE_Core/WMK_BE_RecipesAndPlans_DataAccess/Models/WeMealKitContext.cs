using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

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

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Seed admin user
			var adminUser = new User
			{
				Id = Guid.NewGuid(),
				Email = "admin@gmail.com",
				UserName = "admin",
				PasswordHash = GetSignature256("Admin123@") ,
				FirstName = "Admin" ,
				LastName = "No 1" ,
				Gender = Gender.Male ,
				Phone = "",
				Role = Role.Admin ,
				AccessFailedCount = 0 ,
				Status = BaseStatus.Available ,
				EmailConfirm = EmailConfirm.Confirm ,
			};

			modelBuilder.Entity<User>().HasData(adminUser);
		}
		private string GetSignature256(String text)
		{
			using ( SHA256 sha256 = SHA256.Create() )
			{
				// Convert the input string to a byte array and compute the hash.
				byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));

				// Convert the byte array to a string representation.
				StringBuilder builder = new StringBuilder();
				for ( int i = 0; i < bytes.Length; i++ )
				{
					builder.Append(bytes[i].ToString("x2"));
				}
				return builder.ToString();
			}
		}
	}

}
