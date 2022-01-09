using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSDGenerator.Server.Data;
using NSDGenerator.Shared.Diagram.Helpers;
using NSDGenerator.Shared.Diagram.JsonModels;
using NSDGenerator.Shared.Diagram.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace NSDGenerator.Server.Repo;

public class DbRepo : IDbRepo
{
    private readonly JsonSerializerOptions jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    private readonly ILogger<DbRepo> logger;
    private readonly NsdContext context;

    public DbRepo(ILogger<DbRepo> logger, NsdContext context)
    {
        this.logger = logger;
        this.context = context;
    }

    public async Task<DiagramJsonModel> GetDiagramAsync(Guid id, string userName)
    {
        var row = await context.Diagrams
            .Where(r => r.Id == id)
            .Where(r => r.UserName == userName || !r.IsPrivate)
            .SingleOrDefaultAsync();

        if (row == null)
            return null;


        var blocks = await GetBlockCollectionAsync(id, row.RootBlockId);

        return new DiagramJsonModel
        {
            Id = row.Id,
            Name = row.Name,
            IsPrivate = row.IsPrivate,
            Owner = row.UserName,
            BlockCollection = blocks,
        };
    }

    public async Task<IEnumerable<DiagramInfoModel>> GetDiagramInfosAsync(string userName)
    {
        return await context.Diagrams
            .Where(r => r.UserName == userName)
            .Select(r => new DiagramInfoModel(r.Id, r.Name, r.IsPrivate, r.Created, r.Modified))
            .ToListAsync();
    }


    public async Task SaveDiagramAsync(DiagramJsonModel diagram, string userName)
    {
        var diagramRow = await context.Diagrams.SingleOrDefaultAsync(r => r.Id == diagram.Id);

        if (diagramRow is null)
        {
            diagramRow = new Diagram { Id = diagram.Id, UserName = userName, Created = DateTime.Now };
            context.Diagrams.Add(diagramRow);
        }

        var blockRows = await context.Blocks.Where(r => r.DiagramId == diagram.Id).ToListAsync();
        var deletedBlockRows = new List<Block>();

        if (diagram.BlockCollection?.RootId is not null)
        {
            var blocks = diagram.BlockCollection.Blocks;
            foreach (var block in blocks)
            {
                var blockRow = blockRows.SingleOrDefault(r => r.Id == block.Id);
                if (blockRow is null)
                {
                    blockRow = new Block { Id = block.Id, DiagramId = diagram.Id };
                    context.Blocks.Add(blockRow);
                }
                UpdateBlock(blockRow, block);
            }

            deletedBlockRows = blockRows
                .Where(r => !blocks.Select(rr => rr.Id).Contains(r.Id))
                .ToList();
        }
        else
        {
            deletedBlockRows.AddRange(blockRows);
        }

        context.Blocks.RemoveRange(deletedBlockRows);

        diagramRow.Name = diagram.Name;
        diagramRow.RootBlockId = diagram.BlockCollection?.RootId;
        diagramRow.IsPrivate = diagram.IsPrivate;
        diagramRow.Modified = DateTime.Now;

        var result = await context.SaveChangesAsync();
    }

    #region Converters

    private async Task<BlockCollectionJsonModel> GetBlockCollectionAsync(Guid diagramId, Guid? rootId)
    {
        if (rootId is null)
            return null;

        var blocks = await context.Blocks
            .Where(r => r.DiagramId == diagramId)
            .ToArrayAsync();

        return BlocksToBlockCollectionJsonModel(blocks, rootId.Value);
    }

    private BlockCollectionJsonModel BlocksToBlockCollectionJsonModel(Block[] blocks, Guid rootId)
    {
        var text = blocks
            .Where(r => r.BlockType == EnumBlockType.Text)
            .Select(r => BlockToTextBlockJsonModel(r))
            .ToList();
        var branch = blocks
            .Where(r => r.BlockType == EnumBlockType.Branch)
            .Select(r => BlockToBranchBlockJsonModel(r))
            .ToList();

        return new BlockCollectionJsonModel
        {
            RootId = rootId,
            TextBlocks = text,
            BranchBlocks = branch,
        };
    }

    private TextBlockJsonModel BlockToTextBlockJsonModel(Block block)
    {
        var content = JsonSerializer.Deserialize<TextBlockJsonData>(block.JsonData, jsonOptions);
        return new TextBlockJsonModel(block.Id, content.Text, content.ChildId);
    }

    private BranchBlockJsonModel BlockToBranchBlockJsonModel(Block block)
    {
        var content = JsonSerializer.Deserialize<BranchBlockJsonData>(block.JsonData, jsonOptions);
        return new BranchBlockJsonModel(block.Id, content.Condition, content.LeftBranch, content.RightBranch, content.LeftResult, content.RightResult);
    }

    private void UpdateBlock(Block block, IBlockJsonModel model)
    {
        string jsonData = null;
        if (model is TextBlockJsonModel tb)
        {
            block.BlockType = EnumBlockType.Text;

            var content = new TextBlockJsonData(tb.Text, tb.ChildId);
            jsonData = JsonSerializer.Serialize(content, jsonOptions);
        }

        if (model is BranchBlockJsonModel bb)
        {
            block.BlockType = EnumBlockType.Branch;

            var content = new BranchBlockJsonData(bb.Condition, bb.LeftBranch, bb.RightBranch, bb.LeftResult, bb.RightResult);
            jsonData = JsonSerializer.Serialize(content, jsonOptions);
        }

        block.JsonData = jsonData;
    }

    #endregion
}
