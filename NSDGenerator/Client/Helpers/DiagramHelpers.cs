using NSDGenerator.Shared.Diagram;
using System;

namespace NSDGenerator.Client.Helpers;

public static class DiagramHelpers
{
    public static DiagramModel GetNewDiagram()
        => new DiagramModel
        {
            Id = Guid.NewGuid(),
            Name = "New diagram",
            RootBlock = new TextBlockModel("Start step", new TextBlockModel("sdsdsd")),
        };

    public static bool IsRootBlock(IBlockModel block)
        => block?.Parent == null;

    //public static bool IsRootBlock(DiagramModel diagram, Guid blockId)
    //=> diagram?.RootBlock?.Id == blockId;

}
