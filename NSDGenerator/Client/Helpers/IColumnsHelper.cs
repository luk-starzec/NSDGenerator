using NSDGenerator.Client.Models;
using System.Collections.Generic;

namespace NSDGenerator.Client.Helpers
{
    public interface IColumnsHelper
    {
        void SetColumnsOnBlockAdded(IBlockModel block, DiagramModel diagram);
        void SetColumnsOnBlockDeleted(IBlockModel block, DiagramModel diagram);

        string GetResultWrapperStyle(BranchBlockModel branchBlock, List<int> columnsWidth);
        string GetBranchWrapperStyle(BranchBlockModel branchBlock, List<int> columnsWidth);
        string GetButtonsWrapperStyle(BranchBlockModel branchBlock, List<int> columnsWidth);
    }
}
