namespace T1.ParserKit.SqlDom.Expressions
{
	public class SqlFunctionExpression : SqlExpression
	{
		public string Name { get; set; }
		public SqlExpression[] Parameters { get; set; }
	}
}