using NSDGenerator.Server.Diagram.Helpers;
using NSDGenerator.Shared.Diagram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace NSDGenerator.Server.Tests.Diagram
{
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
        public void BlocksToBlockCollectionDto_()
        {
            throw new NotImplementedException();
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
}
