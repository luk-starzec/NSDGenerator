using NSDGenerator.Shared.Diagram.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSDGenerator.Client.Services
{
    public interface IDiagramService
    {
        Task<IEnumerable<DiagramInfoModel>> GetMyDiagramsAsync();
        Task<DiagramModel> GetDiagramAsync(Guid id);
        DiagramModel GetDiagram(string fileContent);
        Task<bool> SaveDiagramAsync(DiagramModel diagram);
        Task DownloadDiagramAsync(DiagramModel diagram);
    }
}