namespace Parsing.Tests;

class Parser
{
    private readonly IReadOnlyList<Token> _tokens;

    public Parser(IReadOnlyList<Token> tokens)
    {
        _tokens = tokens;
    }
    public Expression Parse(int index = 0)
    {
        //?? how do we get the value of the token?
        var t=  _tokens[index];
        switch (t.Type)
        {
            case TokenType.LeftParens:
                break;
            case TokenType.RightParens:
                break;
            case TokenType.Slash:
                break;
            case TokenType.Plus:
                break;
            case TokenType.Star:
                break;
            case TokenType.Number:
                if (index != _tokens.Count - 1)
                {
                    return Binary(new Expression.Literal(decimal.Parse(t.Lexeme)),index + 1);
                }
                else
                {
                    return new Expression.Literal(decimal.Parse(t.Lexeme));
                }

                break;
            case TokenType.Eof:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        throw new NotImplementedException();
    }

    Expression Binary(Expression left, int index)
    {
        var token = _tokens[index];
        var right = Parse(index + 1);

        switch(token.Type)
        {
            case TokenType.LeftParens:
                break;
            case TokenType.RightParens:
                break;
            case TokenType.Slash:
                break;
            case TokenType.Plus:
                return new Expression.Binary(left, TokenType.Plus, right);
                break;
            case TokenType.Star:
                break;
            case TokenType.Number:
                break;
            case TokenType.Eof:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        throw new NotImplementedException();
    }
}