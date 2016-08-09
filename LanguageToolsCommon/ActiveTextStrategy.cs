namespace LanguageTools.Common
{
    /// <summary>
    /// A strategy to find the active "text" to be searched for when clicking the lookup button.
    /// </summary>
    public interface ActiveTextStrategy<T>
    {
        string FindActiveWord(T document);
    }

}
