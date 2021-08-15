using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSDGenerator.Shared.Diagram
{
    public class BranchBlockModel : IBlockModel
    {
        public Guid Id { get; set; }
        public IBlockModel Parent { get; set; }
        public string Condition { get; set; }
        public string LeftBranch { get; set; } = "Yes";
        public string RightBranch { get; set; } = "No";
        public IBlockModel LeftResult { get; set; }
        public IBlockModel RightResult { get; set; }

        public BranchBlockModel()
        {
            Id = Guid.NewGuid();
        }
    }
}
