using System.Collections.Concurrent;

namespace T1.ParserKit.Core
{
	public class CacheParser : IParser
	{
		private readonly IParser _parser;
		private static readonly ConcurrentDictionary<ITextSpan, IParseResult> _cache = new ConcurrentDictionary<ITextSpan, IParseResult>();

		public CacheParser(IParser parser)
		{
			_parser = parser;
		}

		public string Name
		{
			get => _parser.Name;
			set => _parser.Name = value;
		}

		public IParseResult TryParse(ITextSpan inp)
		{
			if (_cache.TryGetValue(inp, out var rc))
			{
				return rc;
			}

			rc = _parser.TryParse(inp);
			_cache[inp] = rc;
			return rc;
		}
	}
}