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
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Threading;

    using RxUtils;

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

        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
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
            var itemsSelector = Template.FindName(PartSelector, this) as Selector;

            if (editor == null)
            {
                throw new ApplicationException($"Could not find editor({typeof(TextBox)}), name '{PartEditor}'");
            }

            if (popup == null)
            {
                throw new ApplicationException($"Could not find popup({typeof(Popup)}), name '{PartPopup}'");
            }

            if (itemsSelector == null)
            {
                throw new ApplicationException($"Could not find popup({typeof(Selector)}), name '{PartSelector}'");
            }

            var provider = Provider;
            if (provider == null)
            {
                return;
            }

            var binding = new Binding(DisplayMemberPath);
            var bindingEvaluator = new BindingEvaluator(binding);

            var suspendableTextChanged = Observable
                .FromEventPattern<TextChangedEventHandler, TextChangedEventArgs>(x => editor.TextChanged += x, x => editor.TextChanged -= x)
                .Select(x => (TextBox) x.Sender)
                .Select(x => x.Text)
                .DistinctUntilChanged()
                .ToSuspendable();

            var textChanged = suspendableTextChanged
                .Publish();

            textChanged
                .Subscribe(text => IsDropDownOpen = text?.Length > 0 );

            textChanged
                .Do(_ => IsLoading = true)
                .Do(x => Trace.WriteLine($"Resolving '{x}'..."))
                .Select(x => Observable.Start(() => provider.GetSuggestions(x), TaskPoolScheduler.Default).Catch<IEnumerable,Exception>(HandleSuggestionProviderException))
                .Switch()
                .ObserveOn(DispatcherScheduler.Current)
                .Do(_ => IsLoading = false)
                .Subscribe(HandleNextPackOfResultsFromSuggestionProvider);

            var selectedItemChanged = Observable
                .FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>(x => itemsSelector.SelectionChanged += x, x => itemsSelector.SelectionChanged -= x)
                .Select(x => (Selector)x.Sender)
                .Select(x => x.SelectedItem)
                .DistinctUntilChanged()
                .Publish();

            selectedItemChanged
                .Where(x => x != null)
                .Select(x => bindingEvaluator.Evaluate(x)?.ToString())
                .Subscribe(
                    newValue =>
                    {
                        using (suspendableTextChanged.Suspend())
                        {
                            Text = newValue;
                        }
                    });

            textChanged.Connect();
            selectedItemChanged.Connect();
        }

        private IObservable<IEnumerable> HandleSuggestionProviderException(Exception exception)
        {
            return Observable.Return(new object[0]);
        }

        private void HandleNextPackOfResultsFromSuggestionProvider(IEnumerable suggestions)
        {
            var typedSuggestions = suggestions.OfType<object>();
            SuggestionsList = new ObservableCollection<object>(typedSuggestions);
        }   
    }
}