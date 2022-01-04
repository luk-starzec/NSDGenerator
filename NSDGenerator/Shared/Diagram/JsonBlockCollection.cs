using System.Collections.Generic;
using System.Linq;

namespace NSDGenerator.Shared.Diagram;

public record JsonBlockCollection
{
    public Guid RootId { get; init; }
    public List<JsonTextBlockModel> TextBlocks { get; init; } = new();
    public List<JsonBranchBlockModel> BranchBlocks { get; init; } = new();

    public IEnumerable<IJsonBlockModel> Blocks
        => TextBlocks.Select(r => r as IJsonBlockModel)
            .Union(BranchBlocks.Select(r => r as IJsonBlockModel));
}