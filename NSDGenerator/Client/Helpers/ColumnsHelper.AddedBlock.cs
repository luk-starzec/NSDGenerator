using NSDGenerator.Client.Models;
using System.Collections.Generic;
using System.Linq;

namespace NSDGenerator.Client.Helpers
{
    public partial class ColumnsHelper : IColumnsHelper
    {
        public void SetColumnsOnBlockAdded(IBlockModel addedBlock, DiagramModel diagram)
        {
            if (addedBlock is not BranchBlockModel block)
                return;

            if (diagram.ColumnsWidth.Count < 2)
            {
                AddedToOnlyBranch(diagram, block);
                return;
            }

            (var parent, var isLeftChild) = GetParentBranchBlock(block);

            if (parent is not null)
            {
                var index = isLeftChild ? parent.LeftColumns[0] : parent.RightColumns[0];
                AddedToChildBranch(diagram, block, index);
                return;
            }

            AddedToRootBranch(diagram, block);
        }

        private void AddedToRootBranch(DiagramModel diagram, BranchBlockModel block)
        {
            SetIndexesOnAddedToRootBranch(block, diagram);
            SetWidthsOnAddedToRootBranch(diagram);
        }

        private void AddedToChildBranch(DiagramModel diagram, BranchBlockModel block, int index)
        {
            SetIndexesOnAddedToChildBranch(block, index, diagram.RootBlock);
            SetWidthsOnAddedToChildBranch(diagram, index);
        }

        private void AddedToOnlyBranch(DiagramModel diagram, BranchBlockModel block)
        {
            SetIndexesOnAddedToOnlyBranch(block);
            SetWidthsOnAddedToOnlyBranch(diagram);
        }

        private void SetIndexesOnAddedToOnlyBranch(BranchBlockModel block)
        {
            block.LeftColumns = new() { 0 };
            block.RightColumns = new() { 1 };
        }

        private void SetWidthsOnAddedToOnlyBranch(DiagramModel diagram)
        {
            diagram.ColumnsWidth = new() { 50, 50 };
        }

        private void SetIndexesOnAddedToChildBranch(BranchBlockModel block, int index, IBlockModel diagramRootBlock)
        {
            var blocks = modelConverterService.RootBlockToChildrenBranchBlockModels(diagramRootBlock);

            var greaterLeft = blocks.Where(r => r.LeftColumns.Where(rr => rr >= index).Any()).ToArray();
            var greaterRight = blocks.Where(r => r.RightColumns.Where(rr => rr >= index).Any()).ToArray();

            var containsLeft = blocks.Where(r => r.LeftColumns.Contains(index)).ToArray();
            var containsRight = blocks.Where(r => r.RightColumns.Contains(index)).ToArray();

            foreach (var b in greaterLeft)
                b.LeftColumns = ShiftColumnsRight(b.LeftColumns, index);
            foreach (var b in greaterRight)
                b.RightColumns = ShiftColumnsRight(b.RightColumns, index);

            foreach (var b in containsLeft)
                b.LeftColumns.Add(index);
            foreach (var b in containsRight)
                b.RightColumns.Add(index);

            block.LeftColumns = new() { index };
            block.RightColumns = new() { index + 1 };
        }

        private void SetWidthsOnAddedToChildBranch(DiagramModel diagram, int index)
        {
            var currentWidth = diagram.ColumnsWidth[index];
            diagram.ColumnsWidth.Insert(index, currentWidth / 2);
            diagram.ColumnsWidth[index + 1] = currentWidth / 2;
        }

        private void SetIndexesOnAddedToRootBranch(BranchBlockModel block, DiagramModel diagram)
        {
            for (int i = 0; i < diagram.ColumnsWidth.Count; i++)
                block.LeftColumns.Add(i);

            block.RightColumns = new() { diagram.ColumnsWidth.Count };
        }

        private void SetWidthsOnAddedToRootBranch(DiagramModel diagram)
        {
            for (int i = 0; i < diagram.ColumnsWidth.Count; i++)
                diagram.ColumnsWidth[i] = diagram.ColumnsWidth[i] / 2;

            diagram.ColumnsWidth.Add(50);
        }
        private List<int> ShiftColumnsRight(List<int> columns, int index) => columns.Select(r => r >= index ? r + 1 : r).ToList();

    }
}
