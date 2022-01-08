namespace NSDGenerator.Shared.Diagram.Models;

public class FileButtonBlockModel : IBlockModel
{

    public IBlockModel Parent { get; set; }
    public Guid Id { get; set; }
    public string ButtonText { get; set; }
    public Action<string> OnFileSelected { get; set; }

    private IBlockModel child;
    public IBlockModel Child
    {
        get => child;
        set
        {
            child = value;
            if (child is null)
                child.Parent = this;
        }
    }

    public FileButtonBlockModel()
    {
        Id = Guid.NewGuid();
    }

    public FileButtonBlockModel(string buttonText, Action<string> onFileSelected) : this()
        => (ButtonText, OnFileSelected) = (buttonText, onFileSelected);

}
