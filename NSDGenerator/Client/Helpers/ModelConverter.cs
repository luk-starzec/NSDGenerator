using NSDGenerator.Client.ViewModels;
using NSDGenerator.Shared.Diagram;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace NSDGenerator.Client.Helpers;

public class ModelConverter : IModelConverter
{
    private readonly JsonSerializerOptions jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public string DiagramModelToJson(DiagramVM diagram)
    {
        BlockCollectionDTO blocks = null;
        if (diagram.RootBlock is not null)
        {
            blocks = new BlockCollectionDTO { RootId = diagram.RootBlock.Id };
            BlocksToBlockCollection(diagram.RootBlock, blocks);
        }

        var dto = new DiagramDTO(diagram.Id, diagram.Name, diagram.IsPrivate, diagram.Owner, blocks, diagram.ColumnsWidth);
        var json = JsonSerializer.Serialize(dto, jsonOptions);

        return json;
    }

    public DiagramVM JsonToDiagramModel(string json)
    {
        var dto = JsonSerializer.Deserialize<DiagramDTO>(json, jsonOptions);
        return DiagramFullDtoToDiagramModel(dto);
    }

    public DiagramVM DiagramFullDtoToDiagramModel(DiagramDTO diagramFullDto)
    {
        var rootBlock = diagramFullDto.BlockCollection is not null
            ? ConvertToBlockModel(diagramFullDto.BlockCollection, diagramFullDto.BlockCollection.RootId)
            : null;

        if (rootBlock is not null)
            SetDiagramBlockColumns(rootBlock, diagramFullDto.BlockCollection.BranchBlocks);

        var diagram = new DiagramVM
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

    private void SetDiagramBlockColumns(IBlockVM rootBlock, List<BranchBlockDTO> branchBlockDtos)
    {
        var rootBranchBlockDto = branchBlockDtos.OrderBy(r => r.Level).FirstOrDefault();

        if (rootBranchBlockDto is null)
            return;

        var branchBlocks = RootBlockToChildrenBranchBlockModels(rootBlock);
        var rootBranchBlock = branchBlocks.Single(r => r.Id == rootBranchBlockDto.Id);

        SetBranchBlocksColumnIndexes(rootBranchBlock, 0, branchBlocks);
    }


    private void SetBranchBlocksColumnIndexes(BranchBlockVM block, int index, List<BranchBlockVM> branchBlocks)
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

    private void SetChildrenBranchBlocksColumns(List<BranchBlockVM> branchBlocks, List<BranchBlockDTO> childBlocks, int startingColumnIndex)
    {
        foreach (var b in childBlocks.OrderBy(r => r.Level))
        {
            var bm = branchBlocks.Single(r => r.Id == b.Id);
            SetBranchBlocksColumnIndexes(bm, startingColumnIndex++, branchBlocks);
        }
    }


    public BlockCollectionDTO RootBlockToBlockCollectionDto(IBlockVM rootBlock)
    {
        if (rootBlock is null)
            return null;

        var blocks = new BlockCollectionDTO { RootId = rootBlock.Id, };
        BlocksToBlockCollection(rootBlock, blocks);

        return blocks;
    }

    public IBlockVM BlockCollectionDtoToRootBlock(BlockCollectionDTO blockCollectionDto)
    {
        return ConvertToBlockModel(blockCollectionDto, blockCollectionDto.RootId);
    }

    public List<BranchBlockVM> RootBlockToChildrenBranchBlockModels(IBlockVM rootBlock)
    {
        var blocks = new List<IBlockVM>();
        GetBranchBlockModels(rootBlock, blocks);

        return blocks
            .Select(r => r as BranchBlockVM)
            .Where(r => r is not null)
            .ToList(); ;
    }


    private void GetBranchBlockModels(IBlockVM block, List<IBlockVM> result)
    {
        if (block is null)
            return;

        if (TryAddAsTextBlock(block, result))
            return;

        if (TryAddAsBranchBlock(block, result))
            return;
    }

    private void BlocksToBlockCollection(IBlockVM block, BlockCollectionDTO result)
    {
        if (block is null)
            return;

        if (TryAddAsTextBlock(block, result))
            return;

        if (TryAddAsBranchBlock(block, result))
            return;
    }

    private bool TryAddAsTextBlock(IBlockVM block, BlockCollectionDTO result)
    {
        if (block is not TextBlockVM)
            return false;

        var tb = block as TextBlockVM;
        var parentLevel = result.Blocks.SingleOrDefault(r => r.Id == block.Parent.Id)?.Level ?? -1;
        var dto = new TextBlockDTO(tb.Id, tb.Text, tb.Child?.Id, parentLevel + 1);
        result.TextBlocks.Add(dto);

        BlocksToBlockCollection(tb.Child, result);
        return true;
    }


    private bool TryAddAsTextBlock(IBlockVM block, List<IBlockVM> result)
    {
        if (block is not TextBlockVM)
            return false;

        var tb = block as TextBlockVM;
        result.Add(tb);

        GetBranchBlockModels(tb.Child, result);
        return true;
    }

    private bool TryAddAsBranchBlock(IBlockVM block, BlockCollectionDTO result)
    {
        if (block is not BranchBlockVM)
            return false;

        var bb = block as BranchBlockVM;
        var parentLevel = result.Blocks.SingleOrDefault(r => r.Id == block.Parent.Id)?.Level ?? -1;
        var dto = new BranchBlockDTO(bb.Id, bb.Condition, bb.LeftBranch, bb.RightBranch, bb.LeftResult?.Id, bb.RightResult?.Id, parentLevel + 1);
        result.BranchBlocks.Add(dto);

        BlocksToBlockCollection(bb.LeftResult, result);
        BlocksToBlockCollection(bb.RightResult, result);
        return true;
    }

    private bool TryAddAsBranchBlock(IBlockVM block, List<IBlockVM> result)
    {
        if (block is not BranchBlockVM)
            return false;

        var bb = block as BranchBlockVM;
        result.Add(bb);

        GetBranchBlockModels(bb.LeftResult, result);
        GetBranchBlockModels(bb.RightResult, result);
        return true;
    }

    private IBlockVM ConvertToBlockModel(BlockCollectionDTO blockCollectionDto, Guid currentId)
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

    private TextBlockVM TryConvertToTextBlockModel(IBlockDTO blockDto, BlockCollectionDTO blockCollectionDto)
    {
        if (blockDto is not TextBlockDTO)
            return null;

        var tbd = blockDto as TextBlockDTO;
        var tb = new TextBlockVM
        {
            Id = tbd.Id,
            Text = tbd.Text,
        };
        if (tbd.ChildId is not null)
            tb.Child = ConvertToBlockModel(blockCollectionDto, tbd.ChildId.Value);

        return tb;
    }

    private BranchBlockVM TryConvertToBranchBlockModel(IBlockDTO blockDto, BlockCollectionDTO jsonBlockCollection)
    {
        if (blockDto is not BranchBlockDTO)
            return null;

        var bbd = blockDto as BranchBlockDTO;
        var bb = new BranchBlockVM
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
