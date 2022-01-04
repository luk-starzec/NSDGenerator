using NSDGenerator.Shared.Diagram;
using System.Linq;
using System;
using System.Text.Json;

namespace NSDGenerator.Client.Services
{
    public class SerializeService : ISerializeService
    {
        private readonly JsonSerializerOptions jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public string SerializeDiagram(DiagramModel diagram)
        {
            var blocks = new JsonBlockCollection
            {
                RootId = diagram.RootBlock.Id,
            };
            SerializeBlocks(diagram.RootBlock, blocks);

            var jd = new JsonDiagram
            {
                Id = diagram.Id,
                Name = diagram.Name,
                BlockCollection = blocks,
            };

            var json = JsonSerializer.Serialize(jd, jsonOptions);

            return json;

        }

        public DiagramModel DeserializeDiagram(string json)
        {
            var jd = JsonSerializer.Deserialize<JsonDiagram>(json, jsonOptions);

            var rootBlock = DeserializeBlocks(jd.BlockCollection, jd.BlockCollection.RootId);

            var diagram = new DiagramModel
            {
                Id = jd.Id,
                Name = jd.Name,
                RootBlock = rootBlock,
            };

            return diagram;
        }


        private void SerializeBlocks(IBlockModel block, JsonBlockCollection result)
        {
            if (block is null)
                return;

            if (TrySerializeTextBlock(block, result))
                return;

            if (TrySerializeBranchBlock(block, result))
                return;
        }

        private bool TrySerializeTextBlock(IBlockModel block, JsonBlockCollection jsonBlockCollection)
        {
            if (block is not TextBlockModel)
                return false;

            var tb = block as TextBlockModel;
            var jtb = new JsonTextBlockModel(tb.Id, tb.Text, tb.Child?.Id);
            jsonBlockCollection.TextBlocks.Add(jtb);

            SerializeBlocks(tb.Child, jsonBlockCollection);
            return true;
        }

        private bool TrySerializeBranchBlock(IBlockModel block, JsonBlockCollection jsonBlockCollection)
        {
            if (block is not BranchBlockModel)
                return false;

            var bb = block as BranchBlockModel;
            var jbb = new JsonBranchBlockModel(bb.Id, bb.Condition, bb.LeftBranch, bb.RightBranch, bb.LeftResult?.Id, bb.RightResult?.Id);
            jsonBlockCollection.BranchBlocks.Add(jbb);

            SerializeBlocks(bb.LeftResult, jsonBlockCollection);
            SerializeBlocks(bb.RightResult, jsonBlockCollection);
            return true;
        }

        private IBlockModel DeserializeBlocks(JsonBlockCollection blockCollection, Guid currentId)
        {
            var current = blockCollection.Blocks.Where(r => r.Id == currentId).SingleOrDefault();

            var tb = TryDeserializeTextBlock(current, blockCollection);
            if (tb is not null)
                return tb;

            var bb = TryDeserializeBranchBlock(current, blockCollection);
            if(bb is not null)
                return bb;

            return null;
        }

        private TextBlockModel TryDeserializeTextBlock(IJsonBlockModel jsonBlock, JsonBlockCollection jsonBlockCollection)
        {
            if (jsonBlock is not JsonTextBlockModel)
                return null;

            var jtb = jsonBlock as JsonTextBlockModel;
            var tb = new TextBlockModel
            {
                Id = jtb.Id,
                Text = jtb.Text,
            };
            if (jtb.ChildId is not null)
                tb.Child = DeserializeBlocks(jsonBlockCollection, jtb.ChildId.Value);

            return tb;
        }

        private BranchBlockModel TryDeserializeBranchBlock(IJsonBlockModel jsonBlock, JsonBlockCollection jsonBlockCollection)
        {
            if (jsonBlock is not JsonBranchBlockModel)
                return null;

            var jbb = jsonBlock as JsonBranchBlockModel;
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
}
