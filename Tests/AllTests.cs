using MeShopV1.Cart;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Testcontainers.MsSql;

namespace Tests;

public class AllTests
{
    private static HttpClient _client = null!;
    private static MediaTypeHeaderValue _jsonMedia = MediaTypeHeaderValue.Parse("application/json");

    private static readonly MsSqlContainer _dbContainer =
        new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    [Before(Class)]
    public static async Task Initialize()
    {
        await _dbContainer.StartAsync();
        var waf = new WebApplicationFactory<CartService>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("ConnectionStrings:SqlServer", _dbContainer.GetConnectionString());
            });

        _client = waf.CreateClient();
    }

    [After(Class)]
    public static async Task CleanUp()
    {
        _client.Dispose();
        await _dbContainer.DisposeAsync();
    }

    [Test]
    public async Task Registration()
    {
        var body =
            """
            {
                "email": "test@mail.ru",
                "password": "1234"
            }
            """;

        var response = await _client.PostAsync("/auth/register", new StringContent(body, _jsonMedia));
        var content = await response.Content.ReadAsStringAsync();

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }

    [Test]
    [DependsOn(nameof(Registration))]
    public async Task RegistrationWithExistEmail()
    {
        var body =
            """
            {
                "email": "test@mail.ru",
                "password": "1234"
            }
            """;

        var response = await _client.PostAsync("/auth/register", new StringContent(body, _jsonMedia));
        var responseContent = await response.Content.ReadAsStringAsync();

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Conflict);
    }

    public record LoginResponse(string AccessToken);
    [Test]
    [DependsOn(nameof(Registration))]
    public async Task Login()
    {
        var body =
            """
            {
                "email": "test@mail.ru",
                "password": "1234"
            }
            """;

        var response = await _client.PostAsync("/auth/login", new StringContent(body, _jsonMedia));
        var responseContent = await response.Content.ReadFromJsonAsync<LoginResponse>();

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        _accessToken = responseContent?.AccessToken ?? throw new Exception("accessToken not found");
    }
    private static string _accessToken = string.Empty;
}
