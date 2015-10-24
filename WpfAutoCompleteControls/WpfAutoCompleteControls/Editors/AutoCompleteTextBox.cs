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
    using System.Windows.Input;
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

        internal ObservableCollection<object> SuggestionsList
        {
            get { return (ObservableCollection<object>)GetValue(SuggestionsListProperty); }
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
                .Select(x => (TextBox)x.Sender)
                .Select(x => x.Text)
                .DistinctUntilChanged()
                .ToSuspendable();

            var textChanged = suspendableTextChanged.Publish();
            
            var suspendableCurrentItemChanged = Observable
                .FromEventPattern<EventHandler, EventArgs>(x => itemsSelector.Items.CurrentChanged += x, x => itemsSelector.Items.CurrentChanged -= x)
                .Select(x => itemsSelector.Items.CurrentItem)
                .DistinctUntilChanged()
                .ToSuspendable();

            var currentItemChanged = suspendableCurrentItemChanged.Publish();

            textChanged
                .Subscribe(text => IsDropDownOpen = text?.Length > 0);

            textChanged
                .Do(_ => IsLoading = true)
                .Do(x => Trace.WriteLine($"Resolving '{x}'..."))
                .Select(x => Observable.Start(() => provider.GetSuggestions(x), TaskPoolScheduler.Default).Catch<IEnumerable, Exception>(HandleSuggestionProviderException))
                .Switch()
                .ObserveOn(DispatcherScheduler.Current)
                .Do(_ => IsLoading = false)
                .Subscribe(HandleNextPackOfResultsFromSuggestionProvider, suspendableCurrentItemChanged);

            currentItemChanged
                .Where(x => x != null)
                .Select(x => bindingEvaluator.Evaluate(x)?.ToString())
                .Subscribe(newValue => 
                {
                    editor.Text = newValue; 
                    editor.CaretIndex = editor.Text?.Length ?? 0;
                }, suspendableTextChanged);

            Observable.Merge(
                    Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(x => editor.PreviewKeyDown += x, x => editor.PreviewKeyDown -= x),
                    Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(x => itemsSelector.PreviewKeyDown += x, x => itemsSelector.PreviewKeyDown -= x))
                .Subscribe((x) => ItemsSelectorOnPreviewKeyDown(itemsSelector, x.EventArgs));

            Observable.Merge(
                    Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(x => editor.PreviewKeyDown += x, x => editor.PreviewKeyDown -= x),
                    Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(x => itemsSelector.PreviewKeyDown += x, x => itemsSelector.PreviewKeyDown -= x))
                .Subscribe((x) => ItemsSelectorOnPreviewKeyDownHandleCommit(itemsSelector, x.EventArgs));

            textChanged.Connect();
            currentItemChanged.Connect();
            currentItemChanged.Connect();
        }

        private void ItemsSelectorOnPreviewKeyDown(Selector itemsSelector, KeyEventArgs keyEventArgs)
        {
            if (!IsDropDownOpen)
            {
                if (keyEventArgs.Key == Key.Down || keyEventArgs.Key == Key.Up)
                {
                    IsDropDownOpen = true;
                }
            }
            else
            {
                if (keyEventArgs.Key == Key.Down)
                {
                    IncrementSelection(itemsSelector);
                }
                else if (keyEventArgs.Key == Key.Up)
                {
                    DecrementSelection(itemsSelector);
                }
                else if (keyEventArgs.Key == Key.Escape)
                {
                    IsDropDownOpen = false;
                }
            }
        }

        private void ItemsSelectorOnPreviewKeyDownHandleCommit(Selector itemsSelector, KeyEventArgs keyEventArgs)
        {
            if (!IsDropDownOpen)
            {
                return;
            }
            if (keyEventArgs.Key == Key.Tab || keyEventArgs.Key == Key.Enter)
            {
                IsDropDownOpen = false;
                itemsSelector.Items.MoveCurrentTo(null);
                itemsSelector.Items.MoveCurrentTo(itemsSelector.SelectedItem);
            }
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

        private void IncrementSelection(Selector itemsSelector)
        {
            if (itemsSelector.Items == null || !itemsSelector.HasItems || itemsSelector.Items.Count < itemsSelector.SelectedIndex)
            {
                return;
            }
            itemsSelector.SelectedIndex++;
        }

        private void DecrementSelection(Selector itemsSelector)
        {
            if (itemsSelector.Items == null || !itemsSelector.HasItems || itemsSelector.SelectedIndex <= 0)
            {
                return;
            }
            itemsSelector.SelectedIndex--;
        }
    }
}