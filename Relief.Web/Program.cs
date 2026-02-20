using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Relief.Domain.Entities;
using Relief.Presistence.Data.DbContexts;
using Relief.Presistence.Repositories;
using Relief.ServiceAbstraction.Interfaces;
using Relief.Services.Implementations;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;


namespace Relief.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // -----------------------------
            // Controllers & Swagger
            // -----------------------------
            builder.Services.AddControllers()
               .AddJsonOptions(options =>
               {
                   options.JsonSerializerOptions.ReferenceHandler =
                       System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
               });
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Relief API",
                    Version = "v1",
                    Description = "Relief Care Platform API"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT token like this: Bearer {your token}"
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
                        Array.Empty<string>()
                    }
                });
            });

            // -----------------------------
            // Databases
            // -----------------------------
            builder.Services.AddDbContext<ReliefIdentityDbContext>(opt =>
                opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddDbContext<ReliefAppDbContext>(opt =>
                opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // -----------------------------
            // Identity
            // -----------------------------
            builder.Services
                .AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
                {
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<ReliefIdentityDbContext>()
                .AddDefaultTokenProviders();

            // -----------------------------
            // JWT Configuration
            // -----------------------------
            var jwt = builder.Configuration.GetSection("Jwt");
            var key = jwt["Key"];
            var issuer = jwt["Issuer"];
            var audience = jwt["Audience"];

            Console.WriteLine("VALIDATION KEY USED: " + key);

            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = issuer,
                        ValidAudience = audience,

                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(key!)
                        ),

                        RoleClaimType = ClaimTypes.Role,
                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine("AUTH FAILED: " + context.Exception.Message);
                            return Task.CompletedTask;
                        }
                    };
                });

            builder.Services.AddAuthorization();


            // -----------------------------
            // Dependency Injection
            // -----------------------------
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IOfferService, OfferService>();
            builder.Services.AddScoped<IJobOfferRepository, JobOfferRepository>();

            var app = builder.Build();

            await SeedRolesAsync(app);

            // -----------------------------
            // Middleware
            // -----------------------------
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();  // لازم قبل Authorization
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        // -----------------------------
        // Seed Roles
        // -----------------------------
        static async Task SeedRolesAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var roleManager = scope.ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            foreach (var role in new[] { "CareHome", "PSW" })
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>
                    {
                        Name = role,
                        NormalizedName = role.ToUpper()
                    });
                }
            }
        }
    }
}
