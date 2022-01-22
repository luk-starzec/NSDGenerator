using NSDGenerator.Client.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace NSDGenerator.Client.Helpers.Columns
{
    public class ColumnsHelper
    {
        public static int? GetParentColumnIndex(IBlockVM block)
        {
            (var parent, var isLeftChild) = GetParentBranchBlock(block);

            if (parent is null)
                return null;

            return isLeftChild ? parent.LeftColumns[0] : parent.RightColumns[0];
        }

        public static List<int> GetParentColumnIndexes(IBlockVM block)
        {
            (var parent, var isLeftChild) = GetParentBranchBlock(block);

            if (parent is null)
                return null;

            return isLeftChild ? parent.LeftColumns.ToList() : parent.RightColumns.ToList();
        }

        private static (BranchBlockVM parent, bool isLeftChild) GetParentBranchBlock(IBlockVM block)
        {
            if (block.Parent is BranchBlockVM bb)
                return (bb, bb.LeftResult?.Id == block.Id);

            if (block.Parent is null)
                return (null, false);

            return GetParentBranchBlock(block.Parent);
        }

    }
}
