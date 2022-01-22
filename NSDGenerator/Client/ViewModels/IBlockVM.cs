namespace NSDGenerator.Client.ViewModels;

public interface IBlockVM
{
    public IBlockVM Parent { get; set; }
    public Guid Id { get; set; }
}
