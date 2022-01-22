using NSDGenerator.Client.ViewModels;

namespace NSDGenerator.Client.Helpers.Columns
{
    public class AddBlockToRootBranch
    {
        private readonly IColumnsViewCalculator columnsViewCalculator;

        public AddBlockToRootBranch(IColumnsViewCalculator columnsViewCalculator)
        {
            this.columnsViewCalculator = columnsViewCalculator;
        }

        public void ApplyChanges(DiagramVM diagram, BranchBlockVM block)
        {
            SetColumns(block, diagram);
            SetColumnsWidth(diagram);
        }

        private void SetColumns(BranchBlockVM block, DiagramVM diagram)
        {
            for (int i = 0; i < diagram.ColumnsWidth.Count; i++)
                block.LeftColumns.Add(i);

            block.RightColumns = new() { diagram.ColumnsWidth.Count };
        }

        private void SetColumnsWidth(DiagramVM diagram)
        {
            for (int i = 0; i < diagram.ColumnsWidth.Count; i++)
                diagram.ColumnsWidth[i] = diagram.ColumnsWidth[i] / 2;

            diagram.ColumnsWidth.Add(50);

            diagram.ColumnsWidth = columnsViewCalculator.RecalculateColumnsWidth(diagram.ColumnsWidth);
        }

    }
}
