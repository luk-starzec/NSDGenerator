using System.Collections.Generic;

namespace NSDGenerator.Shared.Diagram;

public record DiagramDTO(
    Guid Id,
    string Name,
    bool IsPrivate,
    string Owner,
    BlockCollectionDTO BlockCollection,
    List<int> ColumnsWidth
);
