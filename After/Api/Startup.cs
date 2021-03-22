using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Data;
using Microsoft.EntityFrameworkCore;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private readonly string corsPolicy = "defaultPolicy";


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(corsPolicy,
                builder =>
                {
                    builder.WithOrigins(Configuration["CorsUrl"])
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                });
            });

            //use "Bearer" as the authentication scheme
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    //the identity provider
                    options.Authority = "https://login.microsoftonline.com/b0ea34f0-13bb-49aa-a7f0-9f34136ad44e/v2.0";                                         
                    //the Application ID URI
                    options.Audience = "api://04a2312c-fa55-4825-8180-2238a796bebd";  
                    //allow token issuer from Client
                    options.TokenValidationParameters.ValidIssuer = "https://sts.windows.net/b0ea34f0-13bb-49aa-a7f0-9f34136ad44e/";
                    options.SaveToken = true;
                });
                 

            //add the db context with the connection string
            services.AddDbContextPool<AzureADAuthDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("AzureADAuthConn"))
            );
            services.AddScoped<ICustomerData, SqlCustomerData>();
            services.AddControllers();
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(corsPolicy);
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
