namespace T1.ParserKit.SqlDom.Expressions
{
	public class SqlAndOperExpression : SqlExpression
	{
		public SqlExpression Left { get; set; }
		public SqlExpression Right { get; set; }
		public string Oper { get; set; }
	}
}