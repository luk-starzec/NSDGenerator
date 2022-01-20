using NSDGenerator.Client.Components.Blocks.Branch;
using NSDGenerator.Client.Models;
using NSDGenerator.Client.Services;
using System.Linq;
using System.Reflection;

namespace NSDGenerator.Client.Helpers;

public class AppState
{
    private readonly IColumnsHelper columnsHelper;

    public AppState(IColumnsHelper columnsHelper)
    {
        this.columnsHelper = columnsHelper;
    }

    public DiagramModel CurrentDiagram { get; private set; }
    public Guid? SelectedBlockId { get; private set; }

    public event Action<Guid> OnBlockDeleted;

    public event Action OnChange;

    public void SetCurrentDiagram(DiagramModel diagram)
    {
        CurrentDiagram = diagram;
        NotifyStateChanged();
    }

    public void SelectBlock(Guid? id)
    {
        SelectedBlockId = id;
        NotifyStateChanged();
    }

    public void DeleteBlock(IBlockModel block)
    {
        columnsHelper.SetColumnsOnBlockDeleted(block, CurrentDiagram);

        OnBlockDeleted?.Invoke(block.Id);

        if (CurrentDiagram.RootBlock.Id == block.Id)
        {
            CurrentDiagram.RootBlock = null;
            NotifyStateChanged();
        }
    }

    public void AddBlock(IBlockModel block)
    {
        columnsHelper.SetColumnsOnBlockAdded(block, CurrentDiagram);
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
