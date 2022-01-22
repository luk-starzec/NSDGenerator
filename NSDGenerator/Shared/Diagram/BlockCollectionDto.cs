using System.Collections.Generic;
using System.Linq;

namespace NSDGenerator.Shared.Diagram;

public record BlockCollectionDTO
{
    public Guid RootId { get; init; }
    public List<TextBlockDTO> TextBlocks { get; init; } = new();
    public List<BranchBlockDTO> BranchBlocks { get; init; } = new();

    public IEnumerable<IBlockDTO> Blocks
        => TextBlocks.Select(r => r as IBlockDTO)
            .Union(BranchBlocks.Select(r => r as IBlockDTO));
}