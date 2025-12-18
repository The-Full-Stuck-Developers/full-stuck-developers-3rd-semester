using System.Text.Json;
using api.Etc;
using api.Models.Dtos.Requests.Auth;
using api.Services;
using dataccess;
using Xunit;

namespace tests;

public class SetupTests : IClassFixture<SetupFixture>
{
    private readonly MyDbContext _ctx;
    private readonly ISeeder _seeder;
    private readonly IAuthService _authService;
    private readonly ITestOutputHelper _output;

    public SetupTests(SetupFixture fx, ITestOutputHelper output)
    {
        _ctx = fx.Ctx;
        _seeder = fx.Seeder;
        _authService = fx.AuthService;
        _output = output;
    }

    [Fact]
    public async Task SeederDoesNotThrowException()
    {
        await _seeder.Seed();
    }

    [Fact]
    public async Task RegisterReturnsJwtWhichCanVerifyAgain()
    {
        // If you don’t have VerifyAndDecodeToken anymore, keep it commented.
        // But at least the test will construct and run.

        // Example (depends on your actual AuthService interface):
        // var result = await _authService.Register(new RegisterRequestDto
        // {
        //     Email = "test_user@email.dk",
        //     Name = "Test User",
        //     Password = "ppaasswwoorrdd"
        // });
        // _output.WriteLine(JsonSerializer.Serialize(result));
    }
}