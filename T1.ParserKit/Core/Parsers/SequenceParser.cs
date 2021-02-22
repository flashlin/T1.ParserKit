using System.Collections.Generic;
using System.Linq;
using T1.Standard.Extensions;

namespace T1.ParserKit.Core.Parsers
{
	public class SequenceParser<T> : IParser<IEnumerable<T>>
	{
		private readonly IParser<T>[] _parsers;

		public SequenceParser(IEnumerable<IParser<T>> parsers)
		{
			_parsers = parsers.CastArray();
			Name = string.Join(" ", _parsers.Select(x => x.Name));
		}

		public string Name { get; set; }

		public IParseResult<IEnumerable<T>> TryParse(IInputReader inp)
		{
			var acc = new T[_parsers.Length];
			var idx = 0;
			var pos = inp.GetPosition();

			foreach (var parser in _parsers)
			{
				var parsed = parser.TryParse(inp);
				if (!parsed.IsSuccess())
				{
					inp.Seek(pos);
					return Parse.Error<IEnumerable<T>>(parsed.Error);
				}
				acc[idx] = parsed.Result;
				idx++;
			}

			return Parse.Success<IEnumerable<T>>(acc);
		}

		public override string ToString()
		{
			return Name;
		}
	}
}