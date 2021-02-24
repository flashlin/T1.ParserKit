using T1.ParserKit.Core;

namespace T1.ParserKit.SqlDom.Expressions
{
	public class VariableAssignFieldExpression : SqlExpression
	{
		public VariableExpression VariableName { get; set; }
		public SqlExpression From { get; set; }
	}
}