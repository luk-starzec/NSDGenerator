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
            var model = JsonSerializer.Deserialize<JsonDiagram>(json, jsonOptions);

            var rootBlock = DeserializeBlocks(model.BlockCollection, model.BlockCollection.RootId);

            var diagram = new DiagramModel
            {
                Id = model.Id,
                Name = model.Name,
                RootBlock = rootBlock,
            };

            return diagram;
        }


        private void SerializeBlocks(IBlockModel block, JsonBlockCollection result)
        {
            if (block is null)
                return;

            var tb = block as TextBlockModel;
            if (tb is not null)
            {
                var jtb = new JsonTextBlockModel(tb.Id, tb.Text, tb.Child?.Id);
                result.TextBlocks.Add(jtb);

                SerializeBlocks(tb.Child, result);
                return;
            }

            var bb = block as BranchBlockModel;
            if (bb is not null)
            {
                var jbb = new JsonBranchBlockModel(bb.Id, bb.Condition, bb.LeftBranch, bb.RightBranch, bb.LeftResult?.Id, bb.RightResult?.Id);
                result.BranchBlocks.Add(jbb);

                SerializeBlocks(bb.LeftResult, result);
                SerializeBlocks(bb.RightResult, result);
                return;
            }
        }

        private IBlockModel DeserializeBlocks(JsonBlockCollection blockCollection, Guid currentId)
        {
            var current = blockCollection.Blocks.Where(r => r.Id == currentId).SingleOrDefault();

            var jtb = current as JsonTextBlockModel;
            if (jtb is not null)
            {
                var tb = new TextBlockModel
                {
                    Id = jtb.Id,
                    Text = jtb.Text,
                };
                if (jtb.ChildId is not null)
                    tb.Child = DeserializeBlocks(blockCollection, jtb.ChildId.Value);

                return tb;
            }

            var jbb = current as JsonBranchBlockModel;
            if (jbb is not null)
            {
                var bb = new BranchBlockModel
                {
                    Id = jbb.Id,
                    Condition = jbb.Condition,
                    LeftBranch = jbb.LeftBranch,
                    RightBranch = jbb.RightBranch,
                };
                if (jbb.LeftResult is not null)
                    bb.LeftResult = DeserializeBlocks(blockCollection, jbb.LeftResult.Value);
                if (jbb.RightResult is not null)
                    bb.RightResult = DeserializeBlocks(blockCollection, jbb.RightResult.Value);

                return bb;
            }
            return null;
        }

    }
}
