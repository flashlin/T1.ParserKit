//using System;
//using System.Collections.Generic;
//using System.Text;
//using T1.ParserKit.Core;

//namespace T1.ParserKit.SqlDom
//{
//	public class SqlTokenizer
//	{
//		public string[] Symbols = new[]
//		{
//			"[", "]", ",", ".", ";", "@", "::", ":", "\\", "$",
//			">=", "<>", "<=", "<", ">", "=", "!=", "+", "-", "*", "/",
//			"&", "|", "~", "^", "%", "(", ")"
//		};

//		public IParser KeywordsParser =>
//			Parse.Contains(Keywords, true).Assertion(true);

//		public IParser SymbolsParser =>
//			Parse.Contains(Symbols).Assertion(false);

//		public IParser SqlIdentifier
//		{
//			get
//			{
//				var start = Parse.Equal("[");
//				var body = Parse.NotEqual("]").Many(1);
//				var end = Parse.Equal("]");
//				var identifier = Parse.Chain(start, body, end).Merge();
//				return identifier.Or(Parse.CStyleIdentifier())
//					.Named("SqlIdentifier");
//			}
//		}


//		public IParser TmpVariable => Parse.Equal("@").Then(SqlIdentifier)
//			.Named("variable");

//		public ITextSpan Tokenize(string code)
//		{
//			var p = Parse.Any(
//				Parse.Blanks().Skip(),
//				KeywordsParser,
//				Parse.Digits(),
//				SqlIdentifier,
//				TmpVariable,
//				SymbolsParser
//			).Many(1, 1).Then(Parse.Eos());
//			return p.ParseText(code);
//		}
//	}
//}
