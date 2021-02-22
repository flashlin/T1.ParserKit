using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using T1.Standard.Extensions;

namespace T1.ParserKit.Core
{
	public static class TextSpanExtension
	{
		public static string Substr(this ITextSpan textSpan, int len)
		{
			var maxLen = (len > textSpan.Content.Length) ? textSpan.Content.Length : len;
			return textSpan.Content.Substring(0, maxLen);
		}

		public static T FirstCast<T>(this IEnumerable<ITextSpan> textSpans)
			where T : ITextSpan
		{
			return (T)textSpans.FirstOrDefault(x => x is T);
		}

		public static TextSpan GetTextSpan<T>(this IEnumerable<T> textSpans)
			where T : ITextSpan
		{
			var arr = textSpans.CastArray();
			var hd = arr.First();
			var tl = arr.Last();
			var prev = hd;

			var str = new StringBuilder();
			str.Append(hd.Content);
			foreach (var span in arr.Skip(1))
			{
				if (span.Position != prev.Position + prev.Length)
				{
					str.Append(" ");
				}
				str.Append(span.Content);
				prev = span;
			}
			return new TextSpan()
			{
				File = hd.File,
				Position = hd.Position,
				Content = str.ToString(),
				Length = tl.Position + tl.Length - hd.Position
			};
		}
	}
}
