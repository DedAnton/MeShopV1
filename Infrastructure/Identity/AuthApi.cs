using Infrastructure.Database;
using Infrastructure.Database.Entities;
using Infrastructure.Identity;
using LinqToDB;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Auth;

public static class AuthApi
{
    public static void Map(WebApplication app)
    {
        var group = app.MapGroup("/auth")
            .WithOpenApi()
            .WithTags("Auth");

        group.MapPost("/register", async (RegisterRequest request, IDataConnection db, PasswordHasher passwordHasher, CancellationToken cancellationToken) =>
        {
            var isEmailExist = await db.Users.AnyAsync(x => x.Email == request.Email);

            if (isEmailExist)
            {
                return Results.Conflict("Email already exists");
            }

            var passwordHash = passwordHasher.HashPassword(request.Password);
            await db.Users.InsertAsync(() => new IdentityUser 
            {
                Id = Guid.NewGuid(), 
                Email = request.Email,
                PasswordHash = passwordHash 
            }, cancellationToken);

            return Results.Ok();
        });

        group.MapPost("/login", async (
            LoginRequest request, 
            IDataConnection db, 
            PasswordHasher passwordHasher, 
            TokenGenerator tokenGenerator,
            CancellationToken cancellationToken) =>
        {
            var user = await db.Users
                .Where(x => x.Email == request.Email)
                .FirstOrDefaultAsync(cancellationToken);
            if (user is null)
            {
                return Results.NotFound("User not found");
            }

            var verificationResult = passwordHasher.VerifyHashedPassword(request.Password, user.PasswordHash);
            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return Results.Unauthorized();
            }

            if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
            {
                var newPasswordHash = passwordHasher.HashPassword(request.Password);
                await db.Users
                    .Where(x => x.Id == user.Id)
                    .Set(x => x.PasswordHash, newPasswordHash)
                    .UpdateAsync(cancellationToken);
            }

            var accessToken = tokenGenerator.GenerateToken(user.Id, user.Email);
            return Results.Ok(new
            {
                accessToken
            });
        });
    }
}