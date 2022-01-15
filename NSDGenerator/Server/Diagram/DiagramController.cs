using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSDGenerator.Server.Diagram.Repo;
using NSDGenerator.Shared.Diagram;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSDGenerator.Server;

[ApiController]
[Route("api/[controller]")]
public class DiagramController : ControllerBase
{
    private readonly IDiagramRepo _repo;

    public DiagramController(IDiagramRepo diagramRepo)
    {
        _repo = diagramRepo ?? throw new ArgumentNullException(nameof(diagramRepo));
    }

    [HttpGet, Authorize]
    public async Task<IEnumerable<DiagramDto>> GetDiagrams()
    {
        var userName = User.Identity.Name;
        return await _repo.GetDiagramInfosAsync(userName);
    }

    [HttpPost, Authorize]
    public async Task<IActionResult> SaveDiagram([FromBody] DiagramFullDto diagram)
    {
        var userName = User.Identity.Name;
        var result = await _repo.SaveDiagramAsync(diagram, userName);
        return result ? NoContent() : BadRequest();
    }

    [HttpDelete("{id}"), Authorize]
    public async Task<IActionResult> DeleteDiagram(Guid id)
    {
        var userName = User.Identity.Name;
        var result = await _repo.DeleteDiagramAsync(id, userName);
        return result ? NoContent() : BadRequest();
    }

    [HttpGet("{id}")]
    public async Task<DiagramFullDto> GetDiagram(Guid id)
    {
        var userName = User.Identity.IsAuthenticated ? User.Identity.Name : null;
        return await _repo.GetDiagramAsync(id, userName);
    }

    [HttpGet("exists/{id}")]
    public async Task<IActionResult> CheckIfDiagramExists(Guid id)
    {
        var exists = await _repo.CheckIfDiagramExistsAsync(id);
        return exists ? Ok() : NotFound();
    }
}
