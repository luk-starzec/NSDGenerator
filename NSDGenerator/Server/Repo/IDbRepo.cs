using NSDGenerator.Shared.Diagram;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSDGenerator.Server.Repo;

public interface IDbRepo
{
    Task<IEnumerable<DiagramDto>> GetDiagramInfosAsync(string userName);
    Task<DiagramFullDto> GetDiagramAsync(Guid id, string userName);
    Task SaveDiagramAsync(DiagramFullDto diagram, string userName);
}
