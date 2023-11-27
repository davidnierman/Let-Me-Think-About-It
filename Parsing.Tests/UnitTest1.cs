namespace Parsing.Tests
{
    public class UnitTest2
    {
        [Fact]
        public void FirstTest()
        {
            var tokens = new Token[] { new Token("1", TokenType.Number) };
            var parser = new Parser(tokens);
            var expression = parser.Parse();
            var evaluator = new Evaluator(expression);
            var result = evaluator.Evaluate();

            Assert.Equal(1m, result);
        }

        [Fact]
        public void SecondTest()
        {
            var tokens = new Token[] { new Token("2", TokenType.Number) };
            var parser = new Parser(tokens);
            var expression = parser.Parse();
            var evaluator = new Evaluator(expression);
            var result = evaluator.Evaluate();

            Assert.Equal(2m, result);
        }

        [Fact]
        public void ThirdTest()
        {
            var tokens = new Token[] {
                new Token("1", TokenType.Number),
                new Token("+", TokenType.Plus), 
                new Token("2", TokenType.Number)
            };
            var parser = new Parser(tokens);
            var expression = parser.Parse();
            var evaluator = new Evaluator(expression);
            var result = evaluator.Evaluate();

            Assert.Equal(3m, result);
        }

        

        //public void Foo()
        //{
        //    var tokens = new[]
        //    {
        //        new Token("1", TokenType.Number),
        //        new Token("+", TokenType.Plus),
        //        new Token("2", TokenType.Number)
        //    };

        //    var a = tokens[0];
        //    if (tokens.Length == 1)
        //        return a;
        //    else
        //    {
        //        if (tokens[1] != something)
        //        {
        //            //error
        //        }
        //        else
        //        {
        //            new SomeOtherThing(tokens[1], tokens[0], tokens[2]);
        //        }
        //    }
        //}
    }

    abstract record Expression
    {
        public record Literal(decimal Value) : Expression
        {

        }

        public record Binary(Expression Left, TokenType Operator, Expression Right):Expression;
    }

    class Evaluator
    {
        private readonly Expression _expression;

        public Evaluator(Expression expression)
        {
            _expression = expression;
        }

        public decimal Evaluate()
        {
            switch (_expression)
            {
                case Expression.Literal l :
                    return l.Value;
                case Expression.Binary b:
                    switch (b.Operator)
                    {
                        case TokenType.LeftParens:
                            break;
                        case TokenType.RightParens:
                            break;
                        case TokenType.Slash:
                            break;
                        case TokenType.Plus:
                            return ((Expression.Literal)b.Left).Value + ((Expression.Literal)b.Right).Value;
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
            throw new NotImplementedException();
        }
    }
    public class UnitTest1
    {
        // "1 + 2" == 3
        // "1+2" == 3
        // "            1+      
        // 2" == 3
        // "1 + 1/2" == 1.5
        // "2/2 + 1/(1+1)" == 1.5
        // "(2/2 + 1/(1+1)) *1e3" == 150
        // "asin(4/5)" == ~57.2 advanced, future work

        //TODO decimals?
        public static IEnumerable<object[]> ValidStringsForTokenizing()
        {
            yield return ConvertToData("");
            yield return ConvertToData("1", new Token("1", TokenType.Number));
            yield return ConvertToData(
                """

                        
                  
                """
                );
            yield return ConvertToData("1 + 2",
                new Token("1", TokenType.Number),
                new Token("+", TokenType.Plus),
                new Token("2", TokenType.Number));

            yield return ConvertToData("11+30000",
                new Token("11", TokenType.Number),
                new Token("+", TokenType.Plus),
                new Token("30000", TokenType.Number));


            yield return ConvertToData(
                """
                    1+
                    2
                    """,
                new Token("1", TokenType.Number),
                new Token("+", TokenType.Plus),
                new Token("2", TokenType.Number)
            );

            yield return ConvertToData("1 + 2",
                new Token("1", TokenType.Number),
                new Token("+", TokenType.Plus),
                new Token("2", TokenType.Number));

            yield return ConvertToData("(2/2 + 1/(1+1)) *1e3",
                new Token("(", TokenType.LeftParens),
                new Token("2", TokenType.Number),
                new Token("/", TokenType.Slash),
                new Token("2", TokenType.Number),
                new Token("+", TokenType.Plus),
                new Token("1", TokenType.Number),
                new Token("/", TokenType.Slash),
                new Token("(", TokenType.LeftParens),
                new Token("1", TokenType.Number),
                new Token("+", TokenType.Plus),
                new Token("1", TokenType.Number),
                new Token("(", TokenType.RightParens),
                new Token("(", TokenType.RightParens),
                new Token("*", TokenType.Star),
                new Token("1e3", TokenType.Number)
            );

            yield return ConvertToData("1E3",

                new Token("1E3", TokenType.Number)
            );

            yield return ConvertToData("1.5",
                new Token("1.5", TokenType.Number)
            );

            static object[] ConvertToData(string input, params Token[] tokens)
            {
                return new object[] { input, tokens.Concat(new[] { new Token(null, TokenType.Eof) }) };
            }
        }

        [Theory]
        [MemberData(nameof(ValidStringsForTokenizing))]
        public void CanTokenizeStringCorrectly(string input, Token[] expected)
        {
            var tokenizer = new Tokenizer(input);
            Assert.Equal(expected, tokenizer.Tokenize());
        }


        public static IEnumerable<object[]> InvalidStringsForTokenizing()
        {
            yield return new object[] { "1,2" };
            yield return new object[] { "1ee2" };
            yield return new object[] { "1..2" };
            //TODO: dave tests
            //yield return new object[] { "1.e2" };
            //yield return new object[] { "1e.2" };

        }
        [Theory]
        [MemberData(nameof(InvalidStringsForTokenizing))]
        public void UnexpectedCharacterShouldThrow(string input)
        {
            var tokenizer = new Tokenizer(input);
            var ex = Assert.Throws<Exception>(() => tokenizer.Tokenize());
            Assert.Equal("Unexpected character", ex.Message);
        }

    }

    class Tokenizer // normally called a lexer
    {
        private readonly string _input;
        int _position = 0;
        int _startPosition = 0;

        public Tokenizer(string input)
        {
            _input = input;
        }

        public IReadOnlyList<Token> Tokenize()
        {
            List<Token> tokens = new List<Token>();
            while (_position < _input.Length)
            {
                var c = _input[_position];
                switch (c)
                {
                    case '\r':
                    case '\n':
                    case ' ':
                    case '\t':
                        break;

                    case '+':
                        tokens.Add(new Token("+", TokenType.Plus));
                        break;
                    case '/':
                        tokens.Add(new Token("/", TokenType.Slash));
                        break;
                    case '*':
                        tokens.Add(new Token("*", TokenType.Star));
                        break;
                    case '(':
                        tokens.Add(new Token("(", TokenType.LeftParens));
                        break;
                    case ')':
                        tokens.Add(new Token("(", TokenType.RightParens));
                        break;
                    default:
                        if (char.IsAsciiDigit(c))
                        {
                            tokens.Add(MatchNumber());

                            break;
                        }

                        throw new Exception("Unexpected character");
                }

                _startPosition = _position + 1;
                _position++;

            }

            tokens.Add(new Token(null, TokenType.Eof));
            return tokens;
        }

        Token MatchNumber()
        {
            var count = 1;
            var isExponent = false;
            var isDecimal = false;
            //TODO: blurgh
            //TODO: mention ANT4LR
            while (_position < _input.Length)
            {
                if (char.ToLowerInvariant(Peek()) == 'e')
                {
                    if (isExponent)
                    {
                        throw new Exception("Unexpected character");
                    }
                    count++;
                    _position++;
                    isExponent = true;
                    continue;
                }

                if (Peek() == '.')
                {
                    if (isDecimal)
                    {
                        throw new Exception("Unexpected character");
                    }
                    count++;
                    _position++;
                    isDecimal = true;
                    continue;
                }
                if (!char.IsAsciiDigit(Peek()))
                {
                    break;
                }


                count++;
                _position++;
            }

            return new Token(_input.Substring(_startPosition, count), TokenType.Number);

        }

        char Peek()
        {
            if (_position < _input.Length - 1)
            {
                return _input[_position + 1];
            }
            else
            {
                return '\0';
            }
        }
    }
    public record Token(string Lexeme, TokenType Type);

    //TODO: start here with binary expressions
    public enum TokenType
    {
        LeftParens, RightParens,
        Slash, Plus, Star, Minus,
        Number,
        Eof
    }
}