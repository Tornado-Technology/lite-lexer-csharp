namespace LiteLexer.Definition;

public interface ITokenDefinition
{
    public string RegexPattern { get; }
    public int Priority { get; }
}