using NSDGenerator.Client.Helpers.Columns;
using NSDGenerator.Client.ViewModels;

namespace NSDGenerator.Client.Helpers;

public class AppState
{
    private readonly IColumnsBlockCalculator columnsBlockCalculator;

    public AppState(IColumnsBlockCalculator columnsBlockCalculator)
    {
        this.columnsBlockCalculator = columnsBlockCalculator;
    }

    public DiagramVM CurrentDiagram { get; private set; }
    public Guid? SelectedBlockId { get; private set; }

    public event Action<Guid> OnBlockAdded;

    public event Action<Guid> OnBlockDeleted;

    public event Action OnChange;

    public void SetCurrentDiagram(DiagramVM diagram)
    {
        CurrentDiagram = diagram;
        NotifyStateChanged();
    }

    public void SelectBlock(Guid? id)
    {
        SelectedBlockId = id;
        NotifyStateChanged();
    }

    public void DeleteBlock(IBlockVM block)
    {
        columnsBlockCalculator.SetColumnsOnBlockDeleted(block, CurrentDiagram);

        OnBlockDeleted?.Invoke(block.Id);

        if (CurrentDiagram.RootBlock.Id == block.Id)
        {
            CurrentDiagram.RootBlock = null;
            NotifyStateChanged();
        }
    }

    public void AddBlock(IBlockVM block)
    {
        columnsBlockCalculator.SetColumnsOnBlockAdded(block, CurrentDiagram);

        OnBlockAdded?.Invoke(block.Id);
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
