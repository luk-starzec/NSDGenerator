using NSDGenerator.Client.ViewModels;
using System.Collections.Generic;

namespace NSDGenerator.Client.Helpers.Columns
{
    public class ColumnsBlockCalculator : IColumnsBlockCalculator
    {
        private readonly IModelConverter modelConverter;
        private readonly IColumnsViewCalculator columnsViewCalculator;

        public ColumnsBlockCalculator(IModelConverter modelConverter, IColumnsViewCalculator columnsViewCalculator)
        {
            this.modelConverter = modelConverter;
            this.columnsViewCalculator = columnsViewCalculator;
        }

        public void SetColumnsOnBlockAdded(IBlockVM addedBlock, DiagramVM diagram)
        {
            var type = GetAddedColumnsType(addedBlock, diagram.ColumnsWidth);

            var block = addedBlock as BranchBlockVM;

            switch (type)
            {
                case AddedColumnsTypeEnum.None:
                    return;
                case AddedColumnsTypeEnum.SingleBranch:
                    new AddBlockToSingleBranch().ApplyChanges(diagram, block);
                    break;
                case AddedColumnsTypeEnum.ChildBranch:
                    new AddBlockToChildBranch(modelConverter, columnsViewCalculator).ApplyChanges(diagram, block);
                    break;
                case AddedColumnsTypeEnum.RootBranch:
                    new AddBlockToRootBranch(columnsViewCalculator).ApplyChanges(diagram, block);
                    break;
                default:
                    break;
            }
        }

        public void SetColumnsOnBlockDeleted(IBlockVM block, DiagramVM diagram)
        {
            var parentColumns = ColumnsHelper.GetParentColumnIndexes(block);

            if (parentColumns is null)
                new DeleteBlockRootBranch().ApplyChanges(diagram);
            else
                new DeleteBlockChildBranch(modelConverter).ApplyChanges(parentColumns, diagram);
        }

        private AddedColumnsTypeEnum GetAddedColumnsType(IBlockVM addedBlock, List<int> columnsWidth)
        {
            if (addedBlock is not BranchBlockVM block)
                return AddedColumnsTypeEnum.None;

            if (columnsWidth.Count < 2)
                return AddedColumnsTypeEnum.SingleBranch;

            var parentColumn = ColumnsHelper.GetParentColumnIndex(block);

            if (parentColumn is not null)
                return AddedColumnsTypeEnum.ChildBranch;

            return AddedColumnsTypeEnum.RootBranch;
        }
    }
}
