namespace T1.ParserKit.SqlDom.Expressions
{
	public class SqlGrantPermissionToExpression : SqlExpression
	{
		public SqlIdentifierExpression ToPrincipal { get; set; }
	}
}