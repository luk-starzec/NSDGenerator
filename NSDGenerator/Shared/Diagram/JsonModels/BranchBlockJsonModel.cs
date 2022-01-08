namespace NSDGenerator.Shared.Diagram.JsonModels;

public record BranchBlockJsonModel(Guid Id, string Condition, string LeftBranch, string RightBranch, Guid? LeftResult, Guid? RightResult) : IBlockJsonModel;
