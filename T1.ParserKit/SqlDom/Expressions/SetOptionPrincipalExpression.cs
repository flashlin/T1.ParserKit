namespace T1.ParserKit.SqlDom.Expressions
{
	public class SetOptionPrincipalExpression : SqlOptionNameExpression
	{
		public SqlObjectNameExpression Principal { get; set; }
		public bool IsToggle { get; set; }
	}
}