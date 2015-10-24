namespace TestApp
{
    using System;
    using System.Collections;
    using System.Threading;

    using WpfAutoCompleteControls.Editors;

    internal sealed class BadSuggestionProvider : ISuggestionProvider
    {
        private readonly TimeSpan Delay = TimeSpan.FromSeconds(3);

        public IEnumerable GetSuggestions(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return null;
            }

            Thread.Sleep(Delay);
            
            throw new ApplicationException("Msg");
        }
    }
}