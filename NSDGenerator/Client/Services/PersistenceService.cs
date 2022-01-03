using Microsoft.JSInterop;
using NSDGenerator.Client.Pages;
using NSDGenerator.Shared.Diagram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NSDGenerator.Client.Services
{
    public class PersistenceService : IPersistenceService
    {
        private readonly IJSRuntime js;
        private readonly JsonSerializerOptions jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public PersistenceService(IJSRuntime js)
        {
            this.js = js;
        }

        public async Task DownloadDiagram(DiagramModel diagram)
        {
            var fileName = GetFileName(diagram.Name);
            string json = GetFileContent(diagram);

            await js.InvokeVoidAsync("DownloadFile", $"{fileName}.json", "application/json;charset=utf-8", json);
        }

        private string GetFileName(string diagramName)
        {
            return string.IsNullOrWhiteSpace(diagramName)
                ? "unnamed-diagram"
                : diagramName
                    .Replace(" ", "_")
                    .ToLower();
        }

        private string GetFileContent(DiagramModel diagram)
        {
            var blocks = new List<string>();
            SerializeBlocks(diagram.RootBlock, blocks);

            var json = $"[{string.Join(",", blocks)}]";

            return json;
        }

        private void SerializeBlocks(IBlockModel block, List<string> result)
        {
            if (block is null)
                return;

            var tb = block as TextBlockModel;
            if (tb is not null)
            {
                var json = tb.ToJson(jsonOptions);
                result.Add(json);
                SerializeBlocks(tb.Child, result);
                return;
            }

            var bb = block as BranchBlockModel;
            if (bb is not null)
            {
                var json = bb.ToJson(jsonOptions);
                result.Add(json);
                SerializeBlocks(bb.LeftResult, result);
                SerializeBlocks(bb.RightResult, result);
                return;
            }
        }
    }
}
