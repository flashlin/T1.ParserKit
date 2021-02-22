using System.Collections.Generic;
using T1.Standard.Extensions;

namespace T1.ParserKit.Core.Parsers
{
	public class SequenceParser<T> : IParser<IEnumerable<T>>
	{
		private readonly IParser<T>[] _parsers;

		public SequenceParser(IEnumerable<IParser<T>> parsers)
		{
			_parsers = parsers.CastArray();
		}

		public string Name { get; set; }

		public IParseResult<IEnumerable<T>> TryParse(IInputReader inp)
		{
			var acc = new T[_parsers.Length];
			var idx = 0;

			foreach (var parser in _parsers)
			{
				var parsed = parser.TryParse(inp);
				if (!parsed.IsSuccess())
				{
					return Parse.Error<IEnumerable<T>>(parsed.Error);
				}
				acc[idx] = parsed.Result;
				idx++;
			}
			return Parse.Success<IEnumerable<T>>(acc);
		}
	}
}