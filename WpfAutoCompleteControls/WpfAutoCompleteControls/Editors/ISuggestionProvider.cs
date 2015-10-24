namespace WpfAutoCompleteControls.Editors
{
    using System.Collections;

    public interface ISuggestionProvider
    {
        IEnumerable GetSuggestions(string filter);
    }
}