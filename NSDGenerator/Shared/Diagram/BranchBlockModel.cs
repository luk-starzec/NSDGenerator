using System.Text.Json;

namespace NSDGenerator.Shared.Diagram;

public class BranchBlockModel : IBlockModel
{

    public Guid Id { get; set; }
    public IBlockModel Parent { get; set; }
    public string Condition { get; set; }
    public string LeftBranch { get; set; } = "Yes";
    public string RightBranch { get; set; } = "No";

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

    public string ToJson(JsonSerializerOptions options)
        => JsonSerializer.Serialize(new JsonBranchBlockModel(Id, Parent?.Id, Condition, LeftBranch, RightBranch, LeftResult?.Id, RightResult?.Id), options);

    public BranchBlockModel()
    {
        Id = Guid.NewGuid();
    }

    private record JsonBranchBlockModel(Guid Id, Guid? ParentId, string Condition, string LeftBranch, string RightBranch, Guid? LeftResult, Guid? RightResult, string BlockType = nameof(TextBlockModel));
}
