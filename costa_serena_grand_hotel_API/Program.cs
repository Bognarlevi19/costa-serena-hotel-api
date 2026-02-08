using costa_serena_grand_hotel_API.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;

namespace costa_serena_grand_hotel_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<HotelDbContext>(options =>
               options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")!));
           
            
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                 options.UseSqlite(builder.Configuration.GetConnectionString("IdentityConnection")));

            builder.Services.AddDefaultIdentity<IdentityUser>(options =>

            {

                options.SignIn.RequireConfirmedAccount = false;

            })

            .AddEntityFrameworkStores<ApplicationDbContext>();


            builder.Services.AddRazorPages(); // Identity UI-hoz kell
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();
            app.MapRazorPages();
            app.Run();
        }
    }
}
