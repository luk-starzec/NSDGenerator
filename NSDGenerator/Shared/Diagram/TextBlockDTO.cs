namespace NSDGenerator.Shared.Diagram;

public record TextBlockDTO(
    Guid Id,
    string Text,
    Guid? ChildId,
    int Level = 0
) : IBlockDTO;
