using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NSDGenerator.Server.DbData;
using NSDGenerator.Server.Diagram.Helpers;
using NSDGenerator.Server.Diagram.Repo;
using System;
using System.Threading.Tasks;
using Xunit;

namespace NSDGenerator.Server.Tests.Diagram;

public class DiagramRepoTests
{
    private DbContextOptions<NsdContext> dbContextOptions;

    private readonly Guid privateDiagramId = Guid.NewGuid();
    private readonly Guid publicDiagramId = Guid.NewGuid();
    private readonly string ownerName = "Owner";

    public DiagramRepoTests()
    {
        dbContextOptions = new DbContextOptionsBuilder<NsdContext>()
            .UseInMemoryDatabase("TestDB")
            .Options;
    }

    [Fact]
    public async Task GetDiagramAsync_ForOwnerReturnsPrivateDiagram()
    {
        var sut = await CreateRepo();

        var actual = await sut.GetDiagramAsync(privateDiagramId, ownerName);

        Assert.NotNull(actual);
    }

    [Fact]
    public async Task GetDiagramAsync_ForOwnerReturnsPublicDiagram()
    {
        var sut = await CreateRepo();

        var actual = await sut.GetDiagramAsync(publicDiagramId, ownerName);

        Assert.NotNull(actual);
    }

    [Fact]
    public async Task GetDiagramAsync_ForGuestNotReturnsPrivateDiagram()
    {
        var sut = await CreateRepo();

        var actual = await sut.GetDiagramAsync(privateDiagramId, "");

        Assert.Null(actual);
    }

    [Fact]
    public async Task GetDiagramAsync_ForGuestReturnsPublicDiagram()
    {
        var sut = await CreateRepo();

        var actual = await sut.GetDiagramAsync(publicDiagramId, "");

        Assert.NotNull(actual);
    }


    private async Task<IDiagramRepo> CreateRepo()
    {
        var loggerStub = new Mock<ILogger<DiagramRepo>>();
        var dtoConverterStub = new Mock<IDtoConverter>();
        var context = await CreateDbContext();

        return new DiagramRepo(loggerStub.Object, context, dtoConverterStub.Object);
    }

    private async Task<NsdContext> CreateDbContext()
    {
        var context = new NsdContext(dbContextOptions);

        var diagrams = new DbData.Diagram[]
        {
            new () { Id = privateDiagramId, Name = "Private diagram", UserName = ownerName, IsPrivate = true, ColumnsWidth = "" },
            new () { Id = publicDiagramId, Name = "Public diagram", UserName = ownerName, IsPrivate = false, ColumnsWidth = "" },
        };
        await context.Diagrams.AddRangeAsync(diagrams);

        await context.SaveChangesAsync();

        return context;
    }
}
