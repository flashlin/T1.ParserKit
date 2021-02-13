﻿using T1.ParserKit.Core;

namespace T1.ParserKit.SqlDom.Expressions
{
	public class FilterExpression : SqlExpression
	{
		public SqlExpression Left { get; set; }
		public string Oper { get; set; }
		public SqlExpression Right { get; set; }
	}
}