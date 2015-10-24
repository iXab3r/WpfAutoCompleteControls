namespace WpfAutoCompleteControls.Behaviors
{
    using System.Windows.Controls;
    using System.Windows.Interactivity;

    internal sealed class KeepSelectionInViewBehavior : Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.SelectionChanged += AssociatedObjectOnSelectionChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.SelectionChanged -= AssociatedObjectOnSelectionChanged;

            base.OnDetaching();
        }

        private void AssociatedObjectOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            var selectedItem = AssociatedObject.SelectedItem;

            if (selectedItem == null)
            {
                return;
            }

            AssociatedObject.ScrollIntoView(selectedItem);
        }
    }
}