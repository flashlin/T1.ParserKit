using System.Collections.Generic;

namespace T1.ParserKit.SqlDom.Expressions
{
	public class SqlCreateStoredProcedureExpression : SqlExpression
	{
		public ObjectNameExpression Name { get; set; }
		public SqlParameterExpression[] Parameters { get; set; }
		public SqlExpression Body { get; set; }
	}
}