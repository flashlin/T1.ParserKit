namespace T1.ParserKit.SqlDom.Expressions
{
	public class SqlParameterExpression : SqlExpression
	{
		public VariableExpression Name { get; set; }
		public SqlDataTypeExpression DataTypeExpr { get; set; }
	}
}