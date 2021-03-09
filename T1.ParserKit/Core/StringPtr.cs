using System;

namespace T1.ParserKit.Core
{
	public class StringPtr
	{
		public static StringPtr Empty = new StringPtr()
		{
			Content = String.Empty
		};

		public string Content { get; set; }

		public int GetLength()
		{
			return Content.Length;
		}

		public string Substring(int pos, int len)
		{
			return Content.Substring(pos, len);
		}
	}
}