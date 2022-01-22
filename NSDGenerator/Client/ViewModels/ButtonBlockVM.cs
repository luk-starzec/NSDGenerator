namespace NSDGenerator.Client.ViewModels;

public class ButtonBlockVM : IBlockVM
{
    public IBlockVM Parent { get; set; }
    public Guid Id { get; set; }
    public string ButtonText { get; set; }
    public string AfterText { get; set; }
    public Action OnClick { get; set; }

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

    public ButtonBlockVM() => Id = Guid.NewGuid();

    public ButtonBlockVM(string buttonText, Action onClick, string afterText = null) : this()
        => (ButtonText, OnClick, AfterText) = (buttonText, onClick, afterText);

}
