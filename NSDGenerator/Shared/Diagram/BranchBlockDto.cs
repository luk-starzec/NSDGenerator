using System.Collections.Generic;

namespace NSDGenerator.Shared.Diagram;

public record BranchBlockDTO(
    Guid Id,
    string Condition,
    string LeftBranch,
    string RightBranch,
    Guid? LeftResult,
    Guid? RightResult,
    List<int> LeftColumns,
    List<int> RightColumns
) : IBlockDTO;
