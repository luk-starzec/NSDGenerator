using NSDGenerator.Shared.Diagram;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace NSDGenerator.Server.Diagram.Repo;

public interface IDiagramRepo
{
    Task<IEnumerable<DiagramDto>> GetDiagramInfosAsync(string userName);
    Task<DiagramFullDto> GetDiagramAsync(Guid id, string userName);

    Task<bool> CheckIfDiagramExistsAsync(Guid id);

    Task<bool> SaveDiagramAsync(DiagramFullDto diagram, string userName);

    Task<bool> DeleteDiagramAsync(Guid id, string userName);

}
