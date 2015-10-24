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

        public static readonly DependencyProperty ItemTemplateSelectorProperty = DependencyProperty.Register(
            "ItemTemplateSelector",
            typeof(DataTemplateSelector),
            typeof(AutoCompleteTextBox),
            new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty ProviderProperty = DependencyProperty.Register(
            "Provider",
            typeof(ISuggestionProvider),
            typeof(AutoCompleteTextBox),
            new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", 
            typeof(string),
            typeof(AutoCompleteTextBox), 
            new FrameworkPropertyMetadata(string.Empty));

        public static readonly DependencyProperty DisplayMemberPathProperty = DependencyProperty.Register(
            "DisplayMemberPathProperty", 
            typeof(string),
            typeof(AutoCompleteTextBox), 
            new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty SuggestionsListProperty = DependencyProperty.Register(
           "SuggestionsList",
           typeof(ObservableCollection<object>),
           typeof(AutoCompleteTextBox),
           new FrameworkPropertyMetadata(null));

        static AutoCompleteTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(typeof(AutoCompleteTextBox)));
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
    }
}