namespace T1.ParserKit.SqlDom.Expressions
{
	public class UpdateExpression : SqlExpression
	{
		public UpdateSetFieldExpression[] SetFields { get; set; }
		public WhereExpression WhereExpr { get; set; }
	}
}