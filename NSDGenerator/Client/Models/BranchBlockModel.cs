using System.Collections.Generic;

namespace NSDGenerator.Client.Models;

public partial class BranchBlockModel : IBlockModel
{
    public Guid Id { get; set; }
    public IBlockModel Parent { get; set; }
    public string Condition { get; set; }
    public string LeftBranch { get; set; } = "Yes";
    public string RightBranch { get; set; } = "No";

    public List<int> LeftColumns { get; set; } = new List<int>();
    public List<int> RightColumns { get; set; } = new List<int>();

    private IBlockModel leftResult;
    public IBlockModel LeftResult
    {
        get => leftResult;
        set
        {
            leftResult = value;
            if (leftResult is not null)
                leftResult.Parent = this;
        }
    }
    private IBlockModel rightResult;
    public IBlockModel RightResult
    {
        get => rightResult;
        set
        {
            rightResult = value;
            if (rightResult is not null)
                rightResult.Parent = this;
        }
    }

    public BranchBlockModel() => Id = Guid.NewGuid();
}
