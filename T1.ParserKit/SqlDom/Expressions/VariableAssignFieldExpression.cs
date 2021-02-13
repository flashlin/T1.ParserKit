using T1.ParserKit.Core;

namespace T1.ParserKit.SqlDom.Expressions
{
	public class VariableAssignFieldExpression : FieldExpression
	{
		public string VariableName { get; set; }
		public FieldExpression Field { get; set; }
	}
}