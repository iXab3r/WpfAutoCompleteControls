namespace WpfAutoCompleteControls.Editors
{
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    [TemplatePart(Name = PartEditor, Type = typeof(TextBox))]
    [TemplatePart(Name = PartPopup, Type = typeof(Popup))]
    public partial class AutoCompleteTextBox : Control
    {
        public const string PartEditor = "PART_Editor";
        public const string PartPopup = "PART_Popup";

        public const string PartSelector = "PART_Selector";

        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register(
            "IsDropDownOpen",
            typeof(bool),
            typeof(AutoCompleteTextBox),
            new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register(
           "IsLoading",
           typeof(bool),
           typeof(AutoCompleteTextBox),
           new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
            "ItemTemplate",
            typeof(DataTemplate),
            typeof(AutoCompleteTextBox),
            new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty ItemTemplateSelectorProperty =
            DependencyProperty.Register(
                "ItemTemplateSelector",
                typeof(DataTemplateSelector),
                typeof(AutoCompleteTextBox));

        public static readonly DependencyProperty ProviderProperty = DependencyProperty.Register(
            "Provider",
            typeof(ISuggestionProvider),
            typeof(AutoCompleteTextBox),
            new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string),
            typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(string.Empty));

        public static readonly DependencyProperty SuggestionsListProperty = DependencyProperty.Register("SuggestionsList", typeof(ObservableCollection<object>),
            typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty SuggestionItemStyleProperty = DependencyProperty.Register("SuggestionItemStyle", typeof(Style),
           typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty SuggestionItemStyleSelectorProperty = DependencyProperty.Register("SuggestionItemStyleSelector", typeof(StyleSelector),
           typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(null));
        
        static AutoCompleteTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(typeof(AutoCompleteTextBox)));
        }

        public Style SuggestionItemStyle
        {
            get { return (Style) GetValue(SuggestionItemStyleProperty); }
            set { SetValue(SuggestionItemStyleProperty, value); }
        }

        public StyleSelector SuggestionItemStyleSelector
        {
            get { return (StyleSelector)GetValue(SuggestionItemStyleSelectorProperty); }
            set { SetValue(SuggestionItemStyleSelectorProperty, value); }
        }
    }
}