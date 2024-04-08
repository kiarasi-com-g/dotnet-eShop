﻿using Azure.AI.OpenAI;
using Microsoft.SemanticKernel.Memory;
using Pgvector;

namespace eShop.Catalog.API.Services;

public sealed class CatalogAI(IOptions<AIOptions> options, ISemanticTextMemory memory = null, OpenAIClient openAIClient = null) : ICatalogAI
{
    private const string MemoryCollection = "catalogMemory";
    private readonly string _aiEmbeddingModel = options.Value.OpenAI.EmbeddingName ?? "text-embedding-ada-002";

    /// <summary>Gets whether the AI system is enabled.</summary>
    public bool IsEnabled { get; } = openAIClient is not null;

    /// <summary>Saves the specified catalog item to memory.</summary>
    public ValueTask<string> SaveToMemoryAsync(CatalogItem item) =>
        IsEnabled ?
            new(memory.SaveInformationAsync(MemoryCollection, $"{item.Name} {item.Description}", item.Id.ToString()))
            : new(string.Empty);

    public IAsyncEnumerable<MemoryQueryResult> SearchMemoryAsync(string query, int pageSize) =>
        IsEnabled ? memory.SearchAsync(MemoryCollection, query, pageSize) : throw new InvalidOperationException("Search can't be performed when AI is disabled");

}
