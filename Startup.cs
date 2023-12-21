using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLINK.Application.MappingProfilers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MyApp.Application.Interfaces;
using MyApp.Infrastructure.Repositories;


namespace BLINK.Driver.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {

      services.AddAutoMapper(typeof(DriverConfig));

      var connectionString = Configuration.GetConnectionString("DefaultConnection");
      InitializeDatabase(connectionString);
      services.AddScoped<IDriverService, DriverService>();
      services.AddScoped<IRepository<MyApp.Domain.Entities.Driver>>(provider =>
            new SqliteRepository<MyApp.Domain.Entities.Driver>(connectionString));

      services.AddScoped<IUnitOfWork>(provider => new SqliteUnitOfWork(connectionString));

      services.AddControllers();
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
      });
    }


    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
      app.UseSwagger();


      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
      });

      app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    private void InitializeDatabase(string connectionString)
    {
      try
      {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var tableCmd = connection.CreateCommand();
        tableCmd.CommandText =
        @"
            CREATE TABLE IF NOT EXISTS Drivers (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                FirstName TEXT NOT NULL,
                LastName TEXT NOT NULL,
                Email TEXT NOT NULL,
                PhoneNumber TEXT NOT NULL
            )
        ";
        tableCmd.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        // Log the exception
        Console.WriteLine("Error initializing database: " + ex.Message);
        throw; // Re-throw the exception to make it visible
      }
    }


  }
}
