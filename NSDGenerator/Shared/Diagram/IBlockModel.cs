using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSDGenerator.Shared.Diagram
{
    public interface IBlockModel
    {
        public IBlockModel Parent { get; set; }
        public Guid Id { get; set; }
    }
}
