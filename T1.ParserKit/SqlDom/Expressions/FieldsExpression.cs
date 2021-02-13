using System.Collections.Generic;

namespace T1.ParserKit.SqlDom.Expressions
{
	public class FieldsExpression : SqlExpression
	{
		public List<SqlExpression> Items { get; set; }
	}
}