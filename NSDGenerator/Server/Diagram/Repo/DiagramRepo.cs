﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSDGenerator.Server.DbData;
using NSDGenerator.Server.Diagram.Helpers;
using NSDGenerator.Shared.Diagram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NSDGenerator.Server.Diagram.Repo;

public class DiagramRepo : IDiagramRepo
{
    private readonly ILogger<DiagramRepo> logger;
    private readonly NsdContext context;
    private readonly IDtoConverter diagramConverters;

    public DiagramRepo(ILogger<DiagramRepo> logger, NsdContext context, IDtoConverter diagramConverters)
    {
        this.logger = logger;
        this.context = context;
        this.diagramConverters = diagramConverters;
    }

    public async Task<DiagramDTO> GetDiagramAsync(Guid id, string userName)
    {
        var row = await context.Diagrams
            .Where(r => r.Id == id)
            .Where(r => r.UserName == userName || !r.IsPrivate)
            .SingleOrDefaultAsync();

        if (row == null)
            return null;

        var blocks = await GetBlockCollectionAsync(id, row.RootBlockId);
        var columnsWidth = GetColumnsWidthList(row.ColumnsWidth);

        return new DiagramDTO(row.Id, row.Name, row.IsPrivate, row.UserName, blocks, columnsWidth);
    }

    private static List<int> GetColumnsWidthList(string columnWidthsString)
    {
        var list = columnWidthsString.Split(";", StringSplitOptions.RemoveEmptyEntries);
        return list.Select(r => int.Parse(r)).ToList();
    }

    private static string GetColumnsWidthString(List<int> columnWidths)
    {
        return string.Join(";", columnWidths); ;
    }

    public async Task<IEnumerable<DiagramInfoDTO>> GetDiagramInfosAsync(string userName)
    {
        return await context.Diagrams
            .Where(r => r.UserName == userName)
            .OrderByDescending(r => r.Created)
            .Select(r => new DiagramInfoDTO(r.Id, r.Name, r.IsPrivate, r.Created, r.Modified))
            .ToListAsync();
    }

    public async Task<bool> CheckIfDiagramExistsAsync(Guid id)
    {
        return await context.Diagrams
            .AnyAsync(r => r.Id == id);
    }


    public async Task<bool> SaveDiagramAsync(DiagramDTO diagram, string userName)
    {
        var diagramRow = await context.Diagrams.SingleOrDefaultAsync(r => r.Id == diagram.Id);

        if (diagramRow is null)
        {
            diagramRow = new DbData.Diagram { Id = diagram.Id, UserName = userName, Created = DateTime.Now };
            context.Diagrams.Add(diagramRow);
        }
        else
        {
            if (diagramRow.UserName.ToLower() != userName.ToLower())
                return false;
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
        diagramRow.ColumnsWidth = GetColumnsWidthString(diagram.ColumnsWidth);

        var result = await context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteDiagramAsync(Guid id, string userName)
    {
        var diagramRow = await context.Diagrams
            .Where(r => r.Id == id)
            .Where(r => r.UserName == userName)
            .SingleOrDefaultAsync();

        if (diagramRow is null)
            return false;

        var blockRows = await context.Blocks
            .Where(r => r.DiagramId == id)
            .ToArrayAsync();

        context.Diagrams.Remove(diagramRow);

        if (blockRows.Any())
            context.Blocks.RemoveRange(blockRows);

        await context.SaveChangesAsync();

        return true;
    }

    private async Task<BlockCollectionDTO> GetBlockCollectionAsync(Guid diagramId, Guid? rootId)
    {
        if (rootId is null)
            return null;

        var blocks = await context.Blocks
            .Where(r => r.DiagramId == diagramId)
            .ToArrayAsync();

        return diagramConverters.BlocksToBlockCollectionDto(blocks, rootId.Value);
    }

    private void UpdateBlock(Block block, IBlockDTO dto)
    {
        string jsonData = null;
        if (dto is TextBlockDTO tb)
        {
            block.BlockType = EnumBlockType.Text;
            jsonData = diagramConverters.TextBlockDtoToJson(tb);
        }

        if (dto is BranchBlockDTO bb)
        {
            block.BlockType = EnumBlockType.Branch;
            jsonData = diagramConverters.BranchBlockDtoToJson(bb);
        }

        block.JsonData = jsonData;
    }
}
