using NSDGenerator.Server.DbData;
using NSDGenerator.Shared.Diagram;
using System.Linq;
using System.Text.Json;
using System;
using NSDGenerator.Server.Diagram.Models;

namespace NSDGenerator.Server.Diagram.Helpers;

public class DtoConverters : IDtoConverters
{
    private readonly JsonSerializerOptions jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public DtoConverters(JsonSerializerOptions jsonOptions = null)
    {
        if (jsonOptions is not null)
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

    public string TextBlockDtoToJson(TextBlockDto dto)
    {
        var content = new TextBlockJsonData(dto.Text, dto.ChildId);
        return JsonSerializer.Serialize(content, jsonOptions);
    }

    public string BranchBlockDtoToJson(BranchBlockDto dto)
    {
        var content = new BranchBlockJsonData(dto.Condition, dto.LeftBranch, dto.RightBranch, dto.LeftResult, dto.RightResult);
        return JsonSerializer.Serialize(content, jsonOptions);
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
