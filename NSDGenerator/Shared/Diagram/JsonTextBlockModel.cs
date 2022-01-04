namespace NSDGenerator.Shared.Diagram;

public record JsonTextBlockModel(Guid Id, string Text, Guid? ChildId) : IJsonBlockModel;
