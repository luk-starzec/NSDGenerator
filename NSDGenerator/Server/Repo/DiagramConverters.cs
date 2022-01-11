using NSDGenerator.Server.Data;
using NSDGenerator.Shared.Diagram;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System;

namespace NSDGenerator.Server.Repo
{
    public class DiagramConverters
    {
        private readonly JsonSerializerOptions jsonOptions;

        public DiagramConverters(JsonSerializerOptions jsonOptions)
        {
            this.jsonOptions = jsonOptions;
        }

        public BlockCollectionDto BlocksToBlockCollectionDto(Block[] blocks, Guid rootId)
        {
            var text = blocks
                .Where(r => r.BlockType == EnumBlockType.Text)
                .Select(r => BlockToTextBlockDto(r))
                .ToList();
            var branch = blocks
                .Where(r => r.BlockType == EnumBlockType.Branch)
                .Select(r => BlockToBranchBlockDto(r))
                .ToList();

            return new BlockCollectionDto
            {
                RootId = rootId,
                TextBlocks = text,
                BranchBlocks = branch,
            };
        }

        private TextBlockDto BlockToTextBlockDto(Block block)
        {
            var content = JsonSerializer.Deserialize<TextBlockJsonData>(block.JsonData, jsonOptions);
            return new TextBlockDto(block.Id, content.Text, content.ChildId);
        }

        private BranchBlockDto BlockToBranchBlockDto(Block block)
        {
            var content = JsonSerializer.Deserialize<BranchBlockJsonData>(block.JsonData, jsonOptions);
            return new BranchBlockDto(block.Id, content.Condition, content.LeftBranch, content.RightBranch, content.LeftResult, content.RightResult);
        }

    }
}
