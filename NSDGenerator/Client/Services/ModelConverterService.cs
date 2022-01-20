using NSDGenerator.Client.Models;
using NSDGenerator.Client.Pages;
using NSDGenerator.Shared.Diagram;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace NSDGenerator.Client.Services;

public class ModelConverterService : IModelConverterService
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

        var dto = new DiagramFullDto(diagram.Id, diagram.Name, diagram.IsPrivate, diagram.Owner, blocks, diagram.ColumnsWidth);
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

        if (rootBlock is not null)
            SetDiagramBlockColumns(rootBlock, diagramFullDto.BlockCollection.BranchBlocks);

        var diagram = new DiagramModel
        {
            Id = diagramFullDto.Id,
            Name = diagramFullDto.Name,
            IsPrivate = diagramFullDto.IsPrivate,
            Owner = diagramFullDto.Owner,
            RootBlock = rootBlock,
            ColumnsWidth = diagramFullDto.ColumnsWidth,
        };

        return diagram;
    }

    private void SetDiagramBlockColumns(IBlockModel rootBlock, List<BranchBlockDto> branchBlockDtos)
    {
        var rootBranchBlockDto = branchBlockDtos.OrderBy(r => r.Level).FirstOrDefault();

        if (rootBranchBlockDto is null)
            return;

        var branchBlocks = RootBlockToChildrenBranchBlockModels(rootBlock);
        var rootBranchBlock = branchBlocks.Single(r => r.Id == rootBranchBlockDto.Id);

        SetBranchBlocksColumnIndexes(rootBranchBlock, 0, branchBlocks);
    }


    private void SetBranchBlocksColumnIndexes(BranchBlockModel block, int index, List<BranchBlockModel> branchBlocks)
    {
        var leftBranchBlocks = RootBlockToBlockCollectionDto(block.LeftResult).BranchBlocks;
        var rightBranchBlocks = RootBlockToBlockCollectionDto(block.RightResult).BranchBlocks;

        var lStart = index;
        var lEnd = lStart + leftBranchBlocks.Count + 1;
        for (int i = lStart; i < lEnd; i++)
            block.LeftColumns.Add(i);
        SetChildrenBranchBlocksColumns(branchBlocks, leftBranchBlocks, lStart);

        var rStart = lEnd;
        var rEnd = rStart + rightBranchBlocks.Count + 1;
        for (int i = rStart; i < rEnd; i++)
            block.RightColumns.Add(i);
        SetChildrenBranchBlocksColumns(branchBlocks, rightBranchBlocks, rStart);
    }

    private void SetChildrenBranchBlocksColumns(List<BranchBlockModel> branchBlocks, List<BranchBlockDto> childBlocks, int startingColumnIndex)
    {
        foreach (var b in childBlocks.OrderBy(r => r.Level))
        {
            var bm = branchBlocks.Single(r => r.Id == b.Id);
            SetBranchBlocksColumnIndexes(bm, startingColumnIndex++, branchBlocks);
        }
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

    public List<BranchBlockModel> RootBlockToChildrenBranchBlockModels(IBlockModel rootBlock)
    {
        var blocks = new List<IBlockModel>();
        GetBranchBlockModels(rootBlock, blocks);

        return blocks
            .Select(r => r as BranchBlockModel)
            .Where(r => r is not null)
            .ToList(); ;
    }


    private void GetBranchBlockModels(IBlockModel block, List<IBlockModel> result)
    {
        if (block is null)
            return;

        if (TryAddAsTextBlock(block, result))
            return;

        if (TryAddAsBranchBlock(block, result))
            return;
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
        var parentLevel = result.Blocks.SingleOrDefault(r => r.Id == block.Parent.Id)?.Level ?? -1;
        var dto = new TextBlockDto(tb.Id, tb.Text, tb.Child?.Id, parentLevel + 1);
        result.TextBlocks.Add(dto);

        BlocksToBlockCollection(tb.Child, result);
        return true;
    }


    private bool TryAddAsTextBlock(IBlockModel block, List<IBlockModel> result)
    {
        if (block is not TextBlockModel)
            return false;

        var tb = block as TextBlockModel;
        result.Add(tb);

        GetBranchBlockModels(tb.Child, result);
        return true;
    }

    private bool TryAddAsBranchBlock(IBlockModel block, BlockCollectionDto result)
    {
        if (block is not BranchBlockModel)
            return false;

        var bb = block as BranchBlockModel;
        var parentLevel = result.Blocks.SingleOrDefault(r => r.Id == block.Parent.Id)?.Level ?? -1;
        var dto = new BranchBlockDto(bb.Id, bb.Condition, bb.LeftBranch, bb.RightBranch, bb.LeftResult?.Id, bb.RightResult?.Id, parentLevel + 1);
        result.BranchBlocks.Add(dto);

        BlocksToBlockCollection(bb.LeftResult, result);
        BlocksToBlockCollection(bb.RightResult, result);
        return true;
    }

    private bool TryAddAsBranchBlock(IBlockModel block, List<IBlockModel> result)
    {
        if (block is not BranchBlockModel)
            return false;

        var bb = block as BranchBlockModel;
        result.Add(bb);

        GetBranchBlockModels(bb.LeftResult, result);
        GetBranchBlockModels(bb.RightResult, result);
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
