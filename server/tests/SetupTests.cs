using System.Text.Json;
using api.Etc;
using api.Models.Dtos.Requests;
using api.Models.Requests;
using api.Services;
using dataccess;

namespace tests;

public class SetupTests(MyDbContext ctx,
    ISeeder seeder,
    ITestOutputHelper outputHelper,
    IAuthService authService)
{

    [Fact]
    public async Task RegisterReturnsJwtWhichCanVerifyAgain()
    {
        var result = await authService.Register(new RegisterRequestDto
        {
            Email = "test_user@email.dk",
            Password = "ppaasswwoorrdd"
        });
        outputHelper.WriteLine(result.Token);
        var token = await authService.VerifyAndDecodeToken(result.Token);
        outputHelper.WriteLine(JsonSerializer.Serialize(token));
    }

    [Fact]
    public async Task SeederDoesNotThrowException()
    {
        await seeder.Seed();
    }
}