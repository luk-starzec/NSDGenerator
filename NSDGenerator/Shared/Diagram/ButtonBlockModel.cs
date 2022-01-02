namespace NSDGenerator.Shared.Diagram;

public class ButtonBlockModel : IBlockModel
{

    public IBlockModel Parent { get; set; }
    public Guid Id { get; set; }
    public string ButtonText { get; set; }
    public string AfterText { get; set; }
    public Action OnClick { get; set; }

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

    public ButtonBlockModel()
    {
        Id = Guid.NewGuid();
    }

    public ButtonBlockModel(string buttonText, Action onClick, string afterText = null) : this()
        => (ButtonText, OnClick, AfterText) = (buttonText, onClick, afterText);

}
