namespace T1.ParserKit.SqlDom.Expressions
{
	public class SqlIfExpression : SqlExpression
	{
		public SqlFilterExpression Condition { get; set; }
		public SqlStatementsExpression Body { get; set; }
	}
}