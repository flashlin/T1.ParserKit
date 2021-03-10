namespace T1.ParserKit.SqlDom.Expressions
{
	public class SqlDeleteExpression : SqlExpression
	{
		public SqlObjectNameExpression From { get; set; }
		public SqlWhereExpression Where { get; set; }
	}
}