using NSDGenerator.Shared.Diagram;

namespace NSDGenerator.Client.Services
{
    public interface ISerializeService
    {
        DiagramModel DeserializeDiagram(string json);
        string SerializeDiagram(DiagramModel diagram);
    }
}