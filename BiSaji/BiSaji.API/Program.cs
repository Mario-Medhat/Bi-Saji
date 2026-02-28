
using BiSaji.API.Data;
using BiSaji.API.Exceptions;
using BiSaji.API.Interfaces.RepositoryInterfaces;
using BiSaji.API.Interfaces.ServicesInterfaces;
using BiSaji.API.Models.Domain;
using BiSaji.API.Repositories;
using BiSaji.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;
using Serilog.Events;
using System.Text;

namespace BiSaji.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(
                path: "Logs/InfoLogs/BiSaji_Info_log_.txt",
                rollingInterval: RollingInterval.Hour,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                retainedFileCountLimit: 365 / 2 // keep logs for 6 months
                )
            .MinimumLevel.Information()
            .WriteTo.File(
                path: "Logs/Errorlogs/BiSaji_Error_log_.txt",
                rollingInterval: RollingInterval.Hour,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                retainedFileCountLimit: 365, // keep logs for 12 months
                restrictedToMinimumLevel: LogEventLevel.Error // only log error and above (critical)
            )
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .CreateLogger();
            //.CreateBootstrapLogger();

            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger);


            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Bi Saji API App",
                    Version = "v1",
                    Description = "This API is for managing Bi Saji App.",
                    Contact = new OpenApiContact
                    {
                        Name = "Mario Medhat",
                        Email = "mario.medhat.mansour@gmail.com",
                        Url = new Uri("https://www.linkedin.com/in/mario-medhat-9241b1216/"),
                    }
                });

                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                });

                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(
                        JwtBearerDefaults.AuthenticationScheme,
                        document
                        )] = new List<string>()
                });

            });


            // Add DbContext for the main database connection
            builder.Services.AddDbContext<BiSajiDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("BiSajiConnectionString"));
            });

            // For in-memory repository (Development purposes)
            //builder.Services.AddScoped<IRegionRepository, InMemoryRegionRepository>();

            // For SQL repository (Production purposes)
            builder.Services.AddScoped<ITokenRepository, SQLTokenRepository>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IUserRepository, SQLUsersRepository>();
            builder.Services.AddScoped<IStudentsRepository, SQLStudentsRepository>();

            // TODO: Add AutoMapper
            //builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

            builder.Services.AddIdentityCore<Servant>()
                .AddRoles<IdentityRole>()
                .AddTokenProvider<DataProtectorTokenProvider<Servant>>("BiSajiDb")
                .AddEntityFrameworkStores<BiSajiDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
            });


            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ??
                        throw new InvalidOperationException("JWT Key is not configured")))
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                    options.RoutePrefix = string.Empty; // يخليه يفتح على root
                });
            }

            app.UseHttpsRedirection();

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exceptionHandler = context.Features.Get<IExceptionHandlerFeature>();
                    var exception = exceptionHandler?.Error;

                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = exception switch
                    {
                        NotFoundException => StatusCodes.Status404NotFound,
                        _ => StatusCodes.Status500InternalServerError
                    };

                    await context.Response.WriteAsJsonAsync(new
                    {
                        message = exception?.Message
                    });
                });
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
