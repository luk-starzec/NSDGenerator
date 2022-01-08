using System.Collections.Generic;
using System.Linq;

namespace NSDGenerator.Shared.Diagram.JsonModels;

public record BlockCollectionJsonModel
{
    public Guid RootId { get; init; }
    public List<TextBlockJsonModel> TextBlocks { get; init; } = new();
    public List<BranchBlockJsonModel> BranchBlocks { get; init; } = new();

    public IEnumerable<IBlockJsonModel> Blocks
        => TextBlocks.Select(r => r as IBlockJsonModel)
            .Union(BranchBlocks.Select(r => r as IBlockJsonModel));
}