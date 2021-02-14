using System;

namespace T1.ParserKit.SqlDom.Expressions
{
	public class NumberExpression : SqlExpression
	{
		public object Value { get; set; }

		public string ValueTypeFullname { get; set; }
	}
}