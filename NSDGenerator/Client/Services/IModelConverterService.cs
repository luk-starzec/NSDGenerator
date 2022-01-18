using NSDGenerator.Client.Models;
using NSDGenerator.Shared.Diagram;
using System.Collections.Generic;

namespace NSDGenerator.Client.Services;

internal interface IModelConverterService
{
    DiagramModel JsonToDiagramModel(string json);
    DiagramModel DiagramFullDtoToDiagramModel(DiagramFullDto diagramFullDto);

    string DiagramModelToJson(DiagramModel diagram);

    BlockCollectionDto RootBlockToBlockCollectionDto(IBlockModel rootBlock);
    IBlockModel BlockCollectionDtoToRootBlock(BlockCollectionDto blockCollectionDto);

    List<BranchBlockModel> RootBlockToChildrenBranchBlockModels(IBlockModel rootBlock);
}
