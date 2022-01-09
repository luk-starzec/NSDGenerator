using NSDGenerator.Shared.Diagram.JsonModels;
using NSDGenerator.Shared.Diagram.Models;
using System.Linq;
using System.Text.Json;

namespace NSDGenerator.Shared.Diagram.Helpers;

public class SerializationHelper : ISerializationHelper
{
    private readonly JsonSerializerOptions jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public string SerializeDiagram(DiagramModel diagram)
    {
        var blocks = new BlockCollectionJsonModel
        {
            RootId = diagram.RootBlock.Id,
        };
        SerializeBlocks(diagram.RootBlock, blocks);

        var jd = new DiagramJsonModel
        {
            Id = diagram.Id,
            Name = diagram.Name,
            IsPrivate = diagram.IsPrivate,
            BlockCollection = blocks,
        };

        var json = JsonSerializer.Serialize(jd, jsonOptions);

        return json;
    }

    public DiagramModel DeserializeDiagram(string json)
    {
        var jd = JsonSerializer.Deserialize<DiagramJsonModel>(json, jsonOptions);
        return DeserializeDiagram(jd);
    }

    public DiagramModel DeserializeDiagram(DiagramJsonModel jsonDiagram)
    {
        var rootBlock = DeserializeBlocks(jsonDiagram.BlockCollection, jsonDiagram.BlockCollection.RootId);

        var diagram = new DiagramModel
        {
            Id = jsonDiagram.Id,
            Name = jsonDiagram.Name,
            IsPrivate = jsonDiagram.IsPrivate,
            Owner = jsonDiagram.Owner,
            RootBlock = rootBlock,
        };

        return diagram;
    }


    private void SerializeBlocks(IBlockModel block, BlockCollectionJsonModel result)
    {
        if (block is null)
            return;

        if (TrySerializeTextBlock(block, result))
            return;

        if (TrySerializeBranchBlock(block, result))
            return;
    }

    private bool TrySerializeTextBlock(IBlockModel block, BlockCollectionJsonModel jsonBlockCollection)
    {
        if (block is not TextBlockModel)
            return false;

        var tb = block as TextBlockModel;
        var jtb = new TextBlockJsonModel(tb.Id, tb.Text, tb.Child?.Id);
        jsonBlockCollection.TextBlocks.Add(jtb);

        SerializeBlocks(tb.Child, jsonBlockCollection);
        return true;
    }

    private bool TrySerializeBranchBlock(IBlockModel block, BlockCollectionJsonModel jsonBlockCollection)
    {
        if (block is not BranchBlockModel)
            return false;

        var bb = block as BranchBlockModel;
        var jbb = new BranchBlockJsonModel(bb.Id, bb.Condition, bb.LeftBranch, bb.RightBranch, bb.LeftResult?.Id, bb.RightResult?.Id);
        jsonBlockCollection.BranchBlocks.Add(jbb);

        SerializeBlocks(bb.LeftResult, jsonBlockCollection);
        SerializeBlocks(bb.RightResult, jsonBlockCollection);
        return true;
    }

    private IBlockModel DeserializeBlocks(BlockCollectionJsonModel blockCollection, Guid currentId)
    {
        var current = blockCollection.Blocks.Where(r => r.Id == currentId).SingleOrDefault();

        var tb = TryDeserializeTextBlock(current, blockCollection);
        if (tb is not null)
            return tb;

        var bb = TryDeserializeBranchBlock(current, blockCollection);
        if (bb is not null)
            return bb;

        return null;
    }

    private TextBlockModel TryDeserializeTextBlock(IBlockJsonModel jsonBlock, BlockCollectionJsonModel jsonBlockCollection)
    {
        if (jsonBlock is not TextBlockJsonModel)
            return null;

        var jtb = jsonBlock as TextBlockJsonModel;
        var tb = new TextBlockModel
        {
            Id = jtb.Id,
            Text = jtb.Text,
        };
        if (jtb.ChildId is not null)
            tb.Child = DeserializeBlocks(jsonBlockCollection, jtb.ChildId.Value);

        return tb;
    }

    private BranchBlockModel TryDeserializeBranchBlock(IBlockJsonModel jsonBlock, BlockCollectionJsonModel jsonBlockCollection)
    {
        if (jsonBlock is not BranchBlockJsonModel)
            return null;

        var jbb = jsonBlock as BranchBlockJsonModel;
        var bb = new BranchBlockModel
        {
            Id = jbb.Id,
            Condition = jbb.Condition,
            LeftBranch = jbb.LeftBranch,
            RightBranch = jbb.RightBranch,
        };
        if (jbb.LeftResult is not null)
            bb.LeftResult = DeserializeBlocks(jsonBlockCollection, jbb.LeftResult.Value);
        if (jbb.RightResult is not null)
            bb.RightResult = DeserializeBlocks(jsonBlockCollection, jbb.RightResult.Value);

        return bb;
    }
}
