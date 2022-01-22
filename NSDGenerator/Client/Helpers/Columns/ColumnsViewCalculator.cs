using NSDGenerator.Client.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace NSDGenerator.Client.Helpers.Columns
{
    public class ColumnsViewCalculator : IColumnsViewCalculator
    {
        private readonly int minColumnWidth = 10;

        private readonly IModelConverter modelConverter;

        public ColumnsViewCalculator(IModelConverter modelConverter)
        {
            this.modelConverter = modelConverter;
        }


        public string GetResultWrapperStyle(BranchBlockVM branchBlock, List<int> columnsWidth)
        {
            var percent = GetBranchColumnsRatio(branchBlock, columnsWidth);

            if (percent is null)
                return "";

            return $"grid-template-columns: {percent}% {100 - percent}%";
        }

        public string GetBranchWrapperStyle(BranchBlockVM branchBlock, List<int> columnsWidth)
        {
            var percent = GetBranchColumnsRatio(branchBlock, columnsWidth);

            if (percent is null)
                return "";

            return $"background: linear-gradient(to top right,transparent 50%,var(--color-70) 0) 0 0/calc({percent}% + 1px) 100% no-repeat,linear-gradient(to top left,transparent 50%,var(--color-70) 0) 100% 0/calc({100 - percent}% + 1px) 100% no-repeat";
        }

        public string GetButtonsWrapperStyle(BranchBlockVM branchBlock, List<int> columnsWidth)
        {
            var percent = GetBranchColumnsRatio(branchBlock, columnsWidth);

            if (percent is null)
                return "";

            var first = percent / 2;
            var second = percent / 2;
            var third = (100 - percent) / 2;
            var fourth = 100 - first - second - third;

            return $"grid-template-columns: {first}% {second}% {third}% {fourth}%";
        }

        public int GetMinColumnWidth() => minColumnWidth;
        public int GetMaxColumnWidth(List<int> widths) => 100 - minColumnWidth * (widths.Count - 1);
        public bool AddColumnAvailable(List<int> widths) => widths.Sum() / (widths.Count + 1) > minColumnWidth;

        public List<int> MakeColumnsEqual(List<int> columns, List<int> widths)
        {
            var total = GetTotalWidth(columns, widths);
            var width = total / columns.Count;

            foreach (var i in columns)
                widths[i] = width;

            RecalculateColumnsWidth(widths);

            return widths;
        }

        public List<int> ChangeColumnsWidth(int index, int value, List<int> widths)
        {
            var diff = value - widths[index];

            widths[index] += diff;
            SetDependentColumnWidth(index, diff, widths);

            return widths;
        }

        public List<int> RecalculateColumnsWidth(List<int> widths)
        {
            for (int i = 0; i < widths.Count; i++)
            {
                if (widths[i] < minColumnWidth)
                    ChangeColumnsWidth(i, minColumnWidth, widths);
            }

            var total = widths.Sum();
            if (total < 100)
                widths[0] += 100 - total;

            return widths;
        }

        private int? GetBranchColumnsRatio(BranchBlockVM branchBlock, List<int> columnsWidth)
        {
            if (branchBlock.LeftColumns?.Any() != true)
                return null;

            int left = GetTotalWidth(branchBlock.LeftColumns, columnsWidth);
            int right = GetTotalWidth(branchBlock.RightColumns, columnsWidth);

            float width = left + right;
            var percent = (int)(left / width * 100);

            return percent;
        }

        private void SetDependentColumnWidth(int index, int diff, List<int> widths)
        {
            int dependentIndex = GetDependentColumnIndex(index, widths);

            if (widths[dependentIndex] - diff >= minColumnWidth)
            {
                widths[dependentIndex] -= diff;
                return;
            }

            diff = minColumnWidth - widths[dependentIndex] + diff;
            widths[dependentIndex] = minColumnWidth;
            SetDependentColumnWidth(dependentIndex, diff, widths);
        }

        private int GetDependentColumnIndex(int index, List<int> widths) => index < widths.Count - 1 ? index + 1 : 0;

        private int GetTotalWidth(List<int> indexes, List<int> widths)
        {
            try
            {
                int width = 0;
                foreach (var i in indexes)
                    width += widths[i];
                return width;
            }
            catch
            {
                // possible exception in transient state when deleting blocks
                return 0;
            }
        }
    }
}
