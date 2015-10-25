namespace WpfAutoCompleteControls.Editors
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;

    using RxUtils;

    public partial class AutoCompleteTextBox
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var editor = Template.FindName(PartEditor, this) as TextBox;
            var popup = Template.FindName(PartPopup, this) as Popup;
            var itemsSelector = Template.FindName(PartSelector, this) as Selector;

            if (editor == null)
            {
                throw new ApplicationException($"Could not find editor({typeof (TextBox)}), name '{PartEditor}'");
            }

            if (popup == null)
            {
                throw new ApplicationException($"Could not find popup({typeof (Popup)}), name '{PartPopup}'");
            }

            if (itemsSelector == null)
            {
                throw new ApplicationException($"Could not find popup({typeof (Selector)}), name '{PartSelector}'");
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
                .Select(x => editor.Text)
                .DistinctUntilChanged()
                .ToSuspendable();
            var textChanged = suspendableTextChanged.Publish();

            var adapter = new SelectionAdapter(editor, itemsSelector);

            var suspendableSelectedItemChanged = adapter
                .SelectedItemChanged
                .Where(item => item != null)
                .Select(item => bindingEvaluator.Evaluate(item)?.ToString())
                .ToSuspendable();

            suspendableSelectedItemChanged
                .Subscribe(
                    newValue =>
                    {
                        editor.Text = newValue;
                        editor.CaretIndex = editor.Text?.Length ?? 0;
                        IsDropDownOpen = false;
                    },
                    suspendableTextChanged);

            textChanged
                .Subscribe(text => IsDropDownOpen = text?.Length >= MinimumQueryLength);

            textChanged
                .Where(filter => filter?.Length >= MinimumQueryLength)
                .Do(() => IsLoading = true)
                .Throttle(TimeSpan.FromMilliseconds(Delay))
                .Select(
                    filter => Observable
                        .Start(() => provider.GetSuggestions(filter), TaskPoolScheduler.Default)
                        .Catch<IEnumerable, Exception>(HandleSuggestionProviderException))
                .Switch()
                .ObserveOn(DispatcherScheduler.Current)
                .Do(() => IsLoading = false)
                .Subscribe(HandleNextPackOfResultsFromSuggestionProvider, suspendableSelectedItemChanged);

            textChanged.Connect();
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