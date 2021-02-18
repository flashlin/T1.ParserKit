namespace T1.ParserKit.SqlDom.Expressions
{
	public class UpdateSetFieldExpression : SqlExpression
	{
		public string FieldName { get; set; }
		public SqlExpression AssignExpr { get; set; }
	}
}