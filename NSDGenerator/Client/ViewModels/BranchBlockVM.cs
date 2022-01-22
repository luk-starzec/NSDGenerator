using System.Collections.Generic;

namespace NSDGenerator.Client.ViewModels;

public partial class BranchBlockVM : IBlockVM
{
    public Guid Id { get; set; }
    public IBlockVM Parent { get; set; }
    public string Condition { get; set; }
    public string LeftBranch { get; set; } = "Yes";
    public string RightBranch { get; set; } = "No";

    public List<int> LeftColumns { get; set; } = new List<int>();
    public List<int> RightColumns { get; set; } = new List<int>();

    private IBlockVM leftResult;
    public IBlockVM LeftResult
    {
        get => leftResult;
        set
        {
            leftResult = value;
            if (leftResult is not null)
                leftResult.Parent = this;
        }
    }
    private IBlockVM rightResult;
    public IBlockVM RightResult
    {
        get => rightResult;
        set
        {
            rightResult = value;
            if (rightResult is not null)
                rightResult.Parent = this;
        }
    }

    public BranchBlockVM() => Id = Guid.NewGuid();
}
