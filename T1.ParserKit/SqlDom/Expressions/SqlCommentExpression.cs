namespace T1.ParserKit.SqlDom.Expressions
{
	public class SqlCommentExpression : SqlExpression
	{
		public bool IsMultipleLines { get; set; }
		public string Content { get; set; }
	}
}