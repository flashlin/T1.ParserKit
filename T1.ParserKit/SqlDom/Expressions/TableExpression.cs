namespace T1.ParserKit.SqlDom.Expressions
{
	public class TableExpression : SqlExpression
	{
		public string Name { get; set; }
		public string AliasName { get; set; }
		public WithOptionExpression WithOption { get; set; }
	}
}