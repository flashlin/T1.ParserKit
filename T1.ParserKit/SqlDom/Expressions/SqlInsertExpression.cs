namespace T1.ParserKit.SqlDom.Expressions
{
	public class SqlInsertExpression : SqlExpression
	{
		public ObjectNameExpression Table { get; set; }
		public SqlBaseFieldExpression[] Fields { get; set; }
		public SqlInsertRowExpression[] InsertRows { get; set; }
	}
}