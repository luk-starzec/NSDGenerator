﻿using NSDGenerator.Server.DbData;
using NSDGenerator.Shared.Diagram;
using System;

namespace NSDGenerator.Server.Diagram.Helpers;

public interface IDtoConverters
{
    BlockCollectionDTO BlocksToBlockCollectionDto(Block[] blocks, Guid rootId);
    string TextBlockDtoToJson(TextBlockDTO dto);
    string BranchBlockDtoToJson(BranchBlockDTO dto);

}
