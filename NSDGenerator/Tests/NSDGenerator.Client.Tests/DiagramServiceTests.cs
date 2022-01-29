using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using Xunit;
using NSDGenerator.Client.Services;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using NSDGenerator.Client.Helpers;
using NSDGenerator.Shared.Diagram;
using System.Collections.Generic;
using System;
using NSDGenerator.Client.ViewModels;
using System.Linq;

namespace NSDGenerator.Client.Tests
{
    public class DiagramServiceTests
    {
        private readonly IDiagramService sut;
        private readonly Mock<HttpMessageHandler> handlerMock = new();
        private readonly Mock<IModelConverter> modelConverterStub = new();
        private readonly Mock<IJSRuntime> jsRuntimeStub = new();

        public DiagramServiceTests()
        {
            var loggerStub = new Mock<ILogger<DiagramService>>();

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object) { BaseAddress = new Uri("https://localhost") };

            sut = new DiagramService(loggerStub.Object, httpClient, jsRuntimeStub.Object, modelConverterStub.Object);
        }

        [Fact]
        public async Task GetMyDiagramsAsync_CallsApiEndpoint()
        {
            var url = "api/diagram";

            await sut.GetMyDiagramsAsync();

            VerifyApiCall(url, HttpMethod.Get);
        }

        [Fact]
        public async Task CheckIfDiagramExistsAsync_CallsApiEndpoint()
        {
            var id = Guid.NewGuid();
            var url = $"api/diagram/exists/{id}";

            await sut.CheckIfDiagramExistsAsync(id);

            VerifyApiCall(url, HttpMethod.Get);
        }

        [Fact]
        public async Task GetDiagramAsync_CallsApiEndpoint()
        {
            var id = Guid.NewGuid();
            var url = $"api/diagram/{id}";

            await sut.GetDiagramAsync(id);

            VerifyApiCall(url, HttpMethod.Get);
        }

        [Fact]
        public async Task SaveDiagramAsync_CallsApiEndpoint()
        {
            modelConverterStub
                .Setup(r => r.DiagramViewModelToJson(It.IsAny<DiagramVM>()))
                .Returns(string.Empty);

            var url = $"api/diagram";

            await sut.SaveDiagramAsync(new DiagramVM());

            VerifyApiCall(url, HttpMethod.Post);
        }

        [Fact]
        public async Task DeleteDiagramAsync_CallsApiEndpoint()
        {
            var id = Guid.NewGuid();
            var url = $"api/diagram/{id}";

            await sut.DeleteDiagramAsync(id);

            VerifyApiCall(url, HttpMethod.Delete);
        }

        [Fact]
        public void GetDiagram_ConvertsFileContentToViewModel()
        {
            var expected = new DiagramVM();

            modelConverterStub
                .Setup(r => r.JsonToDiagramViewModel(It.IsAny<string>()))
                .Returns(expected);

            var actual = sut.GetDiagram("");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CreateNewDiagram_ReturnsDiagramViewModel()
        {
            var actual = sut.CreateNewDiagram();

            Assert.NotNull(actual);
        }

        [Fact]
        public void CreateDiagramCopy_ReturnsNewDiagramWithCopiedBlocksAndColumnWidths()
        {
            var childBlock = new TextBlockVM("child");
            var rootBlock = new TextBlockVM("root", childBlock);

            modelConverterStub
                .Setup(r => r.RootBlockToBlockCollectionDto(It.IsAny<IBlockVM>()))
                .Returns(new BlockCollectionDTO()
                {
                    RootId = rootBlock.Id,
                    TextBlocks = new List<TextBlockDTO>
                    {
                        new TextBlockDTO(childBlock.Id, childBlock.Text, null),
                        new TextBlockDTO(rootBlock.Id, rootBlock.Text, childBlock.Id),
                    }
                });
            modelConverterStub
                .Setup(r => r.BlockCollectionDtoToRootBlock(It.IsAny<BlockCollectionDTO>()))
                .Returns(new TextBlockVM(rootBlock.Text) { Child = new TextBlockVM(childBlock.Text) });

            var vm = new DiagramVM
            {
                Id = Guid.NewGuid(),
                RootBlock = rootBlock,
                ColumnsWidth = new List<int> { 1 },
            };

            var actual = sut.CreateDiagramCopy(vm);

            Assert.NotNull(actual);
            Assert.NotEqual(vm.Id, actual.Id);
            Assert.NotEqual(vm.RootBlock.Id, actual.RootBlock.Id);
            Assert.Equal(((TextBlockVM)vm.RootBlock).Text, ((TextBlockVM)actual.RootBlock).Text);
            Assert.NotEqual(((TextBlockVM)vm.RootBlock).Child.Id, ((TextBlockVM)actual.RootBlock).Child.Id);
            Assert.Equal(((TextBlockVM)((TextBlockVM)vm.RootBlock).Child).Text, ((TextBlockVM)((TextBlockVM)actual.RootBlock).Child).Text);
            Assert.True(vm.ColumnsWidth.All(r => actual.ColumnsWidth.Contains(r)));
        }

        private void VerifyApiCall(string url, HttpMethod method)
        {
            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1)
               , ItExpr.Is<HttpRequestMessage>(req => req.Method == method && req.RequestUri.AbsolutePath.EndsWith(url))
               , ItExpr.IsAny<CancellationToken>());
        }
    }
}