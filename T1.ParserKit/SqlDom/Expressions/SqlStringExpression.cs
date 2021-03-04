namespace T1.ParserKit.SqlDom.Expressions
{
	public class SqlStringExpression : SqlExpression
	{
		public string Text { get; set; }
		public bool IsUnicode { get; set; }
	}
}