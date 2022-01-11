using NSDGenerator.Client.Models;
using NSDGenerator.Shared.Diagram;
using System.Linq;
using System.Text.Json;

namespace NSDGenerator.Client.Helpers;

public class SerializationHelper : ISerializationHelper
{
    private readonly JsonSerializerOptions jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public string SerializeDiagram(DiagramModel diagram)
    {
        var blocks = new BlockCollectionDto
        {
            RootId = diagram.RootBlock.Id,
        };
        SerializeBlocks(diagram.RootBlock, blocks);

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

    public DiagramModel DeserializeDiagram(string json)
    {
        var dto = JsonSerializer.Deserialize<DiagramFullDto>(json, jsonOptions);
        return DeserializeDiagram(dto);
    }

    public DiagramModel DeserializeDiagram(DiagramFullDto diagramFullDto)
    {
        var rootBlock = DeserializeBlocks(diagramFullDto.BlockCollection, diagramFullDto.BlockCollection.RootId);

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


    private void SerializeBlocks(IBlockModel block, BlockCollectionDto result)
    {
        if (block is null)
            return;

        if (TrySerializeTextBlock(block, result))
            return;

        if (TrySerializeBranchBlock(block, result))
            return;
    }

    private bool TrySerializeTextBlock(IBlockModel block, BlockCollectionDto blockCollectionDto)
    {
        if (block is not TextBlockModel)
            return false;

        var tb = block as TextBlockModel;
        var jtb = new TextBlockDto(tb.Id, tb.Text, tb.Child?.Id);
        blockCollectionDto.TextBlocks.Add(jtb);

        SerializeBlocks(tb.Child, blockCollectionDto);
        return true;
    }

    private bool TrySerializeBranchBlock(IBlockModel block, BlockCollectionDto blockCollectionDto)
    {
        if (block is not BranchBlockModel)
            return false;

        var bb = block as BranchBlockModel;
        var jbb = new BranchBlockDto(bb.Id, bb.Condition, bb.LeftBranch, bb.RightBranch, bb.LeftResult?.Id, bb.RightResult?.Id);
        blockCollectionDto.BranchBlocks.Add(jbb);

        SerializeBlocks(bb.LeftResult, blockCollectionDto);
        SerializeBlocks(bb.RightResult, blockCollectionDto);
        return true;
    }

    private IBlockModel DeserializeBlocks(BlockCollectionDto blockCollectionDto, Guid currentId)
    {
        var current = blockCollectionDto.Blocks.Where(r => r.Id == currentId).SingleOrDefault();

        var tb = TryDeserializeTextBlock(current, blockCollectionDto);
        if (tb is not null)
            return tb;

        var bb = TryDeserializeBranchBlock(current, blockCollectionDto);
        if (bb is not null)
            return bb;

        return null;
    }

    private TextBlockModel TryDeserializeTextBlock(IBlockDto blockDto, BlockCollectionDto blockCollectionDto)
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
            tb.Child = DeserializeBlocks(blockCollectionDto, tbd.ChildId.Value);

        return tb;
    }

    private BranchBlockModel TryDeserializeBranchBlock(IBlockDto blockDto, BlockCollectionDto jsonBlockCollection)
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
            bb.LeftResult = DeserializeBlocks(jsonBlockCollection, bbd.LeftResult.Value);
        if (bbd.RightResult is not null)
            bb.RightResult = DeserializeBlocks(jsonBlockCollection, bbd.RightResult.Value);

        return bb;
    }
}
