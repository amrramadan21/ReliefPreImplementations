using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.SimpleEmailV2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Relief.Domain.Contracts;
using Relief.Domain.Entities.Users;
using Relief.Presentation.Middleware;
using Relief.Presistence;
using Relief.Presistence.Data.DbContexts;
using Relief.Presistence.Repositories;
using Relief.ServiceAbstraction.Interfaces.Admin;
using Relief.ServiceAbstraction.Interfaces.Applications;
using Relief.ServiceAbstraction.Interfaces.Files;
using Relief.ServiceAbstraction.Interfaces.Offers;
using Relief.ServiceAbstraction.Interfaces.Profiles;
using Relief.ServiceAbstraction.Interfaces.Users;
using Relief.Services.Implementations.Admin;
using Relief.Services.Implementations.Applications;
using Relief.Services.Implementations.Offers;
using Relief.Services.Implementations.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

namespace Relief.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //// ✅ Debug: Check if config is loaded
            //Console.WriteLine("=== AWS CONFIG DEBUG ===");
            //Console.WriteLine($"Region: {builder.Configuration["AWS:Region"]}");
            //Console.WriteLine($"SESFromEmail: {builder.Configuration["AWS:SESFromEmail"]}");
            //Console.WriteLine($"Access Key: {accessKey}");
            //Console.WriteLine($"Secret Key Length: {secretKey?.Length}");
            //Console.WriteLine($"Access Key starts with: {accessKey?.Substring(0, 8)}");
            //Console.WriteLine("========================");

            // -----------------------------
            // CORS
            // -----------------------------
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });


            // -----------------------------
            // Controllers & Swagger
            // -----------------------------
            builder.Services.AddControllers()
               .AddJsonOptions(options =>
               {
                   options.JsonSerializerOptions.ReferenceHandler =
                       System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                   options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
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
                .AddEntityFrameworkStores<ReliefAppDbContext>()
                .AddDefaultTokenProviders();

            // -----------------------------
            // Aws SES
            // -----------------------------
            var awsOptions = new AWSOptions
            {
                Region = Amazon.RegionEndpoint.CACentral1
            };

            //builder.Services.AddDefaultAWSOptions(awsOptions);
            builder.Services.AddAWSService<IAmazonSimpleEmailServiceV2>();



            // -----------------------------------------
            // Aws S3  relates to file upload
            // -----------------------------------------
            builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
            builder.Services.AddAWSService<IAmazonS3>();
            builder.Services.Configure<S3Settings>(builder.Configuration.GetSection("S3Settings"));



            // -----------------------------
            // JWT Configuration
            // -----------------------------
            //var jwt = builder.Configuration.GetSection("Jwt");
            //var key = jwt["Key"];
            //var issuer = jwt["Issuer"];
            //var audience = jwt["Audience"];

            //Console.WriteLine("VALIDATION KEY USED: " + key);
            // -----------------------------
            // JWT Configuration
            // -----------------------------
            var jwt = builder.Configuration.GetSection("Jwt");
            var key = jwt["Key"] ?? throw new InvalidOperationException("CRITICAL ERROR: JWT Key is null or missing from appsettings!");
            var issuer = jwt["Issuer"];
            var audience = jwt["Audience"];

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
            builder.Services.AddScoped<IEmailService, SESEmailService>();
            builder.Services.AddScoped<IOfferService, OfferService>();
            builder.Services.AddScoped<IPswService, PswService>();
            builder.Services.AddScoped<IFileService, S3FileService>();
            builder.Services.AddScoped<IApplyService, ApplyService>();
            builder.Services.AddScoped<IApplicationManagementService, ApplicationManagementService>();
            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IProfileService, ProfileService>();

            var app = builder.Build();

            await SeedRolesAsync(app);

            // -----------------------------
            // Middleware
            // -----------------------------
            // if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI();
            //}
            app.UseMiddleware<ExceptionMiddleware>();

            if (!app.Environment.IsProduction())
            {
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles(); //show photo

            app.UseCors("AllowAngular");

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
            try
            {
                using var scope = app.Services.CreateScope();

                var dbContext = scope.ServiceProvider
                    .GetRequiredService<ReliefAppDbContext>();

                await dbContext.Database.MigrateAsync();

                var roleManager = scope.ServiceProvider
                    .GetRequiredService<RoleManager<IdentityRole<Guid>>>();

                var userManager = scope.ServiceProvider
                    .GetRequiredService<UserManager<ApplicationUser>>();

                // Create Roles
                foreach (var role in new[] { "Admin", "CareHome", "PSW", "Individual" })
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

                // Create Admin User
                var adminEmail = "admin@test.com";

                var admin = await userManager.FindByEmailAsync(adminEmail);

                if (admin != null)
                {
                    // Check if existing user has the Admin role
                    var roles = await userManager.GetRolesAsync(admin);
                    if (!roles.Contains("Admin"))
                    {
                        // Broken record from previous seed — delete and recreate
                        await userManager.DeleteAsync(admin);
                        admin = null;
                    }
                }

                if (admin == null)
                {
                    admin = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true,
                        FirstName = "System",
                        LastName = "Admin",
                        BirthOfDate = new DateTime(1990, 1, 1),
                        Address = new Address
                        {
                            ApartmentNumber = 0,
                            Street = "Admin",
                            City = "Admin",
                            State = "Admin",
                            PostalCode = "00000",
                            Country = "Admin"
                        }
                    };

                    var result = await userManager.CreateAsync(admin, "Admin123!");

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, "Admin");
                    }
                    else
                    {
                        Console.WriteLine("⚠️ Admin seed failed: " +
                            string.Join("; ", result.Errors.Select(e => e.Description)));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ SEED FAILED: {ex.Message}");
                Console.WriteLine($"⚠️ INNER: {ex.InnerException?.Message}");
            }
        }
    }
}
