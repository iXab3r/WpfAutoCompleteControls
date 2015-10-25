namespace WpfAutoCompleteControls.Editors
{
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    [TemplatePart(Name = PartEditor, Type = typeof (TextBox))]
    [TemplatePart(Name = PartPopup, Type = typeof (Popup))]
    [TemplatePart(Name = PartSelector, Type = typeof (Selector))]
    public partial class AutoCompleteTextBox : Control
    {
        public const string PartEditor = "PART_Editor";
        public const string PartPopup = "PART_Popup";
        public const string PartSelector = "PART_Selector";

        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
            "ItemTemplate",
            typeof (DataTemplate),
            typeof (AutoCompleteTextBox),
            new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty ItemTemplateSelectorProperty = DependencyProperty.Register(
            "ItemTemplateSelector",
            typeof (DataTemplateSelector),
            typeof (AutoCompleteTextBox),
            new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty ProviderProperty = DependencyProperty.Register(
            "Provider",
            typeof (ISuggestionProvider),
            typeof (AutoCompleteTextBox),
            new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof (string),
            typeof (AutoCompleteTextBox),
            new FrameworkPropertyMetadata(string.Empty));

        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register(
            "Watermark",
            typeof (string),
            typeof (AutoCompleteTextBox),
            new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty DisplayMemberPathProperty = DependencyProperty.Register(
            "DisplayMemberPath",
            typeof (string),
            typeof (AutoCompleteTextBox),
            new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty DelayProperty = DependencyProperty.Register(
            "Delay",
            typeof(int),
            typeof(AutoCompleteTextBox),
            new FrameworkPropertyMetadata(200));

        public static readonly DependencyProperty MinimumQueryLengthProperty = DependencyProperty.Register(
            "MinimumQueryLength",
            typeof(int),
            typeof(AutoCompleteTextBox),
            new FrameworkPropertyMetadata(1));

        internal static readonly DependencyProperty SuggestionsListProperty = DependencyProperty.Register(
            "SuggestionsList",
            typeof (ObservableCollection<object>),
            typeof (AutoCompleteTextBox),
            new FrameworkPropertyMetadata(null));

        internal static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register(
            "IsDropDownOpen",
            typeof(bool),
            typeof(AutoCompleteTextBox),
            new FrameworkPropertyMetadata(false));

        internal static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register(
            "IsLoading",
            typeof(bool),
            typeof(AutoCompleteTextBox),
            new FrameworkPropertyMetadata(false));

        static AutoCompleteTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (AutoCompleteTextBox), new FrameworkPropertyMetadata(typeof (AutoCompleteTextBox)));
        }

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate) GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public DataTemplateSelector ItemTemplateSelector
        {
            get { return (DataTemplateSelector) GetValue(ItemTemplateSelectorProperty); }
            set { SetValue(ItemTemplateSelectorProperty, value); }
        }

        public string Watermark
        {
            get { return (string) GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        public int MinimumQueryLength
        {
            get { return (int) GetValue(MinimumQueryLengthProperty); }
            set { SetValue(MinimumQueryLengthProperty, value); }
        }

        public int Delay
        {
            get { return (int) GetValue(DelayProperty); }
            set { SetValue(DelayProperty, value); }
        }

        public ISuggestionProvider Provider
        {
            get { return (ISuggestionProvider) GetValue(ProviderProperty); }
            set { SetValue(ProviderProperty, value); }
        }

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public string DisplayMemberPath
        {
            get { return (string) GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }

        internal ObservableCollection<object> SuggestionsList
        {
            get { return (ObservableCollection<object>) GetValue(SuggestionsListProperty); }
            set { SetValue(SuggestionsListProperty, value); }
        }

        internal bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        internal bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            private set { SetValue(IsLoadingProperty, value); }
        }
    }
}