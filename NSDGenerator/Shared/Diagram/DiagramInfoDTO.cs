namespace NSDGenerator.Shared.Diagram;

public record DiagramInfoDTO(
    Guid Id,
    string Name,
    bool IsPrivate,
    DateTime Created,
    DateTime Modified
);

