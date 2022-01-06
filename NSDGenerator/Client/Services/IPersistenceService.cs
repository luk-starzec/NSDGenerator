using NSDGenerator.Client.Pages;
using NSDGenerator.Shared.Diagram;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSDGenerator.Client.Services
{
    public interface IPersistenceService
    {
        Task DownloadDiagramAsync(DiagramModel diagram);
        Task<DiagramModel> GetDiagramAsync(Guid id);
        Task<IEnumerable<DiagramInfoModel>> GetDiagramsAsync();
    }
}