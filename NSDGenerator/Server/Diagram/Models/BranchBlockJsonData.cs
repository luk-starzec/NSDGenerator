using System;
using System.Collections.Generic;

namespace NSDGenerator.Server.Diagram.Models;

public record BranchBlockJsonData(
    string Condition,
    string LeftBranch,
    string RightBranch,
    Guid? LeftResult,
    Guid? RightResult,
    List<int> LeftColumns,
    List<int> RightColumns
);
