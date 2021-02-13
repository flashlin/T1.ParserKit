namespace T1.ParserKit.SqlDom.Expressions
{
	public class FieldExpression : SqlExpression
	{
		public string Name { get; set; }
		public string AliasName { get; set; }
		public string From { get; set; }
	}
}