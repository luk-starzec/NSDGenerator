﻿using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using NSDGenerator.Shared.Diagram.Helpers;
using NSDGenerator.Shared.Diagram.JsonModels;
using NSDGenerator.Shared.Diagram.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace NSDGenerator.Client.Services;

public class DiagramService : IDiagramService
{
    private readonly ILogger<DiagramService> logger;
    private readonly HttpClient httpClient;
    private readonly IJSRuntime js;
    private readonly ISerializationHelper serializationHelper;

    public DiagramService(ILogger<DiagramService> logger, HttpClient httpClient, IJSRuntime js, ISerializationHelper serializationHelper)
    {
        this.logger = logger;
        this.httpClient = httpClient;
        this.js = js;
        this.serializationHelper = serializationHelper;
    }

    public async Task DownloadDiagramAsync(DiagramModel diagram)
    {
        var name = GetFileName(diagram.Name);
        string content = serializationHelper.SerializeDiagram(diagram);

        await js.InvokeVoidAsync("DownloadFile", $"{name}.json", "application/json;charset=utf-8", content);
    }

    public async Task<IEnumerable<DiagramInfoModel>> GetMyDiagramsAsync()
    {
        try
        {
            var diagrams = await httpClient.GetFromJsonAsync<IEnumerable<DiagramInfoModel>>("api/diagram");
            return diagrams;
        }
        catch (Exception ex)
        {
            logger.LogError("Method {Method} thrown exception: {Message}", nameof(GetMyDiagramsAsync), ex.Message);
            return null;
        }
    }

    public async Task<DiagramModel> GetDiagramAsync(Guid id)
    {
        try
        {
            var json = await httpClient.GetFromJsonAsync<DiagramJsonModel>($"api/diagram/{id}");
            var diagram = serializationHelper.DeserializeDiagram(json);
            return diagram;
        }
        catch (Exception ex)
        {
            logger.LogError("Method {Method} thrown exception: {Message}", nameof(GetDiagramAsync), ex.Message);
            return null;
        }
    }

    public async Task<bool> SaveDiagramAsync(DiagramModel diagram)
    {
        try
        {
            var json = serializationHelper.SerializeDiagram(diagram);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"api/diagram", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            logger.LogError("Method {Method} thrown exception: {Message}", nameof(SaveDiagramAsync), ex.Message);
            return false;
        }
    }

    public DiagramModel GetDiagram(string fileContent)
    {
        return serializationHelper.DeserializeDiagram(fileContent);
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