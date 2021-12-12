namespace NSDGenerator.Shared.Diagram;

public class DiagramModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public IBlockModel RootBlock { get; set; }
}
