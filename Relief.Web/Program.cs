
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Relief.Domain.Entities;
using Relief.Presentation.Controllers;
using Relief.Presistence.Data.DbContexts;
using Relief.ServiceAbstraction;
using Relief.Services;
using System.Text;

namespace Relief.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
                .AddApplicationPart(typeof(AuthController).Assembly);
            ;
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Main APP DB
            builder.Services.AddDbContext<ReliefIdentityDbContext>(opt =>
                opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Identity DB
            //builder.Services.AddDbContext<ReliefIdentityDbContext>(opt =>
            //    opt.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));

            // Identity uses ONLY the Identity DbContext
            builder.Services
                .AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
                {
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<ReliefIdentityDbContext>()
                .AddDefaultTokenProviders();

            // JWT
            var jwt = builder.Configuration.GetSection("Jwt");
            var key = jwt["Key"]!;

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(opt =>
                    {
                        opt.TokenValidationParameters = new()
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = jwt["Issuer"],
                            ValidAudience = jwt["Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                            ClockSkew = TimeSpan.Zero
                        };
                    });

            // services

            builder.Services.AddAuthorization();
            builder.Services.AddScoped<IAuthService, AuthService>();

            var app = builder.Build();

            await SeedRolesAsync(app);

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

            app.Run();
            
        }

        static async Task SeedRolesAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            foreach (var role in new[] { "CareHome", "PSW" })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole<Guid> { Name = role, NormalizedName = role.ToUpper() });
            }
        }
    }
}
