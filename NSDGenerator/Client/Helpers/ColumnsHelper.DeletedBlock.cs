using NSDGenerator.Client.Models;
using System.Collections.Generic;
using System.Linq;

namespace NSDGenerator.Client.Helpers
{
    public partial class ColumnsHelper : IColumnsHelper
    {
        public void SetColumnsOnBlockDeleted(IBlockModel block, DiagramModel diagram)
        {
            (var parent, var isLeftChild) = GetParentBranchBlock(block);

            if (parent is null)
            {
                diagram.ColumnsWidth = new() { 100 };
                return;
            }

            var blocks = modelConverterService.RootBlockToChildrenBranchBlockModels(diagram.RootBlock);

            var parentColumns = isLeftChild ? parent.LeftColumns.ToList() : parent.RightColumns.ToList();
            var deleted = parentColumns.Where(r => r > parentColumns.Min()).ToList();

            if (!deleted.Any())
                return;

            SetIndexesOnDelete(deleted, blocks);
            SetWidthsOnDelete(parentColumns, diagram);
        }

        private void SetIndexesOnDelete(List<int> deleted, List<BranchBlockModel> blocks)
        {
            int maxIndex = deleted.Max();
            int count = deleted.Count();

            var greaterLeft = blocks.Where(r => r.LeftColumns.Where(rr => rr >= maxIndex).Any()).ToArray();
            var greaterRight = blocks.Where(r => r.RightColumns.Where(rr => rr >= maxIndex).Any()).ToArray();

            foreach (var i in deleted)
            {
                foreach (var b in blocks)
                {
                    if (b.LeftColumns.Contains(i))
                        b.LeftColumns.Remove(i);
                    if (b.RightColumns.Contains(i))
                        b.RightColumns.Remove(i);
                }
            }

            foreach (var b in greaterLeft)
                b.LeftColumns = ShiftColumnsLeft(b.LeftColumns, maxIndex, count);
            foreach (var b in greaterRight)
                b.RightColumns = ShiftColumnsLeft(b.RightColumns, maxIndex, count);
        }

        private void SetWidthsOnDelete(List<int> parentColumns, DiagramModel diagram)
        {
            var min = parentColumns.Min();
            var max = parentColumns.Max();
            var dec = parentColumns.Count() - 1;

            var width = 0;
            for (int i = min; i <= max; i++)
                width += diagram.ColumnsWidth[i];

            diagram.ColumnsWidth.RemoveRange(min, dec);
            diagram.ColumnsWidth[min] = width;
        }

        private List<int> ShiftColumnsLeft(List<int> columns, int index, int count) => columns.Select(r => r >= index ? r - count : r).ToList();

    }
}
