using System;
using System.Collections.Generic;

namespace T1.ParserKit.Core
{
	public class Parser : IParser
	{
		private readonly Func<IInputReader, IParseResult> _func;

		public Parser(string name, Func<IInputReader, IParseResult> func)
		{
			_func = func;
			Name = name;
		}

		public string Name { get; set; }

		public IParseResult TryParse(IInputReader inp)
		{
			return _func(inp);
		}

		public override string ToString()
		{
			return $"{Name}";
		}
	}
}