namespace T1.ParserKit.SqlDom.Expressions
{
	public class SqlExecExpression : SqlExpression
	{
		public ObjectNameExpression Name { get; set; }
		public SqlExpression[] Parameters { get; set; }
	}
}