using NSDGenerator.Client.Pages;
using NSDGenerator.Shared.Diagram;
using System.Threading.Tasks;

namespace NSDGenerator.Client.Services
{
    public interface IPersistenceService
    {
        Task DownloadDiagram(DiagramModel diagram);
    }
}