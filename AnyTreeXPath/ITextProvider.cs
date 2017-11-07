namespace AnyTreeXPath
{
    /// <summary>
    /// Support querying 'text()'
    /// </summary>
    public interface ITextProvider
    {
        string GetText();
        bool HasText { get; }
    }
}
