namespace NSDGenerator.Client.Models;

public partial class TextBlockModel : IBlockModel
{
    public IBlockModel Parent { get; set; }
    public Guid Id { get; set; }
    public string Text { get; set; }

    private IBlockModel child;
    public IBlockModel Child
    {
        get => child;
        set
        {
            child = value;
            if (child is not null)
                child.Parent = this;
        }
    }

    public TextBlockModel()
    {
        Id = Guid.NewGuid();
    }

    public TextBlockModel(string text, IBlockModel child = null) : this()
        => (Text, Child) = (text, child);
}
