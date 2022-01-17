namespace NSDGenerator.Client.Models;

public class DiagramModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsPrivate { get; set; }
    public IBlockModel RootBlock { get; set; }
    public string Owner { get; set; }

    public DiagramModel() => Id = Guid.NewGuid();

    public int[] ColumnWidths { get; set; } = new int[0];
}
