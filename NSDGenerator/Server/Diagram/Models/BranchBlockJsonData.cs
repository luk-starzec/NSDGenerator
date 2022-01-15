using System;

namespace NSDGenerator.Server.Diagram.Models;

public record BranchBlockJsonData(string Condition, string LeftBranch, string RightBranch, Guid? LeftResult, Guid? RightResult);
