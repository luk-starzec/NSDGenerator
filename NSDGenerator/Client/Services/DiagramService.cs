using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using NSDGenerator.Client.Models;
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
    private readonly IModelConverterService modelConverterService;

    public DiagramService(ILogger<DiagramService> logger, HttpClient httpClient, IJSRuntime js, IModelConverterService modelConverterService)
    {
        this.logger = logger;
        this.httpClient = httpClient;
        this.js = js;
        this.modelConverterService = modelConverterService;
    }

    public async Task<IEnumerable<DiagramDto>> GetMyDiagramsAsync()
    {
        try
        {
            var diagrams = await httpClient.GetFromJsonAsync<IEnumerable<DiagramDto>>("api/diagram");
            return diagrams;
        }
        catch (Exception ex)
        {
            logger.LogError("Method {Method} thrown exception: {Message}", nameof(GetMyDiagramsAsync), ex.Message);
            return null;
        }
    }

    public async Task<DiagramModel> GetDiagramAsync(Guid id)
    {
        try
        {
            var dto = await httpClient.GetFromJsonAsync<DiagramFullDto>($"api/diagram/{id}");
            var diagram = modelConverterService.DiagramFullDtoToDiagramModel(dto);

            var tc = dto.BlockCollection.TextBlocks.Where(r => r.ChildId is null).Count();
            var blc = dto.BlockCollection.BranchBlocks.Where(r => r.LeftResult is null).Count();
            var brc = dto.BlockCollection.BranchBlocks.Where(r => r.RightResult is null).Count();
            var cc = tc + blc + brc;

            var branchBlocks = modelConverterService.GetBranchBlockModels(diagram.RootBlock);

            if (diagram.RootBlock is BranchBlockModel rbb)
            {
                SetBranchBlockColumns(rbb, 0, branchBlocks);

                diagram.ColumnWidths = new int[] { 10, 10, 10, 20, 50 };
            }
            return diagram;
        }
        catch (Exception ex)
        {
            logger.LogError("Method {Method} thrown exception: {Message}", nameof(GetDiagramAsync), ex.Message);
            return null;
        }
    }

    private void SetBranchBlockColumns(BranchBlockModel block, int index, List<BranchBlockModel> branchBlocks)
    {
        var leftBranchBlocks = modelConverterService.RootBlockToBlockCollectionDto(block.LeftResult).BranchBlocks;
        var rightBranchBlocks = modelConverterService.RootBlockToBlockCollectionDto(block.RightResult).BranchBlocks;

        var lStart = index;
        var lEnd = lStart + leftBranchBlocks.Count + 1;
        for (int i = lStart; i < lEnd; i++)
            block.LeftColumns.Add(i);
        SetChildBranchBlocksColumns(branchBlocks, leftBranchBlocks, lStart);

        var rStart = lEnd;
        var rEnd = rStart + rightBranchBlocks.Count + 1;
        for (int i = rStart; i < rEnd; i++)
            block.RightColumns.Add(i);
        SetChildBranchBlocksColumns(branchBlocks, rightBranchBlocks, rStart);
    }

    private void SetChildBranchBlocksColumns(List<BranchBlockModel> branchBlocks, List<BranchBlockDto> childBlocks, int startingColumnIndex)
    {
        foreach (var b in childBlocks.OrderBy(r => r.Level))
        {
            var bm = branchBlocks.Single(r => r.Id == b.Id);
            SetBranchBlockColumns(bm, startingColumnIndex++, branchBlocks);
        }
    }

    public DiagramModel GetDiagram(string fileContent)
    {
        return modelConverterService.JsonToDiagramModel(fileContent);
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

    public async Task DownloadDiagramAsync(DiagramModel diagram)
    {
        var name = GetFileName(diagram.Name);
        string content = modelConverterService.DiagramModelToJson(diagram);

        await js.InvokeVoidAsync("DownloadFile", $"{name}.json", "application/json;charset=utf-8", content);
    }

    public async Task<bool> SaveDiagramAsync(DiagramModel diagram)
    {
        try
        {
            var json = modelConverterService.DiagramModelToJson(diagram);
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

    public DiagramModel CreateDiagramCopy(DiagramModel diagram)
    {
        var rootBlockCopy = diagram.RootBlock is not null ? CopyBlockTree(diagram.RootBlock) : null;

        var copy = new DiagramModel
        {
            Name = $"Copy {diagram.Name}",
            RootBlock = rootBlockCopy,
        };
        return copy;
    }

    private IBlockModel CopyBlockTree(IBlockModel rootBlock)
    {
        var blockCollection = modelConverterService.RootBlockToBlockCollectionDto(rootBlock);

        var map = blockCollection.Blocks.ToDictionary(k => k.Id, v => Guid.NewGuid());

        var textBlocks = blockCollection.TextBlocks
            .Select(r => r with
            {
                Id = map[r.Id],
                ChildId = r.ChildId.HasValue ? map[r.ChildId.Value] : null
            })
            .ToList();
        var branchBlocks = blockCollection.BranchBlocks
            .Select(r => r with
            {
                Id = map[r.Id],
                LeftResult = r.LeftResult.HasValue ? map[r.LeftResult.Value] : null,
                RightResult = r.RightResult.HasValue ? map[r.RightResult.Value] : null
            })
            .ToList();

        var newBlockCollection = new BlockCollectionDto
        {
            RootId = map[rootBlock.Id],
            TextBlocks = textBlocks,
            BranchBlocks = branchBlocks,
        };

        var copy = modelConverterService.BlockCollectionDtoToRootBlock(newBlockCollection);

        return copy;
    }

    private string GetFileName(string diagramName)
    {
        return string.IsNullOrWhiteSpace(diagramName)
            ? "unnamed-diagram"
            : diagramName.Replace(" ", "_").ToLower();
    }
}
