
using Microsoft.EntityFrameworkCore;
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
            builder.Services.AddSwaggerGen();
            builder.Configuration.AddJsonFile("appsettings.json");
            //Add DBContext
            builder.Services.AddDbContext<WeMealKitContext>(ops =>
            {
                ops.UseSqlServer(builder.Configuration.GetConnectionString("DBConnect"),
                    b => b.MigrationsAssembly("WMK_BE_RecipesAndPlans_Controller"));
            });

            //Mapper
            builder.Services.AddAutoMapper(typeof(UserProfile));
            builder.Services.AddAutoMapper(typeof(CategoryProfile));
            builder.Services.AddAutoMapper(typeof(WeeklyPlanProfile));
            //builder.Services.AddAutoMapper(typeof(RecipeStepProfile));

            //scope
            builder.Services.AddScoped<DbContext , WeMealKitContext>();
            builder.Services.AddScoped<IUnitOfWork , UnitOfWork>();
            builder.Services.AddScoped<IUserService , UserService>();
            builder.Services.AddScoped<ICategoryService , CategoryService>();
            builder.Services.AddScoped<IWeeklyPlanService , WeeklyPlanService>();
            builder.Services.AddScoped<IRecipePlanService , RecipePlanService>();
            builder.Services.AddScoped<IRecipeStepService, RecipeStepService>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
                app.UseSwagger();
                app.UseSwaggerUI();
            //}
          
            app.UseAuthorization();
            app.UseDeveloperExceptionPage();

            app.MapControllers();

            app.Run();
        }
    }
}
