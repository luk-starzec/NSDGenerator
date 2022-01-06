using NSDGenerator.Shared.Diagram;

namespace NSDGenerator.Client.Services
{
    public interface ISerializeService
    {
        DiagramModel DeserializeDiagram(string json);
        DiagramModel DeserializeDiagram(JsonDiagram jsonDiagram);

        string SerializeDiagram(DiagramModel diagram);
    }
}