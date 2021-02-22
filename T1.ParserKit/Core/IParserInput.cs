using System;
using System.Collections.Generic;
using System.Text;

namespace T1.ParserKit.Core
{
	public interface ITextSpan
	{
		string Text { get; set; }
		string File { get; set; }
		int Length { get; set; }
		int Position { get; set; }
	}
}
