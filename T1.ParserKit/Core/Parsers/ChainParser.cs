using System.Collections.Generic;

namespace T1.ParserKit.Core.Parsers
{
	public class ChainParser<T> : IParser<T>
	{
		private readonly IEnumerable<IParser<T>> _parsers;

		public ChainParser(IEnumerable<IParser<T>> parsers)
		{
			_parsers = parsers;
		}

		public string Name { get; set; }

		public IParseResult<T> TryParse(IInputReader inp)
		{
			IParseResult<T> parsed = null;

			foreach (var parser in _parsers)
			{
				parsed = parser.TryParse(inp);

				if (!parsed.IsSuccess())
				{
					break;
				}
			}

			return parsed;
		}
	}
}