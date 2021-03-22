using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using WebApp.HttpHandler;

namespace WebApp
{
    public class Startup
    {       
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            //make HttpContextAccessor available
            services.AddHttpContextAccessor();

            //make http bearer token handler available
            services.AddTransient<BearerTokenHandler>();

            //add http client
            services.AddHttpClient("API", client =>
            {
                client.BaseAddress = new Uri(this.Configuration["APIurl"]);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            }).AddHttpMessageHandler<BearerTokenHandler>();    //add the bearer token in request


            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                //set path to access denied from authorization
                options.AccessDeniedPath = "/AccessDenied";
            })
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                //the identity provider
                options.Authority = "https://login.microsoftonline.com/b0ea34f0-13bb-49aa-a7f0-9f34136ad44e/v2.0";
                //this application
                options.ClientId = "232c4cc4-bc82-4b4b-addc-ee689efec83b";
                options.ClientSecret = "gowrq2SNgbe.XhGGh5a9Nm-4DPi.o9__OI";
                //OpenId Connect flow
                options.ResponseType = "code";
                //the "code" flow will use PKCE by default, below line is not required but we can be explicit
                options.UsePkce = true;
                //"openid" and "profile" scopes are included by default by the middleware, it's not required but we can be explicit
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                //add the scope to access the API
                options.Scope.Add("api://04a2312c-fa55-4825-8180-2238a796bebd/AccessCustomer"); 
                options.SaveTokens = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
