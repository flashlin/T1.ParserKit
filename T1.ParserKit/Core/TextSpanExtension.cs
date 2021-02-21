using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using T1.Standard.Extensions;

namespace T1.ParserKit.Core
{
	public static class TextSpanExtension
	{
		public static bool Eof(this ITextSpan textSpan)
		{
			if (textSpan.Position < 0)
			{
				return true;
			}

			if (textSpan.Position >= textSpan.Content.Length)
			{
				return true;
			}

			return false;
		}


		public static ITextSpan AdvanceBy(this ITextSpan textSpan,int len)
		{
			if (textSpan.Eof())
			{
				return textSpan;
			}

			return new TextSpan()
			{
				File = textSpan.File,
				Content = textSpan.Content,
				Position = textSpan.Position + len,
				Length = textSpan.Content.Length - textSpan.Position - len
			};
		}

		public static string GetText(this ITextSpan textSpan)
		{
			return textSpan.Substr(textSpan.Length);
		}

		public static string Substr(this ITextSpan textSpan, int len)
		{
			if (textSpan.Eof()) return string.Empty;

			var maxLen = (len > textSpan.Length) ? textSpan.Length : len;
			return textSpan.Content.Substring(textSpan.Position, maxLen);
		}

		public static ITextSpan Consume(this ITextSpan textSpan, int len)
		{
			if (textSpan.Eof())
			{
				return textSpan;
			}

			return new TextSpan()
			{
				File = textSpan.File,
				Content = textSpan.Content,
				Position = textSpan.Position,
				Length = len
			};
		}

		public static T FirstCast<T>(this IEnumerable<ITextSpan> textSpans)
			where T : ITextSpan
		{
			return (T)textSpans.FirstOrDefault(x => x is T);
		}
	}
}
