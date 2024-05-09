
using Microsoft.EntityFrameworkCore;
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
            builder.Services.AddDbContext<RecipesAndPlansContext>(ops =>
            {
                ops.UseSqlServer(builder.Configuration.GetConnectionString("DBConnect"),
                    b => b.MigrationsAssembly("WMK_BE_RecipesAndPlans_Controller"));
            });


            //scope
            builder.Services.AddScoped<DbContext , RecipesAndPlansContext>();
            builder.Services.AddScoped<IUnitOfWork , UnitOfWork>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
