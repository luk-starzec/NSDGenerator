using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using NSDGenerator.Client.Helpers;
using NSDGenerator.Client.ViewModels;
using NSDGenerator.Shared.Diagram;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace NSDGenerator.Client.Services;

internal class DiagramService : IDiagramService
{
    private readonly ILogger<DiagramService> logger;
    private readonly HttpClient httpClient;
    private readonly IJSRuntime js;
    private readonly IModelConverter modelConverter;

    public DiagramService(ILogger<DiagramService> logger, HttpClient httpClient, IJSRuntime js, IModelConverter modelConverter)
    {
        this.logger = logger;
        this.httpClient = httpClient;
        this.js = js;
        this.modelConverter = modelConverter;
    }

    public async Task<IEnumerable<DiagramInfoDTO>> GetMyDiagramsAsync()
    {
        try
        {
            var diagrams = await httpClient.GetFromJsonAsync<IEnumerable<DiagramInfoDTO>>("api/diagram");
            return diagrams;
        }
        catch (Exception ex)
        {
            logger.LogError("Method {Method} thrown exception: {Message}", nameof(GetMyDiagramsAsync), ex.Message);
            return null;
        }
    }

    public async Task<DiagramVM> GetDiagramAsync(Guid id)
    {
        try
        {
            var dto = await httpClient.GetFromJsonAsync<DiagramDTO>($"api/diagram/{id}");
            var diagram = modelConverter.DiagramDtoToDiagramViewModel(dto);
            return diagram;
        }
        catch (Exception ex)
        {
            logger.LogError("Method {Method} thrown exception: {Message}", nameof(GetDiagramAsync), ex.Message);
            return null;
        }
    }

    public int GetColumnsCount(IBlockVM rootBlock)
    {
        var branchBlocks = modelConverter.RootBlockToChildrenBranchBlockViewModels(rootBlock);
        return branchBlocks.Count + 1;
    }

    public DiagramVM GetDiagram(string fileContent)
    {
        return modelConverter.JsonToDiagramViewModel(fileContent);
    }

    public async Task<bool> CheckIfDiagramExistsAsync(Guid id)
    {
        try
        {
            var result = await httpClient.GetAsync($"api/diagram/exists/{id}");
            return result.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            logger.LogError("Method {Method} thrown exception: {Message}", nameof(CheckIfDiagramExistsAsync), ex.Message);
            return false;
        }

    }

    public async Task DownloadDiagramAsync(DiagramVM diagram)
    {
        var name = GetFileName(diagram.Name);
        string content = modelConverter.DiagramViewModelToJson(diagram);

        await js.InvokeVoidAsync("DownloadFile", $"{name}.nsd", "application/json;charset=utf-8", content);
    }

    public async Task<bool> SaveDiagramAsync(DiagramVM diagram)
    {
        try
        {
            var json = modelConverter.DiagramViewModelToJson(diagram);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"api/diagram", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            logger.LogError("Method {Method} thrown exception: {Message}", nameof(SaveDiagramAsync), ex.Message);
            return false;
        }
    }

    public async Task<bool> DeleteDiagramAsync(Guid id)
    {
        try
        {
            var response = await httpClient.DeleteAsync($"api/diagram/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            logger.LogError("Method {Method} thrown exception: {Message}", nameof(DeleteDiagramAsync), ex.Message);
            return false;
        }
    }

    public DiagramVM CreateNewDiagram()
    {
        return new DiagramVM { Name = "New diagram" };
    }

    public DiagramVM CreateDiagramCopy(DiagramVM diagram)
    {
        var rootBlockCopy = diagram.RootBlock is not null ? CopyBlockTree(diagram.RootBlock) : null;

        var copy = new DiagramVM
        {
            Name = $"Copy {diagram.Name}",
            RootBlock = rootBlockCopy,
            ColumnsWidth = diagram.ColumnsWidth.ToList(),
        };
        return copy;
    }

    private IBlockVM CopyBlockTree(IBlockVM rootBlock)
    {
        var blockCollection = modelConverter.RootBlockToBlockCollectionDto(rootBlock);

        var map = blockCollection.Blocks.ToDictionary(k => k.Id, v => Guid.NewGuid());

        var textBlocks = blockCollection.TextBlocks
            .Select(r => r with
            {
                Id = map[r.Id],
                ChildId = r.ChildId.HasValue ? map[r.ChildId.Value] : null,
            })
            .ToList();
        var branchBlocks = blockCollection.BranchBlocks
            .Select(r => r with
            {
                Id = map[r.Id],
                LeftResult = r.LeftResult.HasValue ? map[r.LeftResult.Value] : null,
                RightResult = r.RightResult.HasValue ? map[r.RightResult.Value] : null,
                LeftColumns = r.LeftColumns.ToList(),
                RightColumns = r.RightColumns.ToList(),
            })
            .ToList();

        var newBlockCollection = new BlockCollectionDTO
        {
            RootId = map[rootBlock.Id],
            TextBlocks = textBlocks,
            BranchBlocks = branchBlocks,
        };

        var copy = modelConverter.BlockCollectionDtoToRootBlock(newBlockCollection);

        return copy;
    }

    private string GetFileName(string diagramName)
    {
        return string.IsNullOrWhiteSpace(diagramName)
            ? "unnamed-diagram"
            : diagramName.Replace(" ", "_").ToLower();
    }
}
