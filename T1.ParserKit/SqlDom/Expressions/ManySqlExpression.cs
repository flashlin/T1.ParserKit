using T1.ParserKit.Core;

namespace T1.ParserKit.SqlDom.Expressions
{
	public class ManySqlExpression : SqlExpression
	{
		public ITextSpan[] Items { get; set; }
	}
}