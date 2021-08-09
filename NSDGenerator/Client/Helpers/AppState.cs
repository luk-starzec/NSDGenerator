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
        public Guid SelectedBlockId { get; private set; }

        public event Action OnChange;

        public void SetCurrentDiagram(DiagramModel diagram)
        {
            CurrentDiagram = diagram;
            NotifyStateChanged();
        }

        public void SetSelectedBlockId(Guid id)
        {
            SelectedBlockId = id;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
