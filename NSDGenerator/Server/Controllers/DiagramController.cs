using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSDGenerator.Server.Repo;
using NSDGenerator.Shared.Diagram;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSDGenerator.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiagramController : ControllerBase
{
    private readonly IDbRepo repo;

    public DiagramController(IDbRepo repo)
    {
        this.repo = repo;
    }

    [HttpGet, Authorize]
    public async Task<IEnumerable<DiagramDto>> GetDiagrams()
    {
        var userName = User.Identity.Name;
        return await repo.GetDiagramInfosAsync(userName);
    }

    [HttpPost, Authorize]
    public async Task SaveDiagram([FromBody] DiagramFullDto diagram)
    {
        var userName = User.Identity.Name;
        await repo.SaveDiagramAsync(diagram, userName);
    }

    [HttpGet("{id}")]
    public async Task<DiagramFullDto> GetDiagram(Guid id)
    {
        var userName = User.Identity.IsAuthenticated ? User.Identity.Name : null;
        return await repo.GetDiagramAsync(id, userName);
    }
}
