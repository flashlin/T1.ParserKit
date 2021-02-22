using System;
using System.Collections.Generic;
using System.Linq;
using T1.Standard.Extensions;

namespace T1.ParserKit.Core
{
	public struct TextSpan : ITextSpan
	{
		public static TextSpan Empty = new TextSpan()
		{
			File = string.Empty,
			Content = string.Empty,
			Position = -1,
			Length = 0
		};

		public string File { get; set; }
		public string Content { get; set; }
		public int Position { get; set; }
		public int Length { get; set; }

		public override string ToString()
		{
			if (Position == -1)
			{
				return "{EOS}";
			}

			var text = "";
			if (Length > 0)
			{
				text = Content.Substring(Position, Length);
			}
			return $"Pos:{Position} '{text}'";
		}

		public static TextSpan From(IEnumerable<ITextSpan> from)
		{
			var fromArr = from.CastArray();
			if (fromArr.Length == 0)
			{
				return TextSpan.Empty;
			}
			var hd = fromArr.First();
			var tl = fromArr.Last();

			return new TextSpan
			{
				File = hd.File,
				Content = string.Join("", fromArr.Select(x => x.Content)),
				Position = hd.Position,
				Length = tl.Position + tl.Length - hd.Position
			};
		}

		public static TextSpan operator +(TextSpan a, TextSpan b)
		{
			if (a.Equals(TextSpan.Empty))
			{
				return new TextSpan()
				{
					File = b.File,
					Content = b.Content,
					Position = b.Position,
					Length = b.Length
				};
			}

			if (b.Equals(TextSpan.Empty))
			{
				return new TextSpan()
				{
					File = a.File,
					Content = a.Content,
					Position = a.Position,
					Length = a.Length
				};
			}

			return new TextSpan()
			{
				File = a.File,
				Content = a.Content + b.Content,
				Position = a.Position,
				Length = a.Length + b.Length
			};
		}
	}
}