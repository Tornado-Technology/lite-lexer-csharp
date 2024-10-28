using System.Reflection;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using LiteLexer.Definition;

namespace LiteLexer;

[PublicAPI]
public sealed class Lexer
{
    private List<Token> _tokens = [];
    private int _position;
    private string _code = string.Empty;
      
    public List<Token> Parse(string content)
    {
        return Parse(content, Token.Classes);
    }

    public List<Token> Parse(string content, List<ITokenDefinition> tokensDefinitions)
    {
        // Build token list
        var tokens = new Dictionary<ITokenDefinition, Type>();
        foreach (var tokenDefinition in tokensDefinitions)
        {
            tokens[tokenDefinition] = tokenDefinition.GetType();
        }

        return Parse(content, tokens);
    }
    
    public List<Token> Parse(string content, Dictionary<ITokenDefinition, Type> tokensDefinitions)
    {
        // Reset variables
        _code = content;
        _tokens = [];
        _position = 0;
        
        // Sorts token by priority
        var tokenList = tokensDefinitions.ToList();
        tokenList.Sort((pair1, pair2) => pair1.Key.Priority.CompareTo(pair2.Key.Priority));

        // Freeze thread
        while (NextToken(tokenList))
        {
        }
        
        return _tokens;
    }

    private bool NextToken(List<KeyValuePair<ITokenDefinition, Type>> tokens)
    {
        if (_position >= _code.Length)
            return false;

        foreach (var (attribute, tokenType) in tokens)
        {
            var result = BuildRegex(attribute.RegexPattern).Match(_code[_position..]);
            if (!result.Success)
                continue;
            
            var token = InstantiateToken(tokenType, result.Value);
            
            _tokens.Add(token);
            _position += result.Value.Length;
            return true;
        }

        _position++;
        return true;
    }

    private static Regex BuildRegex(string pattern)
    {
        return new Regex($"^{pattern}", RegexOptions.None);
    }

    private static Token InstantiateToken(Type type, string content)
    {
        if (!type.IsAssignableTo(typeof(Token)))
            throw new ArgumentException();

        var constructor = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        return (Token) constructor[0].Invoke([content]);
    }
}