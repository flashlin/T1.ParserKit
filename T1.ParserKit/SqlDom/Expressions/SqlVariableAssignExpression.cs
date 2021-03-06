namespace T1.ParserKit.SqlDom.Expressions
{
	public class SqlVariableAssignExpression : SqlExpression
	{
		public SqlVariableExpression VariableName { get; set; }
		public SqlExpression AssignFrom { get; set; }
	}
}