using T1.ParserKit.Core;

namespace T1.ParserKit.SqlDom
{
	public static class SqlToken
	{
		public static IParser Identifier
		{
			get
			{
				var start = Parse.Equal("[");
				var body = Parse.NotEqual("]").Many(1);
				var end = Parse.Equal("]");
				var identifier = Parse.Chain(start, body, end).Merge();

				return identifier.Or(Parse.CStyleIdentifier())
					.Named("SqlIdentifier");
			}
		}
	}
}