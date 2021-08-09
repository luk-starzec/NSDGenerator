using NSDGenerator.Shared.Diagram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NSDGenerator.Client.Helpers
{
    public static class DiagramHelpers
    {
        public static DiagramModel GetNewDiagram()
        {
            return new DiagramModel
            {
                Id = Guid.NewGuid(),
                Name = "New diagram",
                RootBlock = new TextBlockModel("Start step", new TextBlockModel("sdsdsd")),
            };
        }
    }
}
