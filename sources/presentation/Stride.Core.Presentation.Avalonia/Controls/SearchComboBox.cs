// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
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
using Stride.Core.Presentation.Core;
using Stride.Core.Presentation.Extensions;

using Avalonia.Interactivity;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using System.Linq;
using Avalonia.Controls.Metadata;
using System.Windows.Input;

namespace Stride.Core.Presentation.Controls
{
    [TemplatePart(Name = EditableTextBoxPartName, Type = typeof(TextBox))]
    [TemplatePart(Name = ListBoxPartName, Type = typeof(ListBox))]
    public class SearchComboBox : SelectingItemsControl
    {
        /// <summary>
        /// The name of the part for the <see cref="TextBox"/>.
        /// </summary>
        private const string EditableTextBoxPartName = "PART_EditableTextBox";
        /// <summary>
        /// The name of the part for the <see cref="ListBox"/>.
        /// </summary>
        private const string ListBoxPartName = "PART_ListBox";

        /// <summary>
        /// Identifies the <see cref="AlternativeCommand"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<ICommand> AlternativeCommandProperty = StyledProperty<ICommand>.Register<SearchComboBox, ICommand>("AlternativeCommand"); // T1
        /// <summary>
        /// Identifies the <see cref="AlternativeModifiers"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<KeyModifiers> AlternativeModifiersProperty = StyledProperty<KeyModifiers>.Register<SearchComboBox, KeyModifiers>("AlternativeModifiers", KeyModifiers.Shift); // T2
        /// <summary>
        /// Identifies the <see cref="ClearTextAfterSelection"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> ClearTextAfterSelectionProperty = StyledProperty<bool>.Register<SearchComboBox, bool>("ClearTextAfterSelection"); // T1
        /// <summary>
        /// Identifies the <see cref="Command"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<ICommand> CommandProperty = StyledProperty<ICommand>.Register<SearchComboBox, ICommand>("Command"); // T1
        /// <summary>
        /// Identifies the <see cref="IsAlternative"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> IsAlternativeProperty = StyledProperty<bool>.Register<SearchComboBox, bool>("IsAlternative", false); // T2
        /// <summary>
        /// Identifies the <see cref="IsDropDownOpen"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> IsDropDownOpenProperty = StyledProperty<bool>.Register<SearchComboBox, bool>("IsDropDownOpen"); // T1
        /// <summary>
        /// Identifies the <see cref="OpenDropDownOnFocus"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> OpenDropDownOnFocusProperty = StyledProperty<bool>.Register<SearchComboBox, bool>("OpenDropDownOnFocus"); // T1
        /// <summary>
        /// Identifies the <see cref="SearchText"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<string> SearchTextProperty = StyledProperty<string>.Register<SearchComboBox, string>("SearchText"); // T6E
        /// <summary>
        /// Identifies the <see cref="WatermarkContent"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<object> WatermarkContentProperty = StyledProperty<object>.Register<SearchComboBox, object>("WatermarkContent"); // T1

        /// <summary>
        /// The input text box.
        /// </summary>
        private TextBox editableTextBox;
        /// <summary>
        /// The suggestion list box.
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

        static SearchComboBox()
        {
            // FIXME  T31

             // T30
             // T30
             // T30
        }

        public SearchComboBox()
        {
            IsTextSearchEnabled = false;
        }

        /// <summary>
        /// Gets or Sets the command that is invoked once a selection has been made and <see cref="AlternativeModifiers"/> are active.
        /// The parameter of the command is the current <see cref="Selector.SelectedValue"/>.
        /// </summary>
        public ICommand AlternativeCommand { get { return (ICommand)GetValue(AlternativeCommandProperty); } set { SetValue(AlternativeCommandProperty, value); } }
        /// <summary>
        /// 
        /// </summary>
        public KeyModifiers AlternativeModifiers { get { return (KeyModifiers)GetValue(AlternativeModifiersProperty); } set { SetValue(AlternativeModifiersProperty, value); } }
        /// <summary>
        /// Gets or sets whether to clear the text after the selection.
        /// </summary>
        public bool ClearTextAfterSelection { get { return (bool)GetValue(ClearTextAfterSelectionProperty); } set { SetValue(ClearTextAfterSelectionProperty, value); } }
        /// <summary>
        /// Gets or Sets the command that is invoked once a selection has been made. The parameter of the command is the current <see cref="Selector.SelectedValue"/>.
        /// </summary>
        public ICommand Command { get { return (ICommand)GetValue(CommandProperty); } set { SetValue(CommandProperty, value); } }
        /// <summary>
        /// Gets or sets whether to open the dropdown when the control got the focus.
        /// </summary>
        public bool OpenDropDownOnFocus { get { return (bool)GetValue(OpenDropDownOnFocusProperty); } set { SetValue(OpenDropDownOnFocusProperty, value); } }
        /// <summary>
        /// 
        /// </summary>
        public bool IsAlternative { get { return (bool)GetValue(IsAlternativeProperty); } set { SetValue(IsAlternativeProperty, value); } }
        /// <summary>
        /// Gets or sets whether the drop down is open.
        /// </summary>
        public bool IsDropDownOpen { get { return (bool)GetValue(IsDropDownOpenProperty); } set { SetValue(IsDropDownOpenProperty, value); } }
        /// <summary>
        /// Gets or sets the search text of this <see cref="SearchComboBox"/>
        /// </summary>
        public string SearchText { get { return (string)GetValue(SearchTextProperty); } set { SetValue(SearchTextProperty, value); } }
        /// <summary>
        /// Gets or sets the content to display when the TextBox is empty.
        /// </summary>
        public object WatermarkContent { get { return GetValue(WatermarkContentProperty); } set { SetValue(WatermarkContentProperty, value); } }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
		{
			base.OnApplyTemplate(e);

            editableTextBox = e.NameScope.Find<TextBox>(EditableTextBoxPartName);
            if (editableTextBox == null)
                throw new InvalidOperationException($"A part named '{EditableTextBoxPartName}' must be present in the ControlTemplate, and must be of type '{typeof(TextBox).FullName}'.");

            listBox = e.NameScope.Find<ListBox>(ListBoxPartName);
            if (listBox == null)
                throw new InvalidOperationException($"A part named '{ListBoxPartName}' must be present in the ControlTemplate, and must be of type '{nameof(ListBox)}'.");
            
            editableTextBox.LostFocus += EditableTextBoxLostFocus;
            editableTextBox.KeyDown += EditableTextBoxPreviewKeyDown;
            editableTextBox.KeyUp += EditableTextBoxPreviewKeyUp;
            editableTextBox.Validated += EditableTextBoxValidated;
            listBox.PointerReleased += ListBoxMouseUp;
        }

        protected override void OnGotFocus(GotFocusEventArgs e)
		{
			base.OnGotFocus(e);
            if (OpenDropDownOnFocus && !listBoxClicking)
            {
                IsDropDownOpen = true;
            }
            listBoxClicking = false;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (IsAlternativeModifier(e.Key))
            {
                IsAlternative = true;
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (IsAlternativeModifier(e.Key))
            {
                IsAlternative = false;
            }
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

//             var el = e.NewFocus as Control;
//             if (el != null)
//             {
//                 // The user probably clicked (MouseDown) somewhere on our dropdown listbox, so we won't clear to be able to
//                 // get the MouseUp event (<see cref="ListBoxMouseUp">).
//                 if (listBox.FindVisualChildrenOfType<UIElement>().Contains(el))
//                     return;
//             }

            Clear();
        }

        private void EditableTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            // This may happens somehow when the template is refreshed.
            if (!ReferenceEquals(sender, editableTextBox))
                return;

            Clear();
        }

        private void EditableTextBoxPreviewKeyDown(object sender, [NotNull] KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Clear();
                return;
            }

            if (listBox.Items.Count <= 0)
            {
                return;
            }

            var stackPanel = global::Avalonia.VisualTree.VisualExtensions.FindDescendantOfType<VirtualizingStackPanel>(listBox);
            switch (e.Key)
            {
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
        }

        private void EditableTextBoxPreviewKeyUp(object sender, [NotNull] KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Make sure the text is validated
                editableTextBox.ForceValidate();
                // If there a selection?
                var selectedItem = listBox.SelectedItem;
                if (selectedItem == null)
                {
                    // Force selecting the first item
                    listBox.SelectedIndex = 0;
                }
                ValidateSelection();
                if (ClearTextAfterSelection)
                {
                    Clear();
                }
            }
        }

        private void EditableTextBoxValidated(object sender, RoutedEventArgs e)
        {
            if (!IsDropDownOpen && !clearing && IsKeyboardFocusWithin)
            {
                // Setting IsDropDownOpen to true will select all the text. We don't want this behavior, so let's save and restore the caret index.
                var index = editableTextBox.CaretIndex;
                IsDropDownOpen = true;
                editableTextBox.CaretIndex = index;
            }
        }

        private void ListBoxMouseUp(object sender, PointerEventArgs e)
        {
            ValidateSelection();
            if (ClearTextAfterSelection)
            {
                Clear();
            }

            listBoxClicking = true;
        }

        private void BringSelectedItemIntoView()
        {
            var selectedItem = listBox.SelectedItem;
            if (selectedItem != null)
            {
                listBox.ScrollIntoView(selectedItem);
            }
        }

        private void Clear()
        {
            clearing = true;
            editableTextBox.Text = string.Empty;
            listBox.SelectedItem = null;
            // Make sure the drop down is closed
            IsDropDownOpen = false;
            clearing = false;
        }

        private bool IsAlternativeModifier(Key key)
        {
            switch (AlternativeModifiers)
            {
                case KeyModifiers.None:
                    return false;
                case KeyModifiers.Alt:
                    return key == Key.LeftAlt || key == Key.RightAlt;
                case KeyModifiers.Control:
                    return key == Key.LeftCtrl || key == Key.RightCtrl;
                case KeyModifiers.Shift:
                    return key == Key.LeftShift || key == Key.RightShift;
                case KeyModifiers.Meta:
                    return key == Key.LWin || key == Key.RWin;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ValidateSelection()
        {
//             var expression = GetBindingExpression(SelectedIndexProperty);
//             expression?.UpdateSource();
//             expression = GetBindingExpression(SelectedItemProperty);
//             expression?.UpdateSource();
//             expression = GetBindingExpression(SelectedValueProperty);
//             expression?.UpdateSource();
            
            var commandParameter = listBox.SelectedValue;
            if (IsAlternative && AlternativeCommand.CanExecute(commandParameter))
            {
                AlternativeCommand.Execute(commandParameter);
            }
            else if (Command != null && Command.CanExecute(commandParameter))
            {
                Command.Execute(commandParameter);
            }
        }
    }
}
