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
			return new StringInputReader(_text)
			{
				_position = _position + len
			};
		}

		public string Substr(int len)
		{
			if (Eof()) return string.Empty;

			var restLen = _length - _position;
			var maxLen = (len > restLen) ? restLen : len;
			return _text.Substring(_position, maxLen);
		}

		public ITextSpan Consume(int len)
		{
			if (Eof())
			{
				return new TextSpan()
				{
					File = "",
					Content = "",
					Position = -1,
					Length = 0
				};
			}

			return new TextSpan()
			{
				File = "",
				Content = _text,
				Position = _position,
				Length = len
			};
		}
	}
}