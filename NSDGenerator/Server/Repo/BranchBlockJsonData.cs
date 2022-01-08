using System;

namespace NSDGenerator.Server.Repo;

public record BranchBlockJsonData(string Condition, string LeftBranch, string RightBranch, Guid? LeftResult, Guid? RightResult);
