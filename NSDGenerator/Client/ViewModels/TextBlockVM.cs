namespace NSDGenerator.Client.ViewModels;

public class TextBlockVM : IBlockVM
{
    public IBlockVM Parent { get; set; }
    public Guid Id { get; set; }
    public string Text { get; set; }

    private IBlockVM child;
    public IBlockVM Child
    {
        get => child;
        set
        {
            child = value;
            if (child is not null)
                child.Parent = this;
        }
    }

    public TextBlockVM() => Id = Guid.NewGuid();

    public TextBlockVM(string text, IBlockVM child = null) : this()
        => (Text, Child) = (text, child);
}
