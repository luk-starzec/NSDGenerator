using NSDGenerator.Client.ViewModels;

namespace NSDGenerator.Client.Helpers.Columns
{
    public class AddBlockToSingleBranch
    {
        public void ApplyChanges(DiagramVM diagram, BranchBlockVM block)
        {
            SetColumns(block);
            SetColumnsWidth(diagram);
        }

        private void SetColumns(BranchBlockVM block)
        {
            block.LeftColumns = new() { 0 };
            block.RightColumns = new() { 1 };
        }

        private void SetColumnsWidth(DiagramVM diagram)
        {
            diagram.ColumnsWidth = new() { 50, 50 };
        }
    }
}
