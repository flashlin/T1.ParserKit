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
			Text = string.Empty,
			Position = -1,
			Length = 0
		};

		public string File { get; set; }
		public string Text { get; set; }
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
				text = Text.Substring(Position, Length);
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
				Text = string.Join("", fromArr.Select(x => x.Text)),
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
					Text = b.Text,
					Position = b.Position,
					Length = b.Length
				};
			}

			if (b.Equals(TextSpan.Empty))
			{
				return new TextSpan()
				{
					File = a.File,
					Text = a.Text,
					Position = a.Position,
					Length = a.Length
				};
			}

			return new TextSpan()
			{
				File = a.File,
				Text = a.Text + b.Text,
				Position = a.Position,
				Length = a.Length + b.Length
			};
		}
	}
}