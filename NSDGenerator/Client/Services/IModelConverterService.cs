using NSDGenerator.Client.Models;
using NSDGenerator.Shared.Diagram;

namespace NSDGenerator.Client.Services;

internal interface IModelConverterService
{
    DiagramModel JsonToDiagramModel(string json);
    DiagramModel DiagramFullDtoToDiagramModel(DiagramFullDto diagramFullDto);

    string DiagramModelToJson(DiagramModel diagram);

    BlockCollectionDto RootBlockToBlockCollectionDto(IBlockModel rootBlock);
    IBlockModel BlockCollectionDtoToRootBlock(BlockCollectionDto blockCollectionDto);


}
