using System.Collections.Generic;

namespace NSDGenerator.Client.ViewModels;

public class DiagramVM
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsPrivate { get; set; }
    public IBlockVM RootBlock { get; set; }
    public string Owner { get; set; }
    public List<int> ColumnsWidth { get; set; } = new();

    public DiagramVM() => Id = Guid.NewGuid();

}
