namespace NSDGenerator.Shared.Diagram;

public class DiagramFullDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsPrivate { get; set; }

    public string Owner { get; set; }
    public BlockCollectionDto BlockCollection { get; set; }
}
