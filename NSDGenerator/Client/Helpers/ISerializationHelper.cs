using NSDGenerator.Client.Models;
using NSDGenerator.Shared.Diagram;

namespace NSDGenerator.Client.Helpers;

public interface ISerializationHelper
{
    DiagramModel DeserializeDiagram(string json);
    DiagramModel DeserializeDiagram(DiagramFullDto diagramFullDto);

    string SerializeDiagram(DiagramModel diagram);
}
