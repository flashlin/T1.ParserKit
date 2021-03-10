namespace T1.ParserKit.SqlDom.Expressions
{
	public class SetOptionPrincipalExpression : SqlExecExpression
	{
		public string OptionName { get; set; }
		public SqlObjectNameExpression Principal { get; set; }
		public bool IsToggle { get; set; }
	}
}