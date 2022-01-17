namespace NSDGenerator.Shared.Diagram;

public record BranchBlockDto(Guid Id, string Condition, string LeftBranch, string RightBranch, Guid? LeftResult, Guid? RightResult, int Level = 0) : IBlockDto;
