namespace WpfAutoCompleteControls.Editors
{
    using System;
    using System.Reactive.Linq;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    using RxUtils;

    internal sealed class SelectionAdapter
    {
        private readonly Control editor;
        private readonly Selector itemsSelector;

        public SelectionAdapter(Control editor, Selector itemsSelector)
        {
            this.editor = editor ?? throw new ArgumentNullException(nameof(editor));
            this.itemsSelector = itemsSelector ?? throw new ArgumentNullException(nameof(itemsSelector));

            var selectedItemChanged = Observable
               .FromEventPattern<SelectionChangedEventHandler, EventArgs>(x => itemsSelector.SelectionChanged += x, x => itemsSelector.SelectionChanged -= x)
               .Select(x => itemsSelector.SelectedItem)
               .DistinctUntilChanged()
               .Publish();

            Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(x => editor.PreviewKeyDown += x, x => editor.PreviewKeyDown -= x)
                .Subscribe((x) => ItemsSelectorOnPreviewKeyDown(itemsSelector, x.EventArgs));

            Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(x => editor.PreviewKeyDown += x, x => editor.PreviewKeyDown -= x)
                .Subscribe((x) => ItemsSelectorOnPreviewKeyDownHandleCommit(itemsSelector, x.EventArgs));

            Observable.FromEventPattern<MouseButtonEventHandler, MouseEventArgs>(x => itemsSelector.PreviewMouseDown += x, x => itemsSelector.PreviewMouseDown -= x)
                .Select(_ => selectedItemChanged.Take(1).Subscribe(() => CommitSelection(itemsSelector)))
                .Subscribe();

            SelectedItemChanged = Observable
              .FromEventPattern<EventHandler, EventArgs>(x => itemsSelector.Items.CurrentChanged += x, x => itemsSelector.Items.CurrentChanged -= x)
              .Select(x => itemsSelector.Items.CurrentItem)
              .DistinctUntilChanged()
              .ToSuspendable();

            selectedItemChanged.Connect();
        }

        public IObservable<object> SelectedItemChanged { get; }

        private void ItemsSelectorOnPreviewKeyDown(Selector itemsSelector, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key == Key.Down)
            {
                IncrementSelection(itemsSelector);
                keyEventArgs.Handled = true;
            }
            else if (keyEventArgs.Key == Key.Up)
            {
                DecrementSelection(itemsSelector);
                keyEventArgs.Handled = true;
            }
            else if (keyEventArgs.Key == Key.Escape)
            {
                keyEventArgs.Handled = true;
            }
        }

        private void ItemsSelectorOnPreviewKeyDownHandleCommit(Selector itemsSelector, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key == Key.Enter || keyEventArgs.Key == Key.Tab)
            {
                CommitSelection(itemsSelector);
                if (itemsSelector.IsVisible)
                {
                    keyEventArgs.Handled = true;
                }
            } 
        }

        private void CommitSelection(Selector itemsSelector)
        {
            itemsSelector.Items.MoveCurrentTo(null);
            itemsSelector.Items.MoveCurrentTo(itemsSelector.SelectedItem);
        }

        private void IncrementSelection(Selector itemsSelector)
        {
            if (!itemsSelector.HasItems || itemsSelector.Items.Count < itemsSelector.SelectedIndex)
            {
                return;
            }
            itemsSelector.SelectedIndex++;
        }

        private void DecrementSelection(Selector itemsSelector)
        {
            if (!itemsSelector.HasItems || itemsSelector.SelectedIndex <= 0)
            {
                return;
            }
            itemsSelector.SelectedIndex--;
        }
    }
}