using NSDGenerator.Client.ViewModels;

namespace NSDGenerator.Client.Helpers.Columns
{
    public interface IColumnsBlockCalculator
    {
        void SetColumnsOnBlockAdded(IBlockVM addedBlock, DiagramVM diagram);
        void SetColumnsOnBlockDeleted(IBlockVM block, DiagramVM diagram);
    }
}