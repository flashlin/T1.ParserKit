namespace T1.ParserKit.SqlDom.Expressions
{
	public class SqlTableExpression : SqlExpression
	{
		public string Name { get; set; }
		public string AliasName { get; set; }
		public SqlWithOptionExpression WithOption { get; set; }
	}
}