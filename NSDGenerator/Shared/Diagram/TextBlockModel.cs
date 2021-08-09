using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSDGenerator.Shared.Diagram
{
    public class TextBlockModel : IBlockModel
    {
        public IBlockModel Parent { get; set; }
        public Guid Id { get; set; }
        public string Text { get; set; }
        public IBlockModel Child { get; set; }

        public TextBlockModel()
        { }

        public TextBlockModel(string text, IBlockModel child = null)
            => (Text, Child, Id) = (text, child, Guid.NewGuid());

    }
}
