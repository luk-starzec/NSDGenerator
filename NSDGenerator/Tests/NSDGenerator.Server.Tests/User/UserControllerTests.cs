using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using NSDGenerator.Server.User.Models;
using NSDGenerator.Server.User.Repo;
using NSDGenerator.Shared.User;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace NSDGenerator.Server.Tests.User
{
    public class UserControllerTests
    {
        private readonly WebApplicationFactory<Startup> factory;

        private readonly LoginDTO validLoginData = new LoginDTO("test@test.com", "12345678");
        private readonly RegisterDTO validRegisterData = new RegisterDTO("test@test.com", "12345678", "test1");
        private readonly JwtSettings jwtSettings = new() { SecurityKey = "RANDOM_KEY_FOR_TESTS" };

        public UserControllerTests()
        {
            var optionsMock = new Mock<IOptions<JwtSettings>>();
            optionsMock
                .Setup(r => r.Value)
                .Returns(jwtSettings);

            var userRepoMock = new Mock<IUserRepo>();
            userRepoMock
                .Setup(r => r.VerifyUserAsync(It.IsAny<LoginDTO>()))
                .Returns(Task.FromResult(false));
            userRepoMock
                .Setup(r => r.VerifyUserAsync(validLoginData))
                .Returns(Task.FromResult(true));
            userRepoMock
                .Setup(r => r.RegisterUserAsync(It.IsAny<RegisterDTO>()))
                .Returns(Task.FromResult("error"));
            userRepoMock
                .Setup(r => r.RegisterUserAsync(validRegisterData))
                .Returns(Task.FromResult((string?)null));

            factory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.AddSingleton(userRepoMock.Object);
                        services.AddSingleton(optionsMock.Object);
                    });
                });
        }


        [Fact]
        public async Task Login_ForValidUserReturnsSuccessfulResult()
        {
            var client = factory.CreateClient();

            var user = validLoginData;
            var response = await client.PostAsJsonAsync("/api/user/login", user);

            response.EnsureSuccessStatusCode();
            var actual = await response.Content.ReadFromJsonAsync<LoginResult>();

            Assert.True(actual.IsSuccessful);
            Assert.NotNull(actual.Token);
        }

        [Fact]
        public async Task Login_ForInvalidUserReturnsErrorResult()
        {
            var client = factory.CreateClient();

            var user = validLoginData with { Password = "abc" };
            var response = await client.PostAsJsonAsync("/api/user/login", user);

            var actual = await response.Content.ReadFromJsonAsync<LoginResult>();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.False(actual.IsSuccessful);
            Assert.NotNull(actual.Error);
        }

        [Fact]
        public async Task Register_ForValidDataReturnsSuccessfulResult()
        {
            var client = factory.CreateClient();

            var register = validRegisterData;
            var response = await client.PostAsJsonAsync("/api/user/register", register);

            response.EnsureSuccessStatusCode();
            var actual = await response.Content.ReadFromJsonAsync<RegisterResult>();

            Assert.True(actual.IsSuccessful);
            Assert.NotNull(actual.Token);
        }

        [Fact]
        public async Task Register_WithoutRegistrationCodeReturnsErrorResult()
        {
            var client = factory.CreateClient();

            var register = validRegisterData with { RegistrationCode = null };
            var response = await client.PostAsJsonAsync("/api/user/register", register);

            var actual = await response.Content.ReadFromJsonAsync<RegisterResult>();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.False(actual.IsSuccessful);
            Assert.NotNull(actual.Error);
        }

        [Fact]
        public async Task Register_ForInvalidDataReturnsErrorResult()
        {
            var client = factory.CreateClient();

            var register = validRegisterData with { Email = "aaa@aaa.aa" };
            var response = await client.PostAsJsonAsync("/api/user/register", register);

            var actual = await response.Content.ReadFromJsonAsync<RegisterResult>();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.False(actual.IsSuccessful);
            Assert.NotNull(actual.Error);
        }
    }
}
