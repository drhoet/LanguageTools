namespace LanguageTools.Common
{
    /// <summary>
    /// A strategy to find the active "text" to be searched for when clicking the lookup button.
    /// </summary>
    public interface ActiveTextStrategy<T>
    {
        /// <summary>
        /// Find the active text in the given document.
        /// </summary>
        /// <param name="document">The document to search</param>
        /// <returns>The active text in the given document</returns>
        string FindActiveWord(T document);

        /// <summary>
        /// Find the active text in the active document.
        /// </summary>
        /// <returns>The active text in the active document</returns>
        string FindActiveWord();

        /// <summary>
        /// Finds the active document
        /// </summary>
        /// <returns>The active document</returns>
        T FindActiveDocument();
    }


}
