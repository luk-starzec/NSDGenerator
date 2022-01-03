using System.Text.Json;

namespace NSDGenerator.Shared.Diagram;

public class TextBlockModel : IBlockModel
{
    public IBlockModel Parent { get; set; }
    public Guid Id { get; set; }
    public string Text { get; set; }

    private IBlockModel child;
    public IBlockModel Child
    {
        get => child;
        set
        {
            child = value;
            if (child is not null)
                child.Parent = this;
        }
    }

    public string ToJson(JsonSerializerOptions options) => JsonSerializer.Serialize(new JsonTextBlockModel(Id, Parent?.Id, Text), options);

    public TextBlockModel()
    {
        Id = Guid.NewGuid();
    }

    public TextBlockModel(string text, IBlockModel child = null) : this()
        => (Text, Child) = (text, child);


    private record JsonTextBlockModel(Guid Id, Guid? ParentId, string Text, string BlockType = nameof(TextBlockModel));
}
