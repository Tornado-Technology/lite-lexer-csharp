namespace LiteLexer.Definition;

[AttributeUsage(AttributeTargets.Class)]
public sealed class TokenDefinitionAttribute : Attribute, ITokenDefinition
{
    public string RegexPattern { get; }
    public int Priority { get; }

    public TokenDefinitionAttribute(string pattern, int priority = 0)
    {
        RegexPattern = pattern;
        Priority = priority;
    }
}