using Microsoft.JSInterop;
using NSDGenerator.Shared.Diagram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NSDGenerator.Client.Services;

public class PersistenceService : IPersistenceService
{
    private readonly string clientName = "NSDGenerator.Server";

    private readonly IJSRuntime js;
    private readonly ISerializeService serializeService;
    private readonly IHttpClientFactory httpClientFactory;


    public PersistenceService(IJSRuntime js, ISerializeService serializeService, IHttpClientFactory httpClientFactory)
    {
        this.js = js;
        this.serializeService = serializeService;
        this.httpClientFactory = httpClientFactory;
    }

    public async Task DownloadDiagramAsync(DiagramModel diagram)
    {
        var name = GetFileName(diagram.Name);
        string content = serializeService.SerializeDiagram(diagram);

        await js.InvokeVoidAsync("DownloadFile", $"{name}.json", "application/json;charset=utf-8", content);
    }

    public async Task<DiagramModel> GetDiagramAsync(Guid id)
    {
        var client = httpClientFactory.CreateClient(clientName);
        var json = await client.GetFromJsonAsync<JsonDiagram>($"api/diagram/{id}");
        var diagram = serializeService.DeserializeDiagram(json);
        return diagram;
    }

    public async Task<IEnumerable<DiagramInfoModel>> GetDiagramsAsync()
    {
        var client = httpClientFactory.CreateClient(clientName);
        var diagrams = await client.GetFromJsonAsync<IEnumerable<DiagramInfoModel>>("api/diagram");
        return diagrams;
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
