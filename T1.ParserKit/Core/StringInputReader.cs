namespace T1.ParserKit.Core
{
	public class StringInputReader : IInputReader
	{
		private readonly string _text;
		private readonly int _length;
		private int _position;

		public StringInputReader(string text)
		{
			_text = text;
			_length = text.Length;
			_position = 0;
		}

		public TextOffset GetTextOffset()
		{
			return new TextOffset()
			{
				Text = new StringPtr(_text),
				Offset = _position
			};
		}

		public string GetContent()
		{
			return _text;
		}

		public string GetFile()
		{
			return "";
		}

		public int GetPosition()
		{
			return _position;
		}

		public void Seek(int offset)
		{
			_position = offset;
		}

		public bool Eof()
		{
			if (_position < 0)
			{
				return true;
			}

			if (_position >= _length)
			{
				return true;
			}

			return false;
		}

		public IInputReader AdvanceBy(int len)
		{
			if (Eof())
			{
				return this;
			}

			_position += len;
			return this;
		}

		public string Substr(int len)
		{
			if (Eof()) return string.Empty;

			var restLen = _length - _position;
			var maxLen = (len > restLen) ? restLen : len;
			return _text.Substring(_position, maxLen);
		}

		public TextSpan Consume(int len)
		{
			if (Eof())
			{
				return TextSpan.Empty;
			}

			var textSpan = new TextSpan()
			{
				File = string.Empty,
				Text = Substr(len),
				Position = _position,
				Length = len
			};

			AdvanceBy(len);
			return textSpan;
		}

		public override string ToString()
		{
			var linePos = GetTextOffset().GetLinePosition();
			var ch = Substr(40);
			return $"Pos:{_position} {linePos} Rest='{ch}'";
		}
	}
}