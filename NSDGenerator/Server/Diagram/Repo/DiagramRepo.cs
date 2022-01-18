using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSDGenerator.Server.DbData;
using NSDGenerator.Server.Diagram.Helpers;
using NSDGenerator.Shared.Diagram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NSDGenerator.Server.Diagram.Repo
{
    public class DiagramRepo : IDiagramRepo
    {
        private readonly ILogger<DiagramRepo> logger;
        private readonly NsdContext context;
        private readonly IDtoConverters diagramConverters;

        public DiagramRepo(ILogger<DiagramRepo> logger, NsdContext context, IDtoConverters diagramConverters)
        {
            this.logger = logger;
            this.context = context;
            this.diagramConverters = diagramConverters;
        }

        public async Task<DiagramFullDto> GetDiagramAsync(Guid id, string userName)
        {
            var row = await context.Diagrams
                .Where(r => r.Id == id)
                .Where(r => r.UserName == userName || !r.IsPrivate)
                .SingleOrDefaultAsync();

            if (row == null)
                return null;

            var blocks = await GetBlockCollectionAsync(id, row.RootBlockId);
            var columnWidths = GetColumnWidthList(row.ColumnWidths);

            return new DiagramFullDto(row.Id, row.Name, row.IsPrivate, row.UserName, blocks, columnWidths);
        }

        private static List<int> GetColumnWidthList(string columnWidthsString)
        {
            // temp fallback
            if (string.IsNullOrEmpty(columnWidthsString))
                return new List<int>();

            var list = columnWidthsString.Split(";");
            return list.Select(r => int.Parse(r)).ToList();
        }

        private static string GetColumnWidthString(List<int> columnWidths)
        {
            return string.Join(";", columnWidths); ;
        }

        public async Task<IEnumerable<DiagramDto>> GetDiagramInfosAsync(string userName)
        {
            return await context.Diagrams
                .Where(r => r.UserName == userName)
                .OrderByDescending(r => r.Created)
                .Select(r => new DiagramDto(r.Id, r.Name, r.IsPrivate, r.Created, r.Modified))
                .ToListAsync();
        }

        public async Task<bool> CheckIfDiagramExistsAsync(Guid id)
        {
            return await context.Diagrams
                .AnyAsync(r => r.Id == id);
        }


        public async Task<bool> SaveDiagramAsync(DiagramFullDto diagram, string userName)
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
            diagramRow.ColumnWidths = GetColumnWidthString(diagram.ColumnWidths);

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

        private async Task<BlockCollectionDto> GetBlockCollectionAsync(Guid diagramId, Guid? rootId)
        {
            if (rootId is null)
                return null;

            var blocks = await context.Blocks
                .Where(r => r.DiagramId == diagramId)
                .ToArrayAsync();

            return diagramConverters.BlocksToBlockCollectionDto(blocks, rootId.Value);
        }

        private void UpdateBlock(Block block, IBlockDto dto)
        {
            string jsonData = null;
            if (dto is TextBlockDto tb)
            {
                block.BlockType = EnumBlockType.Text;
                jsonData = diagramConverters.TextBlockDtoToJson(tb);
            }

            if (dto is BranchBlockDto bb)
            {
                block.BlockType = EnumBlockType.Branch;
                jsonData = diagramConverters.BranchBlockDtoToJson(bb);
            }

            block.JsonData = jsonData;
        }
    }
}
