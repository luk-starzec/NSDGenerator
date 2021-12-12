namespace NSDGenerator.Shared.Diagram;

public class TextBlockModel : IBlockModel
{
    public IBlockModel Parent { get; set; }
    public Guid Id { get; set; }
    public string Text { get; set; }
    public IBlockModel Child { get; set; }

    public TextBlockModel()
    {
        Id = Guid.NewGuid();
    }

    public TextBlockModel(string text, IBlockModel child = null) : this()
        => (Text, Child) = (text, child);

}
