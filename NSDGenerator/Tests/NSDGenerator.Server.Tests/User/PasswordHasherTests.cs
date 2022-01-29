using Microsoft.Extensions.Options;
using NSDGenerator.Server.User.Helpers;
using NSDGenerator.Server.User.Models;
using Xunit;

namespace NSDGenerator.Server.Tests.User;

public class PasswordHasherTests
{
    private readonly int iterations = 1000;
    private readonly IPasswordHasher sut;

    public PasswordHasherTests()
    {
        var options = Options.Create(new HashingSettings { Iterations = iterations });
        sut = new PasswordHasher(options);
    }

    [Fact]
    public void Hash_ReturnsHashInValidFormat()
    {
        var hash = sut.Hash("test");
        var segments = hash.Split(".");

        Assert.True(segments.Length == 3);
        Assert.Equal(int.Parse(segments[0]), iterations);
    }

    [Fact]
    public void Check_ReturnsPositiveVerificationForValidPassword()
    {
        var password = "test";
        var hash = "1000.wESRsVfNpEkh1rchud8nLw==.iQUU/1qyxKrdvZnblGB4n+Wpr1HueXfjX3iVboOoa7g=";

        var actual = sut.Check(hash, password);

        Assert.True(actual.Verified);
    }

    [Fact]
    public void Check_ReturnsNegativeVerificationForInvalidPassword()
    {
        var password = "abc";
        var hash = "1000.wESRsVfNpEkh1rchud8nLw==.iQUU/1qyxKrdvZnblGB4n+Wpr1HueXfjX3iVboOoa7g=";

        var actual = sut.Check(hash, password);

        Assert.False(actual.Verified);
    }
}
