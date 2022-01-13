using NSDGenerator.Shared.Diagram;
using NSDGenerator.Shared.Login;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSDGenerator.Server.Repo;

public interface IDbRepo
{
    Task<IEnumerable<DiagramDto>> GetDiagramInfosAsync(string userName);
    Task<DiagramFullDto> GetDiagramAsync(Guid id, string userName);

    Task<bool> CheckIfDiagramExistsAsync(Guid id);

    Task<bool> SaveDiagramAsync(DiagramFullDto diagram, string userName);

    Task<bool> DeleteDiagramAsync(Guid id, string userName);

    Task<string> RegisterUserAsync(RegisterDto register);
}
