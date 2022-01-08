using NSDGenerator.Shared.Diagram.Models;
using System;

namespace NSDGenerator.Client.Helpers;

public static class DiagramHelpers
{
    public static DiagramModel GetNewDiagram()
        => new DiagramModel
        {
            Id = Guid.NewGuid(),
            Name = "New diagram",
        };
}
