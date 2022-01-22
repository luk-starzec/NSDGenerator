using NSDGenerator.Client.ViewModels;
using NSDGenerator.Shared.Diagram;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSDGenerator.Client.Services
{
    internal interface IDiagramService
    {
        Task<IEnumerable<DiagramInfoDTO>> GetMyDiagramsAsync();

        Task<DiagramVM> GetDiagramAsync(Guid id);
        DiagramVM GetDiagram(string fileContent);
        Task<bool> CheckIfDiagramExistsAsync(Guid id);

        Task<bool> SaveDiagramAsync(DiagramVM diagram);
        Task DownloadDiagramAsync(DiagramVM diagram);

        Task<bool> DeleteDiagramAsync(Guid id);

        DiagramVM CreateNewDiagram();
        DiagramVM CreateDiagramCopy(DiagramVM diagram);

    }
}