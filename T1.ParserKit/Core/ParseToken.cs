using System.Linq;

namespace T1.ParserKit.Core
{
	public static class ParseToken
	{
		public static IParser<T> FollowedByEos<T>(this IParser<T> parser)
		{
			return parser.ThenLeft(Parse.Eos<T>());
		}

		public static IParser<T> Lexeme<T>(IParser<T> parser)
		{
			return from _ in Parse.Blanks.Optional()
					 from value in parser
					 select value;
		}

		public static IParser<string> Assertion()
		{
			return new Parser<string>("Assertion", inp =>
			{
				if (inp.Eof())
				{
					return Parse.Success<string>(inp);
				}

				var blank = Parse.Blank.TryParse(inp);
				if (blank.IsSuccess())
				{
					return Parse.Success<string>(inp);
				}

				var ch = inp.Substr(1);
				var error = Parse.Error<string>($"Expect assertion, but got '{ch}' at {inp}.", inp);

				var letter = Parse.Letter.TryParse(inp);
				if (letter.IsSuccess())
				{
					return error;
				}
				return Parse.Success<string>(inp);
			});
		}

		public static IParser<TextSpan> Match(string text)
		{
			return Lexeme(
				from value in Parse.Match(text)
				from assertion in Assertion()
				select value);
		}

		public static IParser<TextSpan> Symbol(string symbol)
		{
			return Lexeme(Parse.Equal(symbol));
		}

		public static IParser<TextSpan> Contains(params string[] texts)
		{
			var sorted = texts.OrderByDescending(x => x.Length);
			return Parse.Contains(texts).ThenLeft(Assertion());
		}
	}
}