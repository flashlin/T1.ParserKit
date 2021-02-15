namespace T1.ParserKit.SqlDom.Expressions
{
	public class IfExpression : SqlExpression
	{
		public FilterExpression Condition { get; set; }
		public StatementsExpression Body { get; set; }
	}
}