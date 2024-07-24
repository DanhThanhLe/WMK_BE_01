
using Amazon.S3;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Reflection;
using System.Text;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.TransactionModel;
using WMK_BE_BusinessLogic.Mapper;
using WMK_BE_BusinessLogic.Service.Implement;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Implement;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_RecipesAndPlans_Controller
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers();

			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Configuration.AddJsonFile("appsettings.json");
			
			//Add DBContext
			builder.Services.AddDbContext<WeMealKitContext>(ops =>
			{
				ops.UseSqlServer(builder.Configuration.GetConnectionString("DBConnect") ,
					b => b.MigrationsAssembly("WMK_BE_RecipesAndPlans_Controller"));
			}, ServiceLifetime.Transient);

            //add swagger
            //I want get date time of this file to show version of API 
            var fileInfo = new FileInfo("WMK_BE_RecipesAndPlans_Controller.dll");
            var version = fileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");

            builder.Services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1" , new OpenApiInfo { Title = "WeMealKit" , Version = "v1" , Description = version });
				c.AddSecurityDefinition("Bearer" , new OpenApiSecurityScheme
				{
					In = ParameterLocation.Header ,
					Description = "Please enter a valid token" ,
					Name = "Authorization" ,
					Type = SecuritySchemeType.Http ,
					BearerFormat = "JWT" ,
					Scheme = "Bearer"
				});
				c.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							}
						},
						new string[]{ }
					}
				});
			});

            //CORS

            //builder.Services.AddCors(options =>
            //{
            //    options.AddPolicy("AllowAllOrigins",
            //        policyBuilder => policyBuilder
            //            .AllowAnyOrigin()
            //            .AllowAnyMethod()
            //            .AllowAnyHeader());
            //});


            //JWT
            builder.Services.AddAuthentication(op =>
			{
				op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				op.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(op =>
			{
				op.SaveToken = true;
				op.RequireHttpsMetadata = false;
				op.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true ,
					ValidateAudience = true ,
					ValidateLifetime = true ,
					ValidateIssuerSigningKey = true ,
					ValidAudience = builder.Configuration["JWT:ValidAudience"] ,
					ValidIssuer = builder.Configuration["JWT:ValidIssuer"] ,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:IssuerSigningKey"]))
				};
			});

			// Configure authorization policies
			builder.Services.AddAuthorization(options =>
			{
				options.AddPolicy("AdminPolicy" , policy =>
				{
					policy.RequireAuthenticatedUser();
					policy.RequireRole("Admin");
				});

				options.AddPolicy("ManagerPolicy" , policy =>
				{
					policy.RequireAuthenticatedUser();
					policy.RequireRole("Manager");
				});

				options.AddPolicy("StaffPolicy" , policy =>
				{
					policy.RequireAuthenticatedUser();
					policy.RequireRole("Staff");
				});

				options.AddPolicy("DeliveryPolicy" , policy =>
				{
					policy.RequireAuthenticatedUser();
					policy.RequireRole("Delivery");
				});

				options.AddPolicy("CustomerPolicy" , policy =>
				{
					policy.RequireAuthenticatedUser();
					policy.RequireRole("Customer");
				});
			});


			//Mapper
			builder.Services.AddAutoMapper(typeof(UserProfile));
			builder.Services.AddAutoMapper(typeof(OrderProfile));
			builder.Services.AddAutoMapper(typeof(CategoryProfile));
			builder.Services.AddAutoMapper(typeof(WeeklyPlanProfile));
			builder.Services.AddAutoMapper(typeof(RecipeStepProfile));
			builder.Services.AddAutoMapper(typeof(RecipeCategoryProfile));
			builder.Services.AddAutoMapper(typeof(RecipeProfile));
			builder.Services.AddAutoMapper(typeof(OrderGroupProfile));
			builder.Services.AddAutoMapper(typeof(RecipeIngredientProfile));
			builder.Services.AddAutoMapper(typeof(RecipeNutrientProfile));
            builder.Services.AddAutoMapper(typeof(IngredientCategoryProfile));
			builder.Services.AddAutoMapper(typeof(IngredientNutrientProfile));
			builder.Services.AddAutoMapper(typeof(IngredientProfile));
			builder.Services.AddAutoMapper(typeof(RecipePlanProfile));
            builder.Services.AddAutoMapper(typeof(OrderDetailProfile));

            //scope
            builder.Services.AddScoped<DbContext , WeMealKitContext>();
			builder.Services.AddScoped<IUnitOfWork , UnitOfWork>();
			builder.Services.AddScoped<IAuthService , AuthService>();
			builder.Services.AddScoped<ISendMailService , SendMailService>();
			builder.Services.AddScoped<IUserService , UserService>();
			builder.Services.AddScoped<IOrderService , OrderService>();
			builder.Services.AddScoped<ICategoryService , CategoryService>();
			builder.Services.AddScoped<IWeeklyPlanService , WeeklyPlanService>();
			builder.Services.AddScoped<IRecipeService , RecipeService>();
			builder.Services.AddScoped<IRecipeStepService , RecipeStepService>();
			builder.Services.AddScoped<IRecipeCategoryService , RecipeCategoryService>();
			builder.Services.AddScoped<IRecipePlanService , RecipePlanService>();
			builder.Services.AddScoped<IRecipeIngredientService , RecipeIngredientService>();
			builder.Services.AddScoped<IRecipeCategoryService , RecipeCategoryService>();
			builder.Services.AddScoped<IRecipeNutrientService, RecipeNutrientService>();
			builder.Services.AddScoped<IIngredientService , IngredientService>();
            builder.Services.AddScoped<IIngredientCategoryService, IngredientCategoryService>();
            builder.Services.AddScoped<IIngredientNutrientService, IngredientNutrientService>();
			builder.Services.AddScoped<IOrderGroupService , OrderGroupService>();
			builder.Services.AddScoped<ITransactionService , TransactionService>();
            builder.Services.AddScoped<IOrderDetailService, OrderDetailService>();
			builder.Services.AddScoped<IRecipeIngredientOrderDetailService, RecipeIngredientOrderDetailService>();
            builder.Services.Configure<MomoOption>(builder.Configuration.GetSection("MomoAPI"));

            var app = builder.Build();

			// Configure the HTTP request pipeline.
			//long.nguyen mo swagger for production
			//if ( app.Environment.IsDevelopment() )
			//{
			app.UseSwagger();
			app.UseSwaggerUI();
            //}
            //app.UseCors("AllowAllOrigins"); //cau hinh CORs
            app.UseAuthentication();
			app.UseAuthorization();
			app.MapControllers();

			app.Run();
		}
	}
}
