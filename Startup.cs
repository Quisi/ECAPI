using EnergyScanApi.Filters;
using EnergyScanApi.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace EnergyScanApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            if (System.Environment.GetEnvironmentVariable("SQL_IP") != null)
            {
                string[] constr = Configuration["ConnectionStrings:Default"].Split(";");
                constr[0] = "server=" + System.Environment.GetEnvironmentVariable("SQL_IP");
                string cst = "";
                foreach (string s in constr)
                {
                    cst += s + ";";
                }
                Configuration["ConnectionStrings:Default"] = cst;
                Configuration["ConnectionStrings:Docker"] = cst;
                Configuration["ConnectionStrings:Production"] = cst;
            }

            string connectionString = Configuration["ConnectionStrings:Default"];
            if (!Env.IsDevelopment())
            {
                MariaDbServerVersion serverVersion = new MariaDbServerVersion(ServerVersion.AutoDetect(Configuration["ConnectionStrings:Production"]));
                connectionString = Configuration["ConnectionStrings:Production"];
                services.AddDbContext<AppDb>(dbContextOptions => dbContextOptions
                .UseMySql(connectionString, serverVersion));
            } else
            {
                MariaDbServerVersion serverVersion = new MariaDbServerVersion(ServerVersion.AutoDetect(Configuration["ConnectionStrings:Default"]));
                services.AddDbContext<AppDb>(dbContextOptions => dbContextOptions
                .UseMySql(connectionString, serverVersion)
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors());
            }

            services
                .AddMvc(options =>
                {
                    options.InputFormatters.RemoveType<Microsoft.AspNetCore.Mvc.Formatters.SystemTextJsonInputFormatter>();
                    options.OutputFormatters.RemoveType<Microsoft.AspNetCore.Mvc.Formatters.SystemTextJsonOutputFormatter>();
                })
                .AddNewtonsoftJson(opts =>
                {
                    opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    opts.SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
                })
                .AddXmlSerializerFormatters();
            services
                .AddSwaggerGen(c =>
                {
                    c.EnableAnnotations();
                    
                    c.SwaggerDoc("1.0.0", new OpenApiInfo
                    {
                        Version = "1.0.0",
                        Title = "EnergyCanAPI",
                        Description = "EnergyCanAPI (ASP.NET Core 5.0)",
                        TermsOfService = new Uri("http://energycan.shadowsi.de/terms")
                    });
                    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\" Autorization via username and password",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                    });
                    c.CustomSchemaIds(type => type.FullName);
                    c.IncludeXmlComments($"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}{Env.ApplicationName}.xml");
                    c.OperationFilter<GeneratePathParamsValidationFilter>();
                    c.OperationFilter<SecurityRequirementsOperationFilter>();
                    c.OperationFilter<SwaggerFileOperationFilter>();
                });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, c =>
                    {
                        byte[] secret = Encoding.UTF8.GetBytes(Configuration["Authentification:Secret"]);
                        SymmetricSecurityKey key = new SymmetricSecurityKey(secret);
                        c.Events = new JwtBearerEvents()
                        {
                            //For url-access:jwttoken => /?access_token=...
                            OnMessageReceived = context =>
                            {
                                if (context.Request.Query.ContainsKey("access_token"))
                                {
                                    context.Token = context.Request.Query["access_token"];
                                }
                                return Task.CompletedTask;
                            }
                        };
                        c.TokenValidationParameters = new TokenValidationParameters()
                        {
                            ValidIssuer = Configuration.GetSection("Authentification").GetSection("Issuer").Value,
                            ValidAudience = Configuration.GetSection("Authentification").GetSection("Audience").Value,
                            IssuerSigningKey = key,
                        };
                    }
            );
            services.ConfigureApplicationCookie(o =>
            {
                o.ForwardDefault = JwtBearerDefaults.AuthenticationScheme;
            });
            // Replace the default authorization policy provider with our own
            // custom provider which can return authorization policies for given
            // policy names (instead of using the default policy provider)
            services.AddSingleton<IAuthorizationPolicyProvider, CorePolicyProvider>();

            // As always, handlers must be provided for the requirements of the authorization policies
            services.AddSingleton<IAuthorizationHandler, CoreAuthorizationHandler>();
            //services.AddScoped<IAuthorizationHandler, CoreAuthorizationHandler>();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }

            //app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/1.0.0/swagger.json", "EnergyScanApi"));
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
