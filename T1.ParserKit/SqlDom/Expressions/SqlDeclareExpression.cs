namespace T1.ParserKit.SqlDom.Expressions
{
	public class SqlDeclareExpression : SqlExpression
	{
		public SqlVariableExpression Name { get; set; }
		public string DataType { get; set; }
	}
}