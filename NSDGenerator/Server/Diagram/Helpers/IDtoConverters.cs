using NSDGenerator.Server.DbData;
using NSDGenerator.Shared.Diagram;
using System;

namespace NSDGenerator.Server.Diagram.Helpers;

public interface IDtoConverters
{
    BlockCollectionDto BlocksToBlockCollectionDto(Block[] blocks, Guid rootId);
    string TextBlockDtoToJson(TextBlockDto dto);
    string BranchBlockDtoToJson(BranchBlockDto dto);

}
