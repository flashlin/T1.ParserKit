using System.Collections.Generic;

namespace T1.ParserKit.SqlDom.Expressions
{
	public class SelectExpression : SqlExpression
	{
		public FieldsExpression Fields { get; set; }
		public SqlExpression From { get; set; }
	}
}