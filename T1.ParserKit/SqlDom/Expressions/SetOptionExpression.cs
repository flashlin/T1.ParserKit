namespace T1.ParserKit.SqlDom.Expressions
{
	public class SetOptionExpression : SqlExpression
	{
		public string OptionName { get; set; }
		public bool IsToggle { get; set; }
	}
}