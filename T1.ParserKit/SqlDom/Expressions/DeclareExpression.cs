namespace T1.ParserKit.SqlDom.Expressions
{
	public class DeclareExpression : SqlExpression
	{
		public VariableExpression Name { get; set; }
		public string DataType { get; set; }
	}
}