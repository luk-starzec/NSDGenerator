using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSDGenerator.Server.Data;
using NSDGenerator.Shared.Diagram;
using NSDGenerator.Shared.Login;
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
    private readonly DiagramConverters diagramConverters;

    public DbRepo(ILogger<DbRepo> logger, NsdContext context)
    {
        this.logger = logger;
        this.context = context;
        diagramConverters = new DiagramConverters(jsonOptions);
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

        return new DiagramFullDto
        {
            Id = row.Id,
            Name = row.Name,
            IsPrivate = row.IsPrivate,
            Owner = row.UserName,
            BlockCollection = blocks,
        };
    }

    public async Task<IEnumerable<DiagramDto>> GetDiagramInfosAsync(string userName)
    {
        return await context.Diagrams
            .Where(r => r.UserName == userName)
            .Select(r => new DiagramDto(r.Id, r.Name, r.IsPrivate, r.Created, r.Modified))
            .ToListAsync();
    }


    public async Task SaveDiagramAsync(DiagramFullDto diagram, string userName)
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


    public async Task<string> RegisterUserAsync(RegisterDto register)
    {
        var code = await context.RegistrationCodes
            .Where(r => r.Code == register.RegistrationCode)
            .Where(r => r.IsActive)
            .Where(r => !r.ValidTo.HasValue || r.ValidTo >= DateTime.Now)
            .SingleOrDefaultAsync();

        if (code is null)
            return "Invalid registration code";

        var existingUser = await context.Users.AnyAsync(r => r.Name == register.Email);

        if (existingUser)
            return $"User {register.Email} already exists";

        var user = new User
        {
            Name = register.Email,
            Password = GeneratePasswordHash(register.Password),
            IsEnabled = true,
            Created = DateTime.Now,
        };
        context.Users.Add(user);

        code.IsActive = false;

        await context.SaveChangesAsync();

        return null;
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

            var content = new TextBlockJsonData(tb.Text, tb.ChildId);
            jsonData = JsonSerializer.Serialize(content, jsonOptions);
        }

        if (dto is BranchBlockDto bb)
        {
            block.BlockType = EnumBlockType.Branch;

            var content = new BranchBlockJsonData(bb.Condition, bb.LeftBranch, bb.RightBranch, bb.LeftResult, bb.RightResult);
            jsonData = JsonSerializer.Serialize(content, jsonOptions);
        }

        block.JsonData = jsonData;
    }

    private string GeneratePasswordHash(string password)
    {
        // TODO
        return password;
    }
}
