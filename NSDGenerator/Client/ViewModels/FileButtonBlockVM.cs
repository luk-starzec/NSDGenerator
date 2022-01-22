namespace NSDGenerator.Client.ViewModels;

public class FileButtonBlockVM : IBlockVM
{
    public IBlockVM Parent { get; set; }
    public Guid Id { get; set; }
    public string ButtonText { get; set; }
    public Action<string> OnFileSelected { get; set; }

    private IBlockVM child;
    public IBlockVM Child
    {
        get => child;
        set
        {
            child = value;
            if (child is null)
                child.Parent = this;
        }
    }

    public FileButtonBlockVM() => Id = Guid.NewGuid();

    public FileButtonBlockVM(string buttonText, Action<string> onFileSelected) : this()
        => (ButtonText, OnFileSelected) = (buttonText, onFileSelected);

}
