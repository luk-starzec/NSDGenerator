namespace NSDGenerator.Shared.Diagram.JsonModels;

public class DiagramJsonModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsPrivate { get; set; }

    public string Owner { get; set; }
    public BlockCollectionJsonModel BlockCollection { get; set; }
}
