// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections;
using System.Linq;
using Avalonia;
using Avalonia.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Stride.Core.Annotations;
using Stride.Core.Extensions;
using Stride.Core.Presentation.Core;
using Stride.Core.Presentation.Extensions;

using Avalonia.Interactivity;
using Avalonia.Controls.Primitives;
using System;
using Avalonia.Data;
using Avalonia.Controls.Metadata;
using System.Collections.Generic;
using System.Collections.Specialized;

// FIXME: not doing any filtering at the moment, presumably because no collectionview in avalonia.
// Not reliably hiding dropdown, because events to listbox are lost if the dropdown is hidden too early (e.g. by lost focus in the text box).
namespace Stride.Core.Presentation.Controls
{
    [TemplatePart(Name = "PART_EditableTextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_ListBox", Type = typeof(ListBox))]
    public class FilteringComboBox : SelectingItemsControl
    {
        /// <summary>
        /// A dependency property used to safely evaluate the value of an item given a path.
        /// </summary>
        private static readonly StyledProperty<object> InternalValuePathProperty = StyledProperty<object>.Register<FilteringComboBox, object>("InternalValuePath"); // T1A
        /// <summary>
        /// The input text box.
        /// </summary>
        private TextBox editableTextBox;
        /// <summary>
        /// The filtered list box.
        /// </summary>
        private ListBox listBox;
        /// <summary>
        /// Indicates that the selection is being internally cleared and that the drop down should not be opened nor refreshed.
        /// </summary>
        private bool clearing;
        /// <summary>
        /// Indicates that the user clicked in the listbox with the mouse and that the drop down should not be opened.
        /// </summary>
        private bool listBoxClicking;
        /// <summary>
        /// Indicates that the selection is being internally updated and that the text should not be cleared.
        /// </summary>
        private bool updatingSelection;
        /// <summary>
        /// Indicates that the text box is being validated and that the update of the text should not impact the selected item.
        /// </summary>
        private bool validating;

        /// <summary>
        /// Identifies the <see cref="RequireSelectedItemToValidate"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> RequireSelectedItemToValidateProperty = StyledProperty<bool>.Register<FilteringComboBox, bool>("RequireSelectedItemToValidate"); // T1

        /// <summary>
        /// Identifies the <see cref="Text"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<string> TextProperty = StyledProperty<string>.Register<FilteringComboBox, string>("Text"); // T6E

        /// <summary>
        /// Identifies the <see cref="IsDropDownOpen"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> IsDropDownOpenProperty = StyledProperty<bool>.Register<FilteringComboBox, bool>("IsDropDownOpen", false); // T8

        /// <summary>
        /// Identifies the <see cref="OpenDropDownOnFocus"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> OpenDropDownOnFocusProperty = StyledProperty<bool>.Register<FilteringComboBox, bool>("OpenDropDownOnFocus"); // T1

        /// <summary>
        /// Identifies the <see cref="ClearTextAfterValidation"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> ClearTextAfterValidationProperty = StyledProperty<bool>.Register<FilteringComboBox, bool>("ClearTextAfterValidation"); // T1

        /// <summary>
        /// Identifies the <see cref="WatermarkContent"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<object> WatermarkContentProperty = StyledProperty<object>.Register<FilteringComboBox, object>("WatermarkContent", null); // T2

        /// <summary>
        /// Identifies the <see cref="IsFiltering"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> IsFilteringProperty = StyledProperty<bool>.Register<FilteringComboBox, bool>("IsFiltering", true); // T8

        /// <summary>
        /// Identifies the <see cref="ItemsToExclude"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<IEnumerable> ItemsToExcludeProperty = StyledProperty<IEnumerable>.Register<FilteringComboBox, IEnumerable>("ItemsToExclude"); // T1

        /// <summary>
        /// Identifies the <see cref="Sort"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<FilteringComboBoxSort> SortProperty = StyledProperty<FilteringComboBoxSort>.Register<FilteringComboBox, FilteringComboBoxSort>("Sort"); // T6F

        /// <summary>
        /// Identifies the <see cref="SortMemberPath"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<string> SortMemberPathProperty = StyledProperty<string>.Register<FilteringComboBox, string>("SortMemberPath"); // T1

        /// <summary>
        /// Identifies the <see cref="ValidatedValue"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<object> ValidatedValueProperty = StyledProperty<object>.Register<FilteringComboBox, object>("ValidatedValue"); // T1

        /// <summary>
        /// Identifies the <see cref="ValidatedItem"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<object> ValidatedItemProperty = StyledProperty<object>.Register<FilteringComboBox, object>("ValidatedItem"); // T1

        /// <summary>
        /// Identifies the <see cref="ValidateOnLostFocus"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> ValidateOnLostFocusProperty = StyledProperty<bool>.Register<FilteringComboBox, bool>(nameof(ValidateOnLostFocus), true); // T2


        /// <summary>
        /// Raised just before the TextBox changes are validated. This event is cancellable
        /// </summary>
        public static readonly RoutedEvent ValidatingEvent = RoutedEvent.Register<FilteringComboBox, CancelRoutedEventArgs>("Validating", RoutingStrategies.Bubble);

        /// <summary>
        /// Raised when TextBox changes have been validated.
        /// </summary>
        public static readonly RoutedEvent ValidatedEvent = RoutedEvent.Register<FilteringComboBox, ValidationRoutedEventArgs<string>>("Validated", RoutingStrategies.Bubble);

        static FilteringComboBox()
		{
			SortProperty.Changed.AddClassHandler<FilteringComboBox>(OnItemsSourceRefresh);
			IsDropDownOpenProperty.Changed.AddClassHandler<FilteringComboBox>(OnIsDropDownOpenChanged);
			IsFilteringProperty.Changed.AddClassHandler<FilteringComboBox>(OnIsFilteringChanged);

            
        }

        [ModuleInitializer]
        internal static void Initialize ()
        {
            var v = ValidatedEvent; // Force the event to be registered as soon as possible.
        }
        
        public FilteringComboBox()
        {
            IsTextSearchEnabled = false;
        }

        /// <summary>
        /// Gets or sets whether the drop down is open.
        /// </summary>
        public bool IsDropDownOpen { get { return (bool)GetValue(IsDropDownOpenProperty); } set { SetValue(IsDropDownOpenProperty, value); } }

        /// <summary>
        /// Gets or sets whether to open the dropdown when the control got the focus.
        /// </summary>
        public bool OpenDropDownOnFocus { get { return (bool)GetValue(OpenDropDownOnFocusProperty); } set { SetValue(OpenDropDownOnFocusProperty, value); } }

        /// <summary>
        /// Gets or sets whether the validation will be cancelled if <see cref="Selector.SelectedItem"/> is null.
        /// </summary>
        public bool RequireSelectedItemToValidate { get { return (bool)GetValue(RequireSelectedItemToValidateProperty); } set { SetValue(RequireSelectedItemToValidateProperty, value); } }

        /// <summary>
        /// Gets or sets the text of this <see cref="FilteringComboBox"/>
        /// </summary>
        public string Text { get { return (string)GetValue(TextProperty); } set { SetValue(TextProperty, value); } }

        /// <summary>
        /// Gets or sets whether to clear the text after the validation.
        /// </summary>
        public bool ClearTextAfterValidation { get { return (bool)GetValue(ClearTextAfterValidationProperty); } set { SetValue(ClearTextAfterValidationProperty, value); } }

        /// <summary>
        /// Gets or sets the content to display when the TextBox is empty.
        /// </summary>
        public object WatermarkContent { get { return GetValue(WatermarkContentProperty); } set { SetValue(WatermarkContentProperty, value); } }

        /// <summary>
        /// Gets or sets the content to display when the TextBox is empty.
        /// </summary>
        public bool IsFiltering { get { return (bool)GetValue(IsFilteringProperty); } set { SetValue(IsFilteringProperty, value); } }

        [Obsolete]
        public IEnumerable ItemsToExclude { get { return (IEnumerable)GetValue(ItemsToExcludeProperty); } set { SetValue(ItemsToExcludeProperty, value); } }

        /// <summary>
        /// Gets or sets the comparer used to sort items.
        /// </summary>
        public FilteringComboBoxSort Sort { get { return (FilteringComboBoxSort)GetValue(SortProperty); } set { SetValue(SortProperty, value); } }

        /// <summary>
        /// Gets or sets the name of the member to use to sort items.
        /// </summary>
        public string SortMemberPath { get { return (string)GetValue(SortMemberPathProperty); } set { SetValue(SortMemberPathProperty, value); } }

        public object ValidatedValue { get { return GetValue(ValidatedValueProperty); } set { SetValue(ValidatedValueProperty, value); } }

        public object ValidatedItem { get { return GetValue(ValidatedItemProperty); } set { SetValue(ValidatedItemProperty, value); } }

        /// <summary>
        /// Gets or sets whether the validation should happen when the control losts focus.
        /// </summary>
        public bool ValidateOnLostFocus { get { return (bool)GetValue(ValidateOnLostFocusProperty); } set { SetValue(ValidateOnLostFocusProperty, value); } }

        /// <summary>
        /// Raised just before the TextBox changes are validated. This event is cancellable
        /// </summary>
        public event EventHandler<CancelRoutedEventArgs> Validating { add { AddHandler(ValidatingEvent, value); } remove { RemoveHandler(ValidatingEvent, value); } }

        /// <summary>
        /// Raised when TextBox changes have been validated.
        /// </summary>
        public event EventHandler<ValidationRoutedEventHandler<string>> Validated { add { AddHandler(ValidatedEvent, value); } remove { RemoveHandler(ValidatedEvent, value); } }

        private void OnItemsViewSourceChanged(IEnumerable oldValue, IEnumerable newValue)  // Removed override, since marked as private in SelectingItemsControl.
        {
            //base.OnItemsViewSourceChanged(items, e);

            if (newValue != null)
            {
                UpdateCollectionView();
            }
        }

        private static void OnIsDropDownOpenChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var filteringComboBox = (FilteringComboBox)d;
            if ((bool)e.NewValue && filteringComboBox.ItemsSource != null)
            {
                filteringComboBox.UpdateCollectionView();
            }
        }

        private static void OnIsFilteringChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var filteringComboBox = (FilteringComboBox)d;
            if (filteringComboBox.ItemsSource != null)
            {
                filteringComboBox.UpdateCollectionView();
            }
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
		{
			base.OnApplyTemplate(e);

            editableTextBox = e.NameScope.Find<TextBox>("PART_EditableTextBox");
            if (editableTextBox == null)
                throw new InvalidOperationException("A part named 'PART_EditableTextBox' must be present in the ControlTemplate, and must be of type 'Stride.Core.Presentation.Controls.Input.TextBox'.");

            listBox = e.NameScope.Find<ListBox>("PART_ListBox");
            if (listBox == null)
                throw new InvalidOperationException("A part named 'PART_ListBox' must be present in the ControlTemplate, and must be of type 'ListBox'.");

            editableTextBox.TextChanged += EditableTextBoxTextChanged;
//             editableTextBox.PreviewKeyDown += EditableTextBoxPreviewKeyDown;
            editableTextBox.Validating += EditableTextBoxValidating;
            editableTextBox.Validated += EditableTextBoxValidated;
            editableTextBox.Cancelled += EditableTextBoxCancelled;
            editableTextBox.LostFocus += EditableTextBoxLostFocus;
            listBox.PointerReleased += ListBoxMouseUp;
        }

        protected override void OnGotFocus(GotFocusEventArgs e)
		{
			base.OnGotFocus(e);
            if (OpenDropDownOnFocus && !listBoxClicking)
            {
//                 IsDropDownOpen = true;
            }
            listBoxClicking = false;
        }

        private static void OnItemsSourceRefresh(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var filteringComboBox = (FilteringComboBox)d;
            filteringComboBox.OnItemsViewSourceChanged(filteringComboBox.ItemsSource, filteringComboBox.ItemsSource);
        }

        private void EditableTextBoxValidating(object sender, CancelRoutedEventArgs e)
        {
            // This may happens somehow when the template is refreshed.
            if (!ReferenceEquals(sender, editableTextBox))
                return;

            // If we require a selected item but there is none, cancel the validation
//             BindingExpression expression;
//             if (RequireSelectedItemToValidate && SelectedItem == null)
//             {
//                 e.Cancel = true;
//                 expression = GetBindingExpression(TextProperty);
//                 expression?.UpdateTarget();
//                 editableTextBox.Cancel();
//                 return;
//             }

            validating = true;

            // Update the validated properties
            ValidatedValue = SelectedValue;
            ValidatedItem = SelectedItem;

            // If the dropdown is still open and something is selected, use the string from the selected item
            if (SelectedItem != null && IsDropDownOpen)
            {
                var displayValue = ResolveSortMemberValue(SelectedItem);
                editableTextBox.Text = displayValue?.ToString();
                if (editableTextBox.Text != null)
                {
                    editableTextBox.CaretIndex = editableTextBox.Text.Length;
                    if (IsDropDownOpen)
                    {
                        IsDropDownOpen = false;
                    }
                }
            }

            // Update the source of the text property binding
//             expression = GetBindingExpression(TextProperty);
//             expression?.UpdateSource();
            Text = editableTextBox.Text;

            // Close the dropdown
//             if (IsDropDownOpen)
//             {
//                 IsDropDownOpen = false;
//             }

            validating = false;

            var cancelRoutedEventArgs = new CancelRoutedEventArgs(ValidatingEvent);
            RaiseEvent(cancelRoutedEventArgs);
            if (cancelRoutedEventArgs.Cancel)
            {
                e.Cancel = true;
            }
        }

        private void EditableTextBoxValidated(object sender, ValidationRoutedEventArgs<string> e)
        {
            // This may happens somehow when the template is refreshed.
            if (!ReferenceEquals(sender, editableTextBox))
                return;

            var validatedArgs = new RoutedEventArgs(ValidatedEvent);
            RaiseEvent(validatedArgs);

            if (ClearTextAfterValidation)
            {
                clearing = true;
                editableTextBox.Text = string.Empty;
                clearing = false;
            }
        }

        private void EditableTextBoxCancelled(object sender, RoutedEventArgs e)
        {
            // This may happens somehow when the template is refreshed.
            if (!ReferenceEquals(sender, editableTextBox))
                return;

//             var expression = GetBindingExpression(TextProperty);
//             expression?.UpdateTarget();

            clearing = true;
            IsDropDownOpen = false;
            clearing = false;
        }

        private void EditableTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            // This may happens somehow when the template is refreshed.
            if (!ReferenceEquals(sender, editableTextBox))
                return;
            
            clearing = true;
            if (!RequireSelectedItemToValidate)
            {
                updatingSelection = true;
                SelectedItem = null;
                updatingSelection = false;
            }
            if (ValidateOnLostFocus)
            {
                editableTextBox.Validate();
            }
            // Make sure the drop down is closed
//             IsDropDownOpen = false;
            clearing = false;
        }

        private void EditableTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (ItemsSource == null)
                return;

            updatingSelection = true;
            if (!IsDropDownOpen && !clearing && IsKeyboardFocusWithin)
            {
                // Setting IsDropDownOpen to true will select all the text. We don't want this behavior, so let's save and restore the caret index.
                var index = editableTextBox.CaretIndex;
                IsDropDownOpen = true;
                editableTextBox.CaretIndex = index;
            }
            if (Sort != null)
                Sort.Token = editableTextBox.Text;

            // TODO: this will update the selected index because the collection view is shared. If UpdateSelectionOnValidation is true, this will still modify the SelectedIndex
            UpdateCollectionView();

//             var collectionView = CollectionViewSource.GetDefaultView(ItemsSource);
//             var listCollectionView = collectionView as ListCollectionView;
// 
//             collectionView.Refresh();
//             if (!validating)
//             {
//                 if (listCollectionView?.Count > 0 || collectionView.Cast<object>().Any())
//                 {
//                     listBox.SelectedIndex = 0;
//                 }
//             }
            updatingSelection = false;
        }

        private void UpdateCollectionView()
        {
//             var collectionView = CollectionViewSource.GetDefaultView(ItemsSource);
//             collectionView.Filter = IsFiltering ? (Predicate<object>)InternalFilter : null;
//             var listCollectionView = collectionView as ListCollectionView;
//             if (listCollectionView != null)
//             {
//                 listCollectionView.CustomSort = Sort;
//             }
        }

//         private void EditableTextBoxPreviewKeyDown(object sender, KeyEventArgs e)
        protected override void OnKeyDown(KeyEventArgs e)
        {
            updatingSelection = true;

            if (e.Key == Key.Escape)
            {
                if (IsDropDownOpen)
                {
                    IsDropDownOpen = false;
                    if (RequireSelectedItemToValidate)
                        editableTextBox.Cancel();
                }
                else
                {
                    editableTextBox.Cancel();
                }
            }

            if (listBox.Items.Count <= 0)
            {
                updatingSelection = false;
                return;
            }

            var stackPanel = Avalonia.VisualTree.VisualExtensions.FindDescendantOfType<VirtualizingStackPanel>(listBox);
            switch (e.Key)
            {
                case Key.Escape:
                    if (IsDropDownOpen)
                    {
                        IsDropDownOpen = false;
                        if (RequireSelectedItemToValidate)
                            editableTextBox.Cancel();
                    }
                    else
                    {
                        editableTextBox.Cancel();
                    }
                    break;

                case Key.Up:
                    listBox.SelectedIndex = Math.Max(listBox.SelectedIndex - 1, 0);
                    BringSelectedItemIntoView();
                    break;

                case Key.Down:
                    listBox.SelectedIndex = Math.Min(listBox.SelectedIndex + 1, listBox.Items.Count - 1);
                    BringSelectedItemIntoView();
                    break;

                case Key.PageUp:
                    if (stackPanel != null)
                    {
                        var count = stackPanel.Children.Count;
                        listBox.SelectedIndex = Math.Max(listBox.SelectedIndex - count, 0);
                    }
                    else
                    {
                        listBox.SelectedIndex = 0;
                    }
                    BringSelectedItemIntoView();
                    break;

                case Key.PageDown:
                    if (stackPanel != null)
                    {
                        var count = stackPanel.Children.Count;
                        listBox.SelectedIndex = Math.Min(listBox.SelectedIndex + count, listBox.Items.Count - 1);
                    }
                    else
                    {
                        listBox.SelectedIndex = listBox.Items.Count - 1;
                    }
                    BringSelectedItemIntoView();
                    break;

                case Key.Home:
                    listBox.SelectedIndex = 0;
                    BringSelectedItemIntoView();
                    break;

                case Key.End:
                    listBox.SelectedIndex = listBox.Items.Count - 1;
                    BringSelectedItemIntoView();
                    break;
            }
            updatingSelection = false;
        }

        private void ListBoxMouseUp(object sender, [NotNull] PointerReleasedEventArgs e)
        {
            if (e.InitialPressMouseButton == MouseButton.Left && listBox.SelectedIndex > -1)
            {
                // We need to force the validation here
                // The user might have clicked on the list after the drop down was automatically open (see OpenDropDownOnFocus).
                editableTextBox.ForceValidate();
            }
            listBoxClicking = true;
            
//                         ValidatedValue = SelectedValue;
//             ValidatedItem = SelectedItem;
//                         var validatedArgs = new RoutedEventArgs(ValidatedEvent);
//             RaiseEvent(validatedArgs);

        }

        private void BringSelectedItemIntoView()
        {
            var selectedItem = listBox.SelectedItem;
            if (selectedItem != null)
            {
                listBox.ScrollIntoView(selectedItem);
            }
        }

        private bool InternalFilter(object obj)
        {
            var filter = editableTextBox?.Text.Trim();
            if (string.IsNullOrWhiteSpace(filter))
                return true;

            if (obj == null)
                return false;

            if (ItemsToExclude != null && ItemsToExclude.Cast<object>().Contains(obj))
                return false;

            var value = ResolveSortMemberValue(obj);
            var text = value?.ToString();
            return MatchText(filter, text);
        }

        private static bool MatchText([NotNull] string inputText, string text)
        {
            var tokens = inputText.Split(" \t\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (var token in tokens)
            {
                if (text.IndexOf(token, StringComparison.InvariantCultureIgnoreCase) < 0 && !token.MatchCamelCase(text))
                    return false;
            }
            return true;
        }

        private object ResolveSortMemberValue(object obj)
        {
            var value = obj;
            try
            {
//                 SetBinding(InternalValuePathProperty, new Binding(SortMemberPath) { Source = obj });
//                 value = GetValue(InternalValuePathProperty);
            }
            catch (Exception e)
            {
                e.Ignore();
            }
            finally
            {
//                 BindingOperations.ClearBinding(this, InternalValuePathProperty);
            }
            return value;
        }
    }
}
