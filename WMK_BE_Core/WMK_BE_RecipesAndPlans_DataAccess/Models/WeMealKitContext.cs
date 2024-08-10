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

			SeedData();
		}
		public void SeedData()
		{
			// Check if the admin user already exists
			if ( !Users.Any(u => u.Email == "admin@gmail.com") )
			{
				Users.Add(new User
				{
					Id = Guid.NewGuid() ,
					Email = "admin@gmail.com" ,
					UserName = "admin" ,
					PasswordHash = GetSignature256("Admin123@") ,
					FirstName = "Admin" ,
					LastName = "No 1" ,
					Gender = Gender.Male ,
					Phone = "" ,
					Role = Role.Admin ,
					AccessFailedCount = 0 ,
					Status = BaseStatus.Available ,
					EmailConfirm = EmailConfirm.Confirm ,
				});
			}

			// Check if the staff user already exists
			if ( !Users.Any(u => u.Email == "staff01@gmail.com") )
			{
				Users.Add(new User
				{
					Id = Guid.NewGuid() ,
					Email = "staff01@gmail.com" ,
					UserName = "staff" ,
					PasswordHash = GetSignature256("Staff123@") ,
					FirstName = "Staff01" ,
					LastName = "No 1" ,
					Gender = Gender.Male ,
					Phone = "" ,
					Role = Role.Staff ,
					AccessFailedCount = 0 ,
					Status = BaseStatus.Available ,
					EmailConfirm = EmailConfirm.Confirm ,
				});
			}

			// Check if the manager user already exists
			if ( !Users.Any(u => u.Email == "manager01@gmail.com") )
			{
				Users.Add(new User
				{
					Id = Guid.NewGuid() ,
					Email = "manager01@gmail.com" ,
					UserName = "manager" ,
					PasswordHash = GetSignature256("Manager123@") ,
					FirstName = "Manager01" ,
					LastName = "No 1" ,
					Gender = Gender.Male ,
					Phone = "" ,
					Role = Role.Manager ,
					AccessFailedCount = 0 ,
					Status = BaseStatus.Available ,
					EmailConfirm = EmailConfirm.Confirm ,
				});
			}

			SaveChanges();
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

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			//optionsBuilder.UseLazyLoadingProxies();


		}

	}

}
