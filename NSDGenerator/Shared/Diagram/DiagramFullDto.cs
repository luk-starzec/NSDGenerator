using System.Collections.Generic;

namespace NSDGenerator.Shared.Diagram;

public record DiagramFullDto(Guid Id, string Name, bool IsPrivate, string Owner, BlockCollectionDto BlockCollection, List<int> ColumnWidths);
