namespace T1.ParserKit.SqlDom.Expressions
{
	public class SqlUpdateExpression : SqlExpression
	{
		public UpdateSetFieldExpression[] SetFields { get; set; }
		public SqlWhereExpression WhereExpr { get; set; }
		public SqlObjectNameExpression Table { get; set; }
	}
}