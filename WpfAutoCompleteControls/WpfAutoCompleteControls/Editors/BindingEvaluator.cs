namespace WpfAutoCompleteControls.Editors
{
    using System;
    using System.Windows;
    using System.Windows.Data;

    internal sealed class BindingEvaluator : FrameworkElement
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(object),
                typeof(BindingEvaluator));

        public BindingEvaluator(Binding valueBinding)
        {
            if (valueBinding == null)
            {
                throw new ArgumentNullException(nameof(valueBinding));
            }
            this.ValueBinding = valueBinding;
        }

        public Binding ValueBinding { get; }

        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public object Evaluate(object dataItem)
        {
            this.DataContext = dataItem;
            SetBinding(ValueProperty, ValueBinding);
            return Value;
        }
    }
}