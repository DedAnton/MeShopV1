using Hangfire;
using Infrastructure.Auth;
using Infrastructure.Database;
using Infrastructure.Identity;
using LinqToDB;
using LinqToDB.AspNet;
using LinqToDB.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure;

public static class DependencyInjection
{
    public static void AddDatabase(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetDatabaseConnectionString();

        var dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
           .UseSqlServer(connectionString)
           .Options;
        using var context = new ApplicationDbContext(dbOptions);
        var mappingSchema = LinqToDBForEFTools.GetMappingSchema(context.Model, accessor: context, dataOptions: null);

        builder.Services.AddLinqToDBContext<ApplicationDataConnection>(
            (_, options) => options
                .UseSqlServer(connectionString)
                .UseMappingSchema(mappingSchema));

        builder.Services.AddScoped<IDataConnection>(x => x.GetRequiredService<ApplicationDataConnection>());
    }

    public static void AddAuth(this IHostApplicationBuilder builder)
    {
        var jwtOptions = builder.Configuration.GetRequiredSection(nameof(JwtOptions)).Get<JwtOptions>() 
            ?? throw new InvalidOperationException("Can not bind JwtOptions configuration section");
        builder.Services
            .Configure<JwtOptions>(builder.Configuration.GetRequiredSection(nameof(JwtOptions)))
            .AddSingleton<JwtSecurityTokenHandler>()
            .AddSingleton<TokenGenerator>()
            .AddSingleton<PasswordHasher>()
            .AddSingleton(_ => RandomNumberGenerator.Create());

        builder.Services
            .AddAuthorization()
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped(resolver =>
        {
            var httpContext = resolver.GetRequiredService<IHttpContextAccessor>().HttpContext ?? throw new InvalidOperationException("HttpContext not found"); ;
            var idClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("Claim not found");
            var emailClaim = httpContext.User.FindFirst(ClaimTypes.Email) ?? throw new InvalidOperationException("Claim not found");

            return new UserInfo(Guid.Parse(idClaim.Value), emailClaim.Value);
        });
    }

    public static void UseAuth(this WebApplication app)
    {
        AuthApi.Map(app);
        app.UseAuthentication();
        app.UseAuthorization();
    }

    public static void AddOpenApi(this IHostApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "JWT Authorization header using the Bearer scheme."
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearerAuth" }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }

    public static void UseOpenApi(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger(options =>
            {
                options.RouteTemplate = "openapi/{documentName}.json";
            });
            app.MapScalarApiReference();
        }
    }

    public static void AddBackgroundJobs(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetDatabaseConnectionString();

        builder.Services
            .AddHangfire(x => x
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(connectionString));

        builder.Services.AddHangfireServer();
    }

    private static string GetDatabaseConnectionString(this IConfiguration configuration) => 
        configuration.GetConnectionString("SqlServer") ?? throw new Exception("Conncetion string 'SqlServer' not found");
}