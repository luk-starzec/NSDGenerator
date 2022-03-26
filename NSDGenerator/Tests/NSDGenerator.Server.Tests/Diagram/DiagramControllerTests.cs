using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NSDGenerator.Server.Diagram.Repo;
using NSDGenerator.Shared.Diagram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace NSDGenerator.Server.Tests.Diagram;

public class DiagramControllerTests
{
    private readonly WebApplicationFactory<Startup> factory;
    private readonly WebApplicationFactory<Startup> unauthorizedFactory;

    private readonly Mock<IDiagramRepo> diagramRepoStub = new();

    private DiagramDTO testDiagram;

    public DiagramControllerTests()
    {
        testDiagram = new DiagramDTO(Guid.NewGuid(), "test", false, "test", new BlockCollectionDTO(), new List<int>());

        unauthorizedFactory = new WebApplicationFactory<Startup>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services => services.AddSingleton(diagramRepoStub.Object));
            });

        factory = new WebApplicationFactory<Startup>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
                });
                builder.ConfigureServices(services => services.AddSingleton(diagramRepoStub.Object));
            });
    }

    [Fact]
    public async Task GetDiagram_ForExistingDiagramReturnsDiagram()
    {
        diagramRepoStub
            .Setup(r => r.GetDiagramAsync(It.IsAny<Guid>(), It.IsAny<string>()))
            .Returns(Task.FromResult(testDiagram));

        var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/diagram/{testDiagram.Id}");

        response.EnsureSuccessStatusCode();
        var actual = await response.Content.ReadFromJsonAsync<DiagramDTO>();

        Assert.NotNull(actual);
        Assert.Equal(testDiagram.Id, actual?.Id);
        Assert.Equal(testDiagram.Name, actual?.Name);
    }

    [Fact]
    public async Task GetDiagram_ForNotExistingDiagramReturnsNotFound()
    {
        diagramRepoStub
            .Setup(r => r.GetDiagramAsync(It.IsAny<Guid>(), It.IsAny<string>()))
            .Returns(Task.FromResult<DiagramDTO?>(null));

        var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/diagram/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CheckIfDiagramExists_ForExistingDiagramReturnsOk()
    {
        diagramRepoStub
            .Setup(r => r.CheckIfDiagramExistsAsync(It.IsAny<Guid>()))
            .Returns(Task.FromResult(true));

        var client = factory.CreateClient();

        var actual = await client.GetAsync($"/api/diagram/exists/{testDiagram.Id}");

        Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
    }

    [Fact]
    public async Task CheckIfDiagramExists_ForNotExistingDiagramReturnsNotFound()
    {
        diagramRepoStub
            .Setup(r => r.CheckIfDiagramExistsAsync(It.IsAny<Guid>()))
            .Returns(Task.FromResult(false));

        var client = factory.CreateClient();

        var actual = await client.GetAsync($"/api/diagram/exists/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, actual.StatusCode);
    }

    [Fact]
    public async Task GetDiagrams_ForNotAuthenticatedReturnsUnauthorized()
    {
        var client = unauthorizedFactory.CreateClient();

        var actual = await client.GetAsync($"/api/diagram/");

        Assert.Equal(HttpStatusCode.Unauthorized, actual.StatusCode);
    }

    [Fact]
    public async Task GetDiagrams_ForAuthenticatedReturnsUserDiagrams()
    {
        var diagrams = new List<DiagramInfoDTO>()
            {
                new DiagramInfoDTO(Guid.NewGuid(), "test 1", false, DateTime.Now, DateTime.Now),
                new DiagramInfoDTO(Guid.NewGuid(), "test 2", false, DateTime.Now, DateTime.Now),
            };
        diagramRepoStub
            .Setup(r => r.GetDiagramInfosAsync(It.IsAny<string>()))
            .Returns(Task.FromResult<IEnumerable<DiagramInfoDTO>>(diagrams));

        var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/diagram/");

        response.EnsureSuccessStatusCode();
        var actual = await response.Content.ReadFromJsonAsync<IEnumerable<DiagramInfoDTO>>();

        Assert.NotNull(actual);
#pragma warning disable CS8604 // Possible null reference argument.
        Assert.Equal(diagrams.Count, actual.Count());
        Assert.True(diagrams.All(r => actual.Contains(r)));
#pragma warning restore CS8604 // Possible null reference argument.
    }

    [Fact]
    public async Task SaveDiagram_ForNotAuthenticatedReturnsUnauthorized()
    {
        var client = unauthorizedFactory.CreateClient();

        var actual = await client.PostAsJsonAsync($"/api/diagram/", testDiagram);

        Assert.Equal(HttpStatusCode.Unauthorized, actual.StatusCode);
    }

    [Fact]
    public async Task DeleteDiagram_ForNotAuthenticatedReturnsUnauthorized()
    {
        var client = unauthorizedFactory.CreateClient();

        var actual = await client.DeleteAsync($"/api/diagram/{Guid.NewGuid}");

        Assert.Equal(HttpStatusCode.Unauthorized, actual.StatusCode);
    }
}
