using NSDGenerator.Client.Models;
using NSDGenerator.Shared.Diagram;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSDGenerator.Client.Services
{
    public interface IDiagramService
    {
        Task<IEnumerable<DiagramDto>> GetMyDiagramsAsync();
        Task<DiagramModel> GetDiagramAsync(Guid id);
        DiagramModel GetDiagram(string fileContent);
        Task<bool> SaveDiagramAsync(DiagramModel diagram);
        Task DownloadDiagramAsync(DiagramModel diagram);
    }
}