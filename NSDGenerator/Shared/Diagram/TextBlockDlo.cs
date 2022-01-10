namespace NSDGenerator.Shared.Diagram;

public record TextBlockDlo(Guid Id, string Text, Guid? ChildId) : IBlockDto;
