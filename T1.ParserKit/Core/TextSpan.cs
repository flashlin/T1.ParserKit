using System;

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
			var text = "";
			if (Length > 0)
			{
				text = Content.Substring(Position, Length);
			}
			return $"Pos:{Position} '{text}'";
		}
	}
}