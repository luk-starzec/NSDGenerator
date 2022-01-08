using NSDGenerator.Shared.Diagram.JsonModels;
using NSDGenerator.Shared.Diagram.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSDGenerator.Server.Repo;

public interface IDbRepo
{
    Task<IEnumerable<DiagramInfoModel>> GetDiagramInfosAsync(string userName);
    Task<DiagramJsonModel> GetDiagramAsync(Guid id, string userName);
    Task SaveDiagramAsync(DiagramJsonModel diagram, string userName);
}
