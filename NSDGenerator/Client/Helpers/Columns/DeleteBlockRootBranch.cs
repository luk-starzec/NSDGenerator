using NSDGenerator.Client.ViewModels;

namespace NSDGenerator.Client.Helpers.Columns
{
    public class DeleteBlockRootBranch
    {
        public void ApplyChanges(DiagramVM diagram)
        {
            diagram.ColumnsWidth = new() { 100 };
        }
    }
}
