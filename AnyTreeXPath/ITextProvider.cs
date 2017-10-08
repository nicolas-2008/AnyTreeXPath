namespace AnyTreeXPath
{
    /// <summary>
    /// Indicates that object supports querying 'text()'
    /// </summary>
    public interface ITextProvider
    {
        string GetText();
    }
}
