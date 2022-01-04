using Microsoft.JSInterop;
using NSDGenerator.Client.Pages;
using NSDGenerator.Shared.Diagram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NSDGenerator.Client.Services;

public class PersistenceService : IPersistenceService
{
    private readonly IJSRuntime js;
    private readonly ISerializeService serializeService;

    public PersistenceService(IJSRuntime js, ISerializeService serializeService)
    {
        this.js = js;
        this.serializeService = serializeService;
    }

    public async Task DownloadDiagram(DiagramModel diagram)
    {
        var name = GetFileName(diagram.Name);
        string content = serializeService.SerializeDiagram(diagram);

        await js.InvokeVoidAsync("DownloadFile", $"{name}.json", "application/json;charset=utf-8", content);
    }

    private string GetFileName(string diagramName)
    {
        return string.IsNullOrWhiteSpace(diagramName)
            ? "unnamed-diagram"
            : diagramName
                .Replace(" ", "_")
                .ToLower();
    }
}
