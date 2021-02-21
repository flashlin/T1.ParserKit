using System;
using System.Collections.Generic;

namespace T1.ParserKit.Core
{
	public class Parser<T> : IParser<T>
	{
		private readonly Func<IInputReader, IParseResult<T>> _func;

		public Parser(string name, Func<IInputReader, IParseResult<T>> func)
		{
			_func = func;
			Name = name;
		}

		public string Name { get; set; }

		public IParseResult<T> TryParse(IInputReader inp)
		{
			return _func(inp);
		}

		public override string ToString()
		{
			return $"{Name}";
		}
	}
}