using System.Collections.Generic;

namespace T1.ParserKit.SqlDom.Expressions
{
	public class SqlSelectExpression : SqlExpression
	{
		public SqlExpression[] Fields { get; set; }
		public SqlExpression From { get; set; }
		public SqlWhereExpression Where { get; set; }
	}
}