using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Vectarr.Palooza.Context;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Vectarr.Palooza.Model;
using Vectarr.Palooza.ApiBusiness.IServices.IUserServices;
using Vectarr.Palooza.ApiBusiness.Services.UserServices;
using Vectarr.Palooza.ApiBusiness.IServices.IWebSenders;
using Vectarr.Palooza.ApiBusiness.Services.WebSenders;
using Vectarr.Palooza.ApiBusiness.IServices;
using Vectarr.Palooza.ApiBusiness.Services.ImageServices;
using Microsoft.OpenApi.Models;
using Vectarr.Palooza.ApiBusiness.Emailing;
using Microsoft.Extensions.Configuration;
using Vectarr.Palooza.Digifabster.Classes;
using Vectarr.Palooza.Digifabster;
using Hangfire;
using Hangfire.PostgreSql;
using Vectarr.Palooza.ApiBusiness.Services.BlogServices;

namespace Vectarr.Palooza.WebApi
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
            var jwtTokenConfig = Configuration.GetSection("JwtConfig").Get<JwtConfig>();
            services.AddSingleton(jwtTokenConfig);

            services.AddDbContext<VectarrPostgresDBContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("ProdDefaultConnection")));

            //services.AddDbContext<VectarrDbContext>(options =>
            //   options.UseSqlServer(Configuration.GetConnectionString("ProdDefaultConnection")));

            services.Configure<IdentityOptions>(options =>
            {
                // Default Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;
                options.SignIn.RequireConfirmedEmail = true;
            });

            services.AddMvcCore()
              .AddAuthorization();

            services.AddControllers(options =>
            {
                options.Filters.Add<ErrorValidationExceptionFilter>();
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwt =>
            {
                var key = Encoding.ASCII.GetBytes(Configuration["JwtConfig:Secret"]);

                jwt.SaveToken = true;
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                };
            });


            services.AddSingleton<IJwtTokenService, JwtTokenService>();
            //services.AddHostedService<StatusUpdateService>();
            services.AddSingleton<IWebRequestSender, WebRequestSender>();
            services.AddSingleton<IEmailSender, Smtp2GoEmailSender>();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IFileManager, LocalStorageManager>();
            services.AddScoped<IImageManager, ImageManager>();
            services.AddScoped<UserAuthenticationServices>();

            services.AddDefaultIdentity<UserAccount>(options => options.SignIn.RequireConfirmedAccount = false)
             .AddRoles<Model.IdentityRole>()  // <-- Add this line to enable role support
             .AddEntityFrameworkStores<VectarrPostgresDBContext>();

            services.AddSwaggerGen();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
            });

            services.Configure<DigifabsterOptions>(Configuration.GetSection("Digifabster"));

            var digifabsterBaseUrl = Configuration["Digifabster:BaseUrl"]!.TrimEnd('/');

            services.AddHttpClient<IDigifabsterAuthProvider, DigifabsterAuthProvider>(client =>
            {
                client.BaseAddress = new Uri(digifabsterBaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            });

            services.AddHttpClient<IDigifabsterService, DigifabsterService>(client =>
            {
                client.BaseAddress = new Uri(digifabsterBaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            });

            services.AddScoped<VectarrClientService>();

            // Hangfire
            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(options =>
                    options.UseNpgsqlConnection(Configuration.GetConnectionString("LocalDefaultConnection"))));
            services.AddHangfireServer();

            // Blog Generator
            services.AddScoped<BlogGeneratorService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Vectarr",
                    Version = "v1",
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("AllowAllOrigins");

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
                app.UseCors("CorsPolicy");
            }

            //app.UseMiddleware<ApiKeyMiddleware>();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vectarr");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHangfireDashboard();
            });

            // Schedule blog generation every 2 minutes (testing) — change to "0 6 * * *" for daily at 6AM UTC in production
            RecurringJob.AddOrUpdate<BlogGeneratorService>(
                "daily-blog-generator",
                service => service.GenerateDailyBlogs(),
                "0 9,21 * * *",
                new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });
        }
    }
}