namespace T1.ParserKit.SqlDom.Expressions
{
	public class SqlDataTypeExpression : SqlExpression
	{
		public string DataType { get; set; }
		public int Size { get; set; }
		public int Scale { get; set; }
	}
}