using NSDGenerator.Client.ViewModels;
using NSDGenerator.Shared.Diagram;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace NSDGenerator.Client.Helpers;

public class ModelConverter : IModelConverter
{
    private readonly JsonSerializerOptions jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public string DiagramViewModelToJson(DiagramVM diagram)
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

    public DiagramVM JsonToDiagramViewModel(string json)
    {
        var dto = JsonSerializer.Deserialize<DiagramDTO>(json, jsonOptions);
        return DiagramDtoToDiagramViewModel(dto);
    }

    public DiagramVM DiagramDtoToDiagramViewModel(DiagramDTO diagramFullDto)
    {
        var rootBlock = diagramFullDto.BlockCollection is not null
            ? ConvertToBlockViewModel(diagramFullDto.BlockCollection, diagramFullDto.BlockCollection.RootId)
            : null;

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
        return ConvertToBlockViewModel(blockCollectionDto, blockCollectionDto.RootId);
    }

    public List<BranchBlockVM> RootBlockToChildrenBranchBlockViewModels(IBlockVM rootBlock)
    {
        var blocks = new List<IBlockVM>();
        GetBranchBlockViewModels(rootBlock, blocks);

        return blocks
            .Select(r => r as BranchBlockVM)
            .Where(r => r is not null)
            .ToList(); ;
    }


    private void GetBranchBlockViewModels(IBlockVM block, List<IBlockVM> result)
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
        var dto = new TextBlockDTO(tb.Id, tb.Text, tb.Child?.Id);
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

        GetBranchBlockViewModels(tb.Child, result);
        return true;
    }

    private bool TryAddAsBranchBlock(IBlockVM block, BlockCollectionDTO result)
    {
        if (block is not BranchBlockVM)
            return false;

        var bb = block as BranchBlockVM;
        var dto = new BranchBlockDTO(bb.Id, bb.Condition, bb.LeftBranch, bb.RightBranch, bb.LeftResult?.Id, bb.RightResult?.Id, bb.LeftColumns, bb.RightColumns);
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

        GetBranchBlockViewModels(bb.LeftResult, result);
        GetBranchBlockViewModels(bb.RightResult, result);
        return true;
    }

    private IBlockVM ConvertToBlockViewModel(BlockCollectionDTO blockCollectionDto, Guid currentId)
    {
        var current = blockCollectionDto.Blocks.Where(r => r.Id == currentId).SingleOrDefault();

        var tb = TryConvertToTextBlockViewModel(current, blockCollectionDto);
        if (tb is not null)
            return tb;

        var bb = TryConvertToBranchBlockViewModel(current, blockCollectionDto);
        if (bb is not null)
            return bb;

        return null;
    }

    private TextBlockVM TryConvertToTextBlockViewModel(IBlockDTO blockDto, BlockCollectionDTO blockCollectionDto)
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
            tb.Child = ConvertToBlockViewModel(blockCollectionDto, tbd.ChildId.Value);

        return tb;
    }

    private BranchBlockVM TryConvertToBranchBlockViewModel(IBlockDTO blockDto, BlockCollectionDTO jsonBlockCollection)
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
            LeftColumns = bbd.LeftColumns,
            RightColumns = bbd.RightColumns,
        };
        if (bbd.LeftResult is not null)
            bb.LeftResult = ConvertToBlockViewModel(jsonBlockCollection, bbd.LeftResult.Value);
        if (bbd.RightResult is not null)
            bb.RightResult = ConvertToBlockViewModel(jsonBlockCollection, bbd.RightResult.Value);

        return bb;
    }
}
