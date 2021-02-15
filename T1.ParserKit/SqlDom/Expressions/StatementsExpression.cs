using System.Collections.Generic;

namespace T1.ParserKit.SqlDom.Expressions
{
	public class StatementsExpression : SqlExpression
	{
		public SqlExpression[] Items { get; set; }
	}
}