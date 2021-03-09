namespace T1.ParserKit.Core
{
	public struct FileContent
	{
		public static FileContent Empty = new FileContent()
		{
			Text = StringPtr.Empty,
			Offset = -1
		};

		public StringPtr Text { get; set; }
		public int Offset { get; set; }

		public string Substr(int len)
		{
			var maxLen = Text.GetLength() - Offset;
			if (len > maxLen)
			{
				len = maxLen;
			}
			return Text.Substring(Offset, len);
		}

		public LinePosition GetPosition()
		{
			var length = Text.GetLength();
			var pos = 0;
			var line = 1;
			var col = 1;
			while (pos < length && pos < Offset)
			{
				var ch = Text.Substring(pos, 1);
				pos++;
				if (ch == "\r")
				{
					col = 1;
					continue;
				}

				if (ch == "\n")
				{
					line++;
					continue;
				}

				col++;
			}

			return new LinePosition()
			{
				Line = line,
				Col = col
			};
		}
	}
}