using Microsoft.EntityFrameworkCore;
using LibraryApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LibraryApp.Services;
using LibraryApp.Repositories;
using LibraryApp.Middleware;
using FluentValidation.AspNetCore;
using LibraryApp.Validators;
using LibraryApp.Interfaces;
using LibraryApp.Mappings;
using LibraryApp.Infrastructure;
using Microsoft.EntityFrameworkCore.Design;
using System.Security.Claims;
using LibraryApp.UseCases.Facades;
using LibraryApp.Models;
using Microsoft.AspNetCore.Mvc;


namespace LibraryApp
{
    public class Program
    {
        [Obsolete]
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Регистрация DbContext
            builder.Services.AddDbContext<LibraryContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("LibraryConnection")));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<LibraryContext>()
                .AddDefaultTokenProviders();

            // Регистрация DataSeeder
            builder.Services.AddTransient<DataSeeder>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddEndpointsApiExplorer();

            // Конфигурация JWT
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["Secret"];
            var key = Encoding.ASCII.GetBytes(secretKey!);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!))
                };
            });

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Library API", Version = "v1" });

                // Настройка JWT в Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Введите 'Bearer' [пробел] и ваш токен",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
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

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy =>
                policy.RequireAssertion(context =>
                context.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Admin")));
                options.AddPolicy("UserPolicy", policy => policy.RequireRole("Admin", "User"));
            });

            // Регистрация служб
            builder.Services.AddScoped<TokenService>();
            builder.Services.Scan(scan => scan
                .FromAssembliesOf(typeof(AuthorsUseCasesFacade), typeof(BooksUseCasesFacade), typeof(AuthorizationUseCasesFacade))
                .AddClasses(classes => classes.Where(type => type.Name.EndsWith("UseCase") || type.Name.EndsWith("Facade")))
                .AsSelf()
                .WithTransientLifetime());

            //UnitOfWork
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IBookRepository, BookRepository>();
            builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            //Mapping
            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

            //FluentValidation
            builder.Services.AddControllers()
            .AddFluentValidation(fv =>
            {
                fv.RegisterValidatorsFromAssemblyContaining<BookValidator>();
                fv.RegisterValidatorsFromAssemblyContaining<AuthorValidator>();
            });

            builder.Services.Configure<ApiBehaviorOptions>(options
                => options.SuppressModelStateInvalidFilter = true);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseHsts();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            using (var scope = app.Services.CreateScope())
            {
                var facade = scope.ServiceProvider.GetRequiredService<AuthorizationUseCasesFacade>();
                Console.WriteLine(facade != null ? "Фасад доступен" : "Фасад недоступен");
            }


            using (var scope = app.Services.CreateScope())
            {
                var dataSeeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
                await dataSeeder.SeedAsync();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
                RequestPath = ""
            });

            app.UseRouting();

            app.UseCors();
            app.UseAuthentication();

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseAuthorization();

            app.MapControllers();
            app.MapDefaultControllerRoute();

            app.Run();
        }
        public class LibraryContextFactory : IDesignTimeDbContextFactory<LibraryContext>
        {
            public LibraryContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<LibraryContext>();
                optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=LibraryDb;Trusted_Connection=True;");

                return new LibraryContext(optionsBuilder.Options);
            }
        }

    }
}
