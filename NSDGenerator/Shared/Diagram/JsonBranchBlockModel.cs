namespace NSDGenerator.Shared.Diagram;

public record JsonBranchBlockModel(Guid Id, string Condition, string LeftBranch, string RightBranch, Guid? LeftResult, Guid? RightResult) : IJsonBlockModel;
