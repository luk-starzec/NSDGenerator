namespace NSDGenerator.Shared.Diagram.JsonModels;

public record TextBlockJsonModel(Guid Id, string Text, Guid? ChildId) : IBlockJsonModel;
