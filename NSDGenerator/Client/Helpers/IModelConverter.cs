using NSDGenerator.Client.ViewModels;
using NSDGenerator.Shared.Diagram;
using System.Collections.Generic;

namespace NSDGenerator.Client.Helpers;

public interface IModelConverter
{
    DiagramVM JsonToDiagramViewModel(string json);
    DiagramVM DiagramDtoToDiagramViewModel(DiagramDTO diagramDto);

    string DiagramViewModelToJson(DiagramVM diagram);

    BlockCollectionDTO RootBlockToBlockCollectionDto(IBlockVM rootBlock);
    IBlockVM BlockCollectionDtoToRootBlock(BlockCollectionDTO blockCollectionDto);

    List<BranchBlockVM> RootBlockToChildrenBranchBlockViewModels(IBlockVM rootBlock);
}
