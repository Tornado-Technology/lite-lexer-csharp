using JetBrains.Annotations;
using LiteLexer.Definition;

namespace LiteLexer;

[PublicAPI]
public abstract class Token
{
    public static readonly Dictionary<ITokenDefinition, Type> Classes;

    public string Content;
    public string Name => this.GetType().Name;

    static Token()
    {
        var tokenClasses = new Dictionary<ITokenDefinition, Type>();
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes())
            {
                var data = Attribute.GetCustomAttribute(type, typeof(TokenDefinitionAttribute));
                if (data is null)
                    continue;

                tokenClasses[(ITokenDefinition) data] = type;
            }
        }

        Classes = tokenClasses;
    }
    
    protected Token(string content)
    {
        Content = content;
    }
}