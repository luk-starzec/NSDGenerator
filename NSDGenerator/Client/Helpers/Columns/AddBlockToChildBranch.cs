using NSDGenerator.Client.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace NSDGenerator.Client.Helpers.Columns
{
    public class AddBlockToChildBranch
    {
        private readonly IModelConverter modelConverter;
        private readonly IColumnsViewCalculator columnsViewCalculator;

        public AddBlockToChildBranch(IModelConverter modelConverter, IColumnsViewCalculator columnsViewCalculator)
        {
            this.modelConverter = modelConverter;
            this.columnsViewCalculator = columnsViewCalculator;
        }

        public void ApplyChanges(DiagramVM diagram, BranchBlockVM block)
        {
            var index = ColumnsHelper.GetParentColumnIndex(block);

            if (index is null)
                throw new Exception("Parent block not found");

            SetColumns(block, index.Value, diagram.RootBlock);
            SetColumnsWidth(diagram, index.Value);
        }

        private void SetColumns(BranchBlockVM block, int index, IBlockVM diagramRootBlock)
        {
            var blocks = modelConverter.RootBlockToChildrenBranchBlockModels(diagramRootBlock);

            var greaterLeft = blocks.Where(r => r.LeftColumns.Where(rr => rr >= index).Any()).ToArray();
            var greaterRight = blocks.Where(r => r.RightColumns.Where(rr => rr >= index).Any()).ToArray();

            var containsLeft = blocks.Where(r => r.LeftColumns.Contains(index)).ToArray();
            var containsRight = blocks.Where(r => r.RightColumns.Contains(index)).ToArray();

            foreach (var b in greaterLeft)
                b.LeftColumns = ShiftColumnsRight(b.LeftColumns, index);
            foreach (var b in greaterRight)
                b.RightColumns = ShiftColumnsRight(b.RightColumns, index);

            foreach (var b in containsLeft)
                b.LeftColumns.Add(index);
            foreach (var b in containsRight)
                b.RightColumns.Add(index);

            block.LeftColumns = new() { index };
            block.RightColumns = new() { index + 1 };
        }

        private void SetColumnsWidth(DiagramVM diagram, int index)
        {
            var currentWidth = diagram.ColumnsWidth[index];
            diagram.ColumnsWidth.Insert(index, currentWidth / 2);
            diagram.ColumnsWidth[index + 1] = currentWidth / 2;

            diagram.ColumnsWidth = columnsViewCalculator.RecalculateColumnsWidth(diagram.ColumnsWidth);
        }

        private List<int> ShiftColumnsRight(List<int> columns, int index) => columns.Select(r => r >= index ? r + 1 : r).ToList();

    }
}
