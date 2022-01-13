using NSDGenerator.Client.Models;
using NSDGenerator.Shared.Diagram;
using System.Linq;
using System.Text.Json;

namespace NSDGenerator.Client.Services;

internal class ModelConverterService : IModelConverterService
{
    private readonly JsonSerializerOptions jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public string DiagramModelToJson(DiagramModel diagram)
    {
        BlockCollectionDto blocks = null;
        if (diagram.RootBlock is not null)
        {
            blocks = new BlockCollectionDto { RootId = diagram.RootBlock.Id };
            BlocksToBlockCollection(diagram.RootBlock, blocks);
        }

        var dto = new DiagramFullDto
        {
            Id = diagram.Id,
            Name = diagram.Name,
            IsPrivate = diagram.IsPrivate,
            BlockCollection = blocks,
        };
        var json = JsonSerializer.Serialize(dto, jsonOptions);

        return json;
    }

    public DiagramModel JsonToDiagramModel(string json)
    {
        var dto = JsonSerializer.Deserialize<DiagramFullDto>(json, jsonOptions);
        return DiagramFullDtoToDiagramModel(dto);
    }

    public DiagramModel DiagramFullDtoToDiagramModel(DiagramFullDto diagramFullDto)
    {
        var rootBlock = diagramFullDto.BlockCollection is not null
            ? ConvertToBlockModel(diagramFullDto.BlockCollection, diagramFullDto.BlockCollection.RootId)
            : null;

        var diagram = new DiagramModel
        {
            Id = diagramFullDto.Id,
            Name = diagramFullDto.Name,
            IsPrivate = diagramFullDto.IsPrivate,
            Owner = diagramFullDto.Owner,
            RootBlock = rootBlock,
        };

        return diagram;
    }

    public BlockCollectionDto RootBlockToBlockCollectionDto(IBlockModel rootBlock)
    {
        if (rootBlock is null)
            return null;

        var blocks = new BlockCollectionDto { RootId = rootBlock.Id, };
        BlocksToBlockCollection(rootBlock, blocks);

        return blocks;
    }

    public IBlockModel BlockCollectionDtoToRootBlock(BlockCollectionDto blockCollectionDto)
    {
        return ConvertToBlockModel(blockCollectionDto, blockCollectionDto.RootId);
    }


    private void BlocksToBlockCollection(IBlockModel block, BlockCollectionDto result)
    {
        if (block is null)
            return;

        if (TryAddAsTextBlock(block, result))
            return;

        if (TryAddAsBranchBlock(block, result))
            return;
    }

    private bool TryAddAsTextBlock(IBlockModel block, BlockCollectionDto result)
    {
        if (block is not TextBlockModel)
            return false;

        var tb = block as TextBlockModel;
        var dto = new TextBlockDto(tb.Id, tb.Text, tb.Child?.Id);
        result.TextBlocks.Add(dto);

        BlocksToBlockCollection(tb.Child, result);
        return true;
    }

    private bool TryAddAsBranchBlock(IBlockModel block, BlockCollectionDto result)
    {
        if (block is not BranchBlockModel)
            return false;

        var bb = block as BranchBlockModel;
        var dto = new BranchBlockDto(bb.Id, bb.Condition, bb.LeftBranch, bb.RightBranch, bb.LeftResult?.Id, bb.RightResult?.Id);
        result.BranchBlocks.Add(dto);

        BlocksToBlockCollection(bb.LeftResult, result);
        BlocksToBlockCollection(bb.RightResult, result);
        return true;
    }

    private IBlockModel ConvertToBlockModel(BlockCollectionDto blockCollectionDto, Guid currentId)
    {
        var current = blockCollectionDto.Blocks.Where(r => r.Id == currentId).SingleOrDefault();

        var tb = TryConvertToTextBlockModel(current, blockCollectionDto);
        if (tb is not null)
            return tb;

        var bb = TryConvertToBranchBlockModel(current, blockCollectionDto);
        if (bb is not null)
            return bb;

        return null;
    }

    private TextBlockModel TryConvertToTextBlockModel(IBlockDto blockDto, BlockCollectionDto blockCollectionDto)
    {
        if (blockDto is not TextBlockDto)
            return null;

        var tbd = blockDto as TextBlockDto;
        var tb = new TextBlockModel
        {
            Id = tbd.Id,
            Text = tbd.Text,
        };
        if (tbd.ChildId is not null)
            tb.Child = ConvertToBlockModel(blockCollectionDto, tbd.ChildId.Value);

        return tb;
    }

    private BranchBlockModel TryConvertToBranchBlockModel(IBlockDto blockDto, BlockCollectionDto jsonBlockCollection)
    {
        if (blockDto is not BranchBlockDto)
            return null;

        var bbd = blockDto as BranchBlockDto;
        var bb = new BranchBlockModel
        {
            Id = bbd.Id,
            Condition = bbd.Condition,
            LeftBranch = bbd.LeftBranch,
            RightBranch = bbd.RightBranch,
        };
        if (bbd.LeftResult is not null)
            bb.LeftResult = ConvertToBlockModel(jsonBlockCollection, bbd.LeftResult.Value);
        if (bbd.RightResult is not null)
            bb.RightResult = ConvertToBlockModel(jsonBlockCollection, bbd.RightResult.Value);

        return bb;
    }
}
