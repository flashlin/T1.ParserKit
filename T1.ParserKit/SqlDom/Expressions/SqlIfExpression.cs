namespace T1.ParserKit.SqlDom.Expressions
{
	public class SqlIfExpression : SqlExpression
	{
		public SqlFilterExpression Condition { get; set; }
		public StatementsExpression Body { get; set; }
	}
}