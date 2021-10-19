using NSDGenerator.Shared.Diagram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NSDGenerator.Client.Helpers
{
    public class AppState
    {
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

        public void DeleteBlock(Guid id) => OnBlockDeleted?.Invoke(id);


        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
