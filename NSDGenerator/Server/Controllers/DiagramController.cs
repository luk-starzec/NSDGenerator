using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System;
using NSDGenerator.Shared.Diagram;

namespace NSDGenerator.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiagramController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<DiagramInfoModel> GetDiagrams()
        {
            var diagrams = new List<DiagramInfoModel>
            {
                new DiagramInfoModel(Guid.NewGuid(),"First", new DateTime(2021,1,1,12,0, 0),new DateTime(2021,1,3,12,0, 0)),
                new DiagramInfoModel(Guid.NewGuid(),"Second",new DateTime(2022,1,1,12,0, 0),new DateTime(2022,1,3,12,0, 0)),
            };
            return diagrams;
        }

        [HttpGet("{id}")]
        public JsonDiagram GetDiagram(Guid id)
        {
            var rootId = Guid.NewGuid();
            var diagram = new JsonDiagram
            {
                Id = id,
                Name = $"Test id: {id}",
                BlockCollection = new JsonBlockCollection
                {
                    RootId = rootId,
                    TextBlocks = new List<JsonTextBlockModel>
                    {
                        new JsonTextBlockModel(rootId, "This is text", null),
                    },
                }
            };
            return diagram;
        }
    }
}
