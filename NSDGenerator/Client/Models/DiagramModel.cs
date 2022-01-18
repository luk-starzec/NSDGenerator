using System.Collections.Generic;

namespace NSDGenerator.Client.Models;

public class DiagramModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsPrivate { get; set; }
    public IBlockModel RootBlock { get; set; }
    public string Owner { get; set; }

    public DiagramModel() => Id = Guid.NewGuid();

    public List<int> ColumnWidths { get; set; } = new();
}
