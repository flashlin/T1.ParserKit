using System;

namespace T1.ParserKit.Core
{
	public class StringPtr
	{
		public static StringPtr Empty = new StringPtr(String.Empty);

		private readonly string _content;

		public StringPtr(string str)
		{
			_content = str;
		}

		public int GetLength()
		{
			return _content.Length;
		}

		public string Substring(int pos, int len)
		{
			return _content.Substring(pos, len);
		}
	}
}