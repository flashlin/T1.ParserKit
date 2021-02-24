using System;
using System.Collections.Generic;
using System.Text;
using T1.ParserKit.Core;

namespace T1.ParserKit.SqlDom.Expressions
{
	public class SqlExpression
	{
		public TextSpan TextSpan { get; set; }

		public string GetText()
		{
			return TextSpan.Text;
		}
	}
}
