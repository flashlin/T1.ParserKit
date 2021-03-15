namespace T1.ParserKit.SqlDom.Expressions
{
	public class SqlSourceExpression : SqlExpression
	{
		public SqlExpression Item { get; set; }
		public string AliasName { get; set; }
	}
}