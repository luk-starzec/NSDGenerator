using NSDGenerator.Client.ViewModels;
using NSDGenerator.Shared.Diagram;
using System.Collections.Generic;

namespace NSDGenerator.Client.Helpers;

public interface IModelConverter
{
    DiagramVM JsonToDiagramModel(string json);
    DiagramVM DiagramFullDtoToDiagramModel(DiagramDTO diagramFullDto);

    string DiagramModelToJson(DiagramVM diagram);

    BlockCollectionDTO RootBlockToBlockCollectionDto(IBlockVM rootBlock);
    IBlockVM BlockCollectionDtoToRootBlock(BlockCollectionDTO blockCollectionDto);

    List<BranchBlockVM> RootBlockToChildrenBranchBlockModels(IBlockVM rootBlock);
}
