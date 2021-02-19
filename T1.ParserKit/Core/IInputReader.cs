namespace T1.ParserKit.Core
{
	public interface IInputReader
	{
		bool Eof();
		IInputReader AdvanceBy(int len);
		string Substr(int len);
		ITextSpan Consume(int len);
	}
}