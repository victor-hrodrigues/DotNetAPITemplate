using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Template
{
    public class Startup
    {
        public IServiceCollection Services { get; private set; }

        public static string ConnectionString;
        public static string WebApiSecret;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Services = services;

            Services.AddCors();
            Services.AddControllers();

            WebApiSecret = Configuration.GetValue<string>("Secret");

            RegisterDatabase();

            RegisterAuthentication();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors(cors => cors
                   .AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader());
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public void RegisterDatabase()
        {
            var connectionStringValues = Configuration.GetConnectionString("ConnectionString")?.Split(';');
            var connectionStringBuilder = new SqlConnectionStringBuilder();

            if (!(connectionStringValues is null) && connectionStringValues.Length > 0)
            {
                connectionStringBuilder.DataSource = connectionStringValues[0];
                connectionStringBuilder.UserID = connectionStringValues[1];
                connectionStringBuilder.Password = connectionStringValues[2];
                connectionStringBuilder.InitialCatalog = connectionStringValues[3];

                ConnectionString = connectionStringBuilder.ConnectionString;
            }
        }

        public void RegisterAuthentication()
        {
            var key = Encoding.ASCII.GetBytes(WebApiSecret);

            Services.AddAuthentication(config =>
            {
                config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwtBearer =>
            {
                jwtBearer.RequireHttpsMetadata = false;
                jwtBearer.SaveToken = true;
                jwtBearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }
    }
}
