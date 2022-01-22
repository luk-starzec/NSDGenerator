using NSDGenerator.Client.ViewModels;
using System.Collections.Generic;

namespace NSDGenerator.Client.Helpers.Columns
{
    public interface IColumnsViewCalculator
    {
        List<int> ChangeColumnsWidth(int index, int value, List<int> widths);
        List<int> RecalculateColumnsWidth(List<int> widths);
        List<int> MakeColumnsEqual(List<int> columns, List<int> widths);

        int GetMinColumnWidth();
        int GetMaxColumnWidth(List<int> widths);
        bool AddColumnAvailable(List<int> widths);

        string GetResultWrapperStyle(BranchBlockVM branchBlock, List<int> columnsWidth);
        string GetBranchWrapperStyle(BranchBlockVM branchBlock, List<int> columnsWidth);
        string GetButtonsWrapperStyle(BranchBlockVM branchBlock, List<int> columnsWidth);
    }
}
