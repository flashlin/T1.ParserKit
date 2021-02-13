namespace T1.ParserKit.SqlDom.Expressions
{
	public class SourceExpression : SqlExpression
	{
		public SqlExpression Item { get; set; }
		public string AliasName { get; set; }
	}
}