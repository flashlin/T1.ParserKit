using System.Collections.Generic;

namespace T1.ParserKit.Core
{
	public class CacheStringReader : IInputReader
	{
		private readonly IInputReader _reader;

		public CacheStringReader(IInputReader reader)
		{
			_reader = reader;
		}

		public IInputReader AdvanceBy(int len)
		{
			return _reader.AdvanceBy(len);
		}

		public TextSpan Consume(int len)
		{
			return _reader.Consume(len);
		}

		public bool Eof()
		{
			return _reader.Eof();
		}

		public string GetFile()
		{
			return _reader.GetFile();
		}

		public int GetPosition()
		{
			return _reader.GetPosition();
		}

		public string Substr(int len)
		{
			return _reader.Substr(len);
		}

		public void Seek(int offset)
		{
			_reader.Seek(offset);
		}

		private readonly Dictionary<int, LinePosition> _cache = new Dictionary<int, LinePosition>();

		public LinePosition GetLinePosition(int offset)
		{
			if (_cache.TryGetValue(offset, out var pos))
			{
				return pos;
			}

			pos = _reader.GetLinePosition(offset);
			_cache[offset] = pos;
			return pos;
		}
	}
}