using System.Collections.Generic;
using System.Linq;

namespace NSDGenerator.Shared.Diagram;

public record BlockCollectionDto
{
    public Guid RootId { get; init; }
    public List<TextBlockDlo> TextBlocks { get; init; } = new();
    public List<BranchBlockDto> BranchBlocks { get; init; } = new();

    public IEnumerable<IBlockDto> Blocks
        => TextBlocks.Select(r => r as IBlockDto)
            .Union(BranchBlocks.Select(r => r as IBlockDto));
}