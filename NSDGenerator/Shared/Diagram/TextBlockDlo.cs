namespace NSDGenerator.Shared.Diagram;

public record TextBlockDto(Guid Id, string Text, Guid? ChildId) : IBlockDto;
