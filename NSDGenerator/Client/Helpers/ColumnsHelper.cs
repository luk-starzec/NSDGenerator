using NSDGenerator.Client.Models;
using NSDGenerator.Client.Services;
using System.Collections.Generic;
using System.Linq;

namespace NSDGenerator.Client.Helpers
{
    public partial class ColumnsHelper : IColumnsHelper
    {
        private readonly IModelConverterService modelConverterService;

        public ColumnsHelper(IModelConverterService modelConverterService)
        {
            this.modelConverterService = modelConverterService;
        }


        public string GetResultWrapperStyle(BranchBlockModel branchBlock, List<int> columnsWidth)
        {
            var (first, second) = GetColumnsRatio(branchBlock, columnsWidth);

            return first is not null && second is not null
                ? $"grid-template-columns: {first}% {second}%"
                : "";
        }

        public string GetBranchWrapperStyle(BranchBlockModel branchBlock, List<int> columnsWidth)
        {
            var (first, second) = GetColumnsRatio(branchBlock, columnsWidth);

            return first is not null && second is not null
                ? $"background: linear-gradient(to top right,transparent 50%,var(--color-70) 0) 0 0/calc({first}% + 1px) 100% no-repeat,linear-gradient(to top left,transparent 50%,var(--color-70) 0) 100% 0/calc({second}% + 1px) 100% no-repeat"
                : "";
        }

        public string GetButtonsWrapperStyle(BranchBlockModel branchBlock, List<int> columnsWidth)
        {
            var (one, two) = GetColumnsRatio(branchBlock, columnsWidth);

            if (one is null || two is null)
                return "";

            var first = one / 2;
            var second = one / 2;
            var third = two / 2;
            var fourth = 100 - first - second - third;

            return $"grid-template-columns: {first}% {second}% {third}% {fourth}%";
        }

        private (int?, int?) GetColumnsRatio(BranchBlockModel branchBlock, List<int> columnsWidth)
        {
            if (branchBlock.LeftColumns?.Any() != true)
                return (null, null);

            int left = GetTotalWidth(branchBlock.LeftColumns, columnsWidth);
            int right = GetTotalWidth(branchBlock.RightColumns, columnsWidth);

            float width = left + right;
            var percent = (int)(left / width * 100);

            return (percent, 100 - percent);
        }

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

        private (BranchBlockModel parent, bool isLeftChild) GetParentBranchBlock(IBlockModel block)
        {
            if (block.Parent is BranchBlockModel bb)
                return (bb, bb.LeftResult?.Id == block.Id);

            if (block.Parent is null)
                return (null, false);

            return GetParentBranchBlock(block.Parent);
        }

    }
}
