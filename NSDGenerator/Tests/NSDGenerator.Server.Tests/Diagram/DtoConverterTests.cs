using NSDGenerator.Server.DbData;
using NSDGenerator.Server.Diagram.Helpers;
using NSDGenerator.Shared.Diagram;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NSDGenerator.Server.Tests.Diagram;

public class DtoConverterTests
{
    private readonly IDtoConverter sut = new DtoConverter();

    [Theory]
    [MemberData(nameof(TextBlockDtos))]
    public void TextBlockDtoToJson_ReturnsValidJson(TextBlockDTO dto, string expected)
    {
        var actual = sut.TextBlockDtoToJson(dto);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(BranchBlockDtos))]
    public void BranchBlockDtoToJson_ReturnsValidJson(BranchBlockDTO dto, string expected)
    {
        var actual = sut.BranchBlockDtoToJson(dto);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void BlocksToBlockCollectionDto_SetsRootId()
    {
        var rootId = Guid.NewGuid();

        var actual = sut.BlocksToBlockCollectionDto(new Block[0], rootId);

        Assert.Equal(rootId, actual.RootId);
    }

    [Fact]
    public void BlocksToBlockCollectionDto_SetsTextBlocks()
    {
        var id = Guid.NewGuid();
        var text = "test";
        var childId = Guid.NewGuid();
        var json = $"{{\"text\":\"{text}\",\"childId\":\"{childId}\"}}";

        var blocks = new Block[]
        {
                new Block{BlockType = EnumBlockType.Text, Id = id, JsonData = json},
        };

        var actual = sut.BlocksToBlockCollectionDto(blocks, Guid.NewGuid());

        Assert.NotNull(actual.TextBlocks);
        Assert.Equal(id, actual.TextBlocks[0].Id);
        Assert.Equal(text, actual.TextBlocks[0].Text);
        Assert.Equal(childId, actual.TextBlocks[0].ChildId);
    }

    [Fact]
    public void BlocksToBlockCollectionDto_SetsBranchBlocks()
    {
        var id = Guid.NewGuid();
        var condition = "test";
        var leftBranch = "yes";
        var rightBranch = "yes";
        var leftResult = Guid.NewGuid();
        var rightResult = Guid.NewGuid();
        var leftColumns = new int[] { 0, 1 };
        var rightColumns = new int[] { 2, 3 };
        var json = $"{{\"condition\":\"{condition}\",\"leftBranch\":\"{leftBranch}\",\"rightBranch\":\"{rightBranch}\",\"leftResult\":\"{leftResult}\",\"rightResult\":\"{rightResult}\",\"leftColumns\":[{string.Join(",", leftColumns)}],\"rightColumns\":[{string.Join(",", rightColumns)}]}}";

        var blocks = new Block[]
        {
                new Block{BlockType = EnumBlockType.Branch, Id = id, JsonData = json},
        };

        var actual = sut.BlocksToBlockCollectionDto(blocks, Guid.NewGuid());

        Assert.NotNull(actual.BranchBlocks);
        Assert.Equal(id, actual.BranchBlocks[0].Id);
        Assert.Equal(condition, actual.BranchBlocks[0].Condition);
        Assert.Equal(leftBranch, actual.BranchBlocks[0].LeftBranch);
        Assert.Equal(rightBranch, actual.BranchBlocks[0].RightBranch);
        Assert.Equal(leftResult, actual.BranchBlocks[0].LeftResult);
        Assert.Equal(rightResult, actual.BranchBlocks[0].RightResult);
        Assert.True(leftColumns.All(r => actual.BranchBlocks[0].LeftColumns.Contains(r)));
        Assert.True(rightColumns.All(r => actual.BranchBlocks[0].RightColumns.Contains(r)));
    }

    public static IEnumerable<object[]> TextBlockDtos
    {
        get
        {
            yield return new object[] {
                new TextBlockDTO(Guid.NewGuid(),"test", Guid.Parse("bc45a629-d76e-483d-86d1-ab2bc1a6ab95")),
                "{\"text\":\"test\",\"childId\":\"bc45a629-d76e-483d-86d1-ab2bc1a6ab95\"}" };
            yield return new object[] {
                new TextBlockDTO(Guid.NewGuid(),"test", null),
                "{\"text\":\"test\",\"childId\":null}" };
        }
    }
    public static IEnumerable<object[]> BranchBlockDtos
    {
        get
        {
            yield return new object[] {
                new BranchBlockDTO(Guid.NewGuid(), "condition", "yes", "no", null, null, new(){0, 1}, new(){2}),
                "{\"condition\":\"condition\",\"leftBranch\":\"yes\",\"rightBranch\":\"no\",\"leftResult\":null,\"rightResult\":null,\"leftColumns\":[0,1],\"rightColumns\":[2]}" };
            yield return new object[] {
                new BranchBlockDTO(Guid.NewGuid(), "condition", "yes", "no", Guid.Parse("bc45a629-d76e-483d-86d1-ab2bc1a6ab95"), null, new(){0, 1}, new(){2}),
                "{\"condition\":\"condition\",\"leftBranch\":\"yes\",\"rightBranch\":\"no\",\"leftResult\":\"bc45a629-d76e-483d-86d1-ab2bc1a6ab95\",\"rightResult\":null,\"leftColumns\":[0,1],\"rightColumns\":[2]}" };
            yield return new object[] {
                new BranchBlockDTO(Guid.NewGuid(), "condition", "yes", "no", Guid.Parse("bc45a629-d76e-483d-86d1-ab2bc1a6ab95"), Guid.Parse("bc45a629-d76e-483d-86d1-ab2bc1a6ab90"), new(){0}, new(){1}),
                "{\"condition\":\"condition\",\"leftBranch\":\"yes\",\"rightBranch\":\"no\",\"leftResult\":\"bc45a629-d76e-483d-86d1-ab2bc1a6ab95\",\"rightResult\":\"bc45a629-d76e-483d-86d1-ab2bc1a6ab90\",\"leftColumns\":[0],\"rightColumns\":[1]}" };
        }
    }
}
