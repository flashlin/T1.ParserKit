﻿using T1.ParserKit.Core;

namespace T1.ParserKit.SqlDom.Expressions
{
	public class SqlVariableAssignFieldExpression : SqlBaseFieldExpression
	{
		public SqlVariableExpression VariableName { get; set; }
		public SqlExpression From { get; set; }
	}
}