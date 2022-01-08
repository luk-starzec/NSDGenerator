using NSDGenerator.Shared.Diagram.JsonModels;
using NSDGenerator.Shared.Diagram.Models;

namespace NSDGenerator.Shared.Diagram.Helpers;

public interface ISerializationHelper
{
    DiagramModel DeserializeDiagram(string json);
    DiagramModel DeserializeDiagram(DiagramJsonModel jsonDiagram);

    string SerializeDiagram(DiagramModel diagram);
}
