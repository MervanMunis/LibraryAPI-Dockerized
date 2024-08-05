using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Google;
using LibraryAPI.Data;
using LibraryAPI.Services.impl;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using LibraryAPI.Auth.Controller;
using LibraryAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using LibraryAPI.Models.Entities;
using LibraryAPI.Auth.Services;

namespace LibraryAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register the DbContext with the connection string
            builder.Services.AddDbContext<LibraryAPIContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("LibraryAPIContext"),
                    sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()
                ));

            // Add services to the container.
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<LibraryAPIContext>()
                .AddDefaultTokenProviders();


            // Configure authentication and authorization
            var jwtSettings = builder.Configuration.GetSection("Authentication:Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddCookie(options =>
                {
                    options.LoginPath = "/api/authentication/login";
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        RoleClaimType = ClaimTypes.Role,
                        ClockSkew = TimeSpan.Zero
                    };
                })
                .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
                {
                    IConfigurationSection googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");
                    options.ClientId = googleAuthNSection["ClientId"];
                    options.ClientSecret = googleAuthNSection["ClientSecret"];
                    options.SaveTokens = true;
                    options.Events.OnRedirectToAuthorizationEndpoint = context =>
                    {
                        context.Response.Redirect(context.RedirectUri);
                        return Task.CompletedTask;
                    };
                });

            builder.Services.AddAuthorization();

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                });

            // Register services and their implementations
            builder.Services.AddScoped<IBookService, BookService>();
            builder.Services.AddScoped<IAuthorService, AuthorService>();
            builder.Services.AddScoped<IPublisherService, PublisherService>();
            builder.Services.AddScoped<ILanguageService, LanguageService>();
            builder.Services.AddScoped<ILocationService, LocationService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ISubCategoryService, SubCategoryService>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            builder.Services.AddScoped<IFileService, FileService>();
            builder.Services.AddScoped<INationalityService, NationalityService>();
            builder.Services.AddScoped<ILoanService, LoanService>();
            builder.Services.AddScoped<IPenaltyService, PenaltyService>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped<IMemberService, MemberService>();
            builder.Services.AddScoped<IWantedBookService, WantedBookService>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<IEmailService, EmailService>();


            // Add Swagger configuration
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "LibraryAPI", Version = "v1" });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LibraryAPI v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Apply migrations at startup
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<LibraryAPIContext>();
                dbContext.Database.Migrate();

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                RoleInitializer.InitializeAsync(userManager, roleManager).Wait();
            }
            app.Run();
        }
    }
}
