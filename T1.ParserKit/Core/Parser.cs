using System;

namespace T1.ParserKit.Core
{
	public class Parser : IParser
	{
		private readonly Func<ITextSpan, IParseResult> _func;

		public Parser(string name, Func<ITextSpan, IParseResult> func)
		{
			_func = func;
			Name = name;
		}

		public string Name { get; set; }

		public IParseResult TryParse(ITextSpan inp)
		{
			return _func(inp);
		}

		public override string ToString()
		{
			return $"{Name}";
		}
	}
}