namespace T1.ParserKit.Core
{
	public interface IInputReader
	{
		IInputReader AdvanceBy(int len);

		TextSpan Consume(int len);

		bool Eof();
		string GetFile();

		int GetPosition();

		string Substr(int len);
		void Seek(int offset);
		string GetContent();
		TextOffset GetTextOffset();
	}
}