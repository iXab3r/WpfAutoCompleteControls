namespace WpfAutoCompleteControls.Editors
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Threading;

    public partial class AutoCompleteTextBox 
    {
        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            internal set { SetValue(IsDropDownOpenProperty, value); }
        }

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            private set { SetValue(IsLoadingProperty, value); }
        }

        public ISuggestionProvider Provider
        {
            get { return (ISuggestionProvider)GetValue(ProviderProperty); }
            set { SetValue(ProviderProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        internal ObservableCollection<object> SuggestionsList {
            get { return (ObservableCollection<object>) GetValue(SuggestionsListProperty); }
            set { SetValue(SuggestionsListProperty, value); }
        } 

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var editor = Template.FindName(PartEditor, this) as TextBox;
            var popup = Template.FindName(PartPopup, this) as Popup;

            if (editor == null)
            {
                throw new ApplicationException($"Could not find editor({typeof(TextBox)}), name '{PartEditor}'");
            }

            if (popup == null)
            {
                throw new ApplicationException($"Could not find popup({typeof(Popup)}), name '{PartPopup}'");
            }

            var provider = Provider;
            if (provider == null)
            {
                return;
            }

            var textChanged = Observable
                .FromEventPattern<TextChangedEventHandler, TextChangedEventArgs>(x => editor.TextChanged += x, x => editor.TextChanged -= x)
                .Select(x => ((TextBox)x.Sender).Text)
                .DistinctUntilChanged()
                .Publish();

            textChanged
                .Subscribe(text => IsDropDownOpen = text?.Length > 0 );

            textChanged
                .Do(_ => IsLoading = true)
                .Select(x => Observable.Start(() => provider.GetSuggestions(x), TaskPoolScheduler.Default))
                .Switch()
                .Take(1)
                .ObserveOn(DispatcherScheduler.Current)
                .Finally(() => IsLoading = false)
                .Subscribe(HandleNextPackOfResultsFromSuggestionProvider, HandleErrorFromSuggestionProvider);

            textChanged.Connect();
        }

        private void HandleNextPackOfResultsFromSuggestionProvider(IEnumerable suggestions)
        {
            var typedSuggestions = suggestions.OfType<object>();
            SuggestionsList = new ObservableCollection<object>(typedSuggestions);
        }

        private void HandleErrorFromSuggestionProvider(Exception exception)
        {
            SuggestionsList = null;
        }
    }
}