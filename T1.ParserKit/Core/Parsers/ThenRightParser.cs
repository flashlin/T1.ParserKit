using System;
using System.Collections.Generic;
using System.Text;

namespace T1.ParserKit.Core.Parsers
{
	public class ThenRightParser<T1, T2> : IParser<T2>
	{
		private readonly IParser<T1> _p1;
		private readonly IParser<T2> _p2;
		public ThenRightParser(IParser<T1> p1, IParser<T2> p2)
		{
			Name = $"{p1.Name} >>. {p2.Name}";
			_p1 = p1;
			_p2 = p2;
		}

		public string Name { get; set; }
		public IParseResult<T2> TryParse(IInputReader inp)
		{
			var parsed1 = _p1.TryParse(inp);
			if (!parsed1.IsSuccess())
			{
				return Parse.Error<T2>(parsed1.Error);
			}
			return _p2.TryParse(inp);
		}
	}
}
