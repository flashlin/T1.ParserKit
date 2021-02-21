namespace T1.ParserKit.Core
{
	public interface IParser<T>
	{
		string Name { get; set; }
		IParseResult<T> TryParse(IInputReader inp);
	}
}