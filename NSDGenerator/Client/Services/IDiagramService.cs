using NSDGenerator.Client.Models;
using NSDGenerator.Shared.Diagram;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSDGenerator.Client.Services
{
    internal interface IDiagramService
    {
        Task<IEnumerable<DiagramDto>> GetMyDiagramsAsync();

        Task<DiagramModel> GetDiagramAsync(Guid id);
        DiagramModel GetDiagram(string fileContent);
        Task<bool> CheckIfDiagramExistsAsync(Guid id);

        Task<bool> SaveDiagramAsync(DiagramModel diagram);
        Task DownloadDiagramAsync(DiagramModel diagram);

        Task<bool> DeleteDiagramAsync(Guid id);

        DiagramModel CreateDiagramCopy(DiagramModel diagram);

    }
}