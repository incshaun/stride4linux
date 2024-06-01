// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;

using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;

using Stride.Core.Presentation.Core;

using Avalonia.Markup.Xaml.Templates;
using Avalonia.Interactivity;
using System;
using Avalonia.Data;
using System.Windows.Input;
using Avalonia.Interactivity;

namespace Stride.Core.Presentation.Controls
{
    /// <summary>
    /// An implementation of the <see cref="Avalonia.Controls.TextBox"/> control
    /// that provides additional features such as a proper validation/cancellation workflow.
    /// </summary>
    public class TextBoxBase : Avalonia.Controls.TextBox
    {
        private bool validating;

        /// <summary>
        /// Identifies the <see cref="HasText"/> dependency property.
        /// </summary>
        private static readonly DirectProperty<TextBoxBase, bool> HasTextProperty = AvaloniaProperty.RegisterDirect<TextBoxBase, bool>("HasText", o => o.HasText); // T10H4

        /// <summary>
        /// Identifies the <see cref="GetFocusOnLoad"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> GetFocusOnLoadProperty = StyledProperty<bool>.Register<TextBoxBase, bool>("GetFocusOnLoad", false); // T2

        /// <summary>
        /// Identifies the <see cref="SelectAllOnFocus"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> SelectAllOnFocusProperty = StyledProperty<bool>.Register<TextBoxBase, bool>("SelectAllOnFocus", false); // T2

        /// <summary>
        /// Identifies the <see cref="WatermarkContent"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<object> WatermarkContentProperty = StyledProperty<object>.Register<TextBoxBase, object>("WatermarkContent", null); // T2

        /// <summary>
        /// Identifies the <see cref="WatermarkContentTemplate"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<DataTemplate> WatermarkContentTemplateProperty = StyledProperty<DataTemplate>.Register<TextBoxBase, DataTemplate>("WatermarkContentTemplate", null); // T2

        /// <summary>
        /// Identifies the <see cref="ValidateWithEnter"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> ValidateWithEnterProperty = StyledProperty<bool>.Register<TextBoxBase, bool>("ValidateWithEnter", true); // T2

        /// <summary>
        /// Identifies the <see cref="ValidateOnTextChange"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> ValidateOnTextChangeProperty = StyledProperty<bool>.Register<TextBoxBase, bool>("ValidateOnTextChange", false); // T2

        /// <summary>
        /// Identifies the <see cref="ValidateOnLostFocus"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> ValidateOnLostFocusProperty = StyledProperty<bool>.Register<TextBoxBase, bool>("ValidateOnLostFocus", true); // T5

        /// <summary>
        /// Identifies the <see cref="CancelWithEscape"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> CancelWithEscapeProperty = StyledProperty<bool>.Register<TextBoxBase, bool>("CancelWithEscape", true); // T2

        /// <summary>
        /// Identifies the <see cref="CancelOnLostFocus"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> CancelOnLostFocusProperty = StyledProperty<bool>.Register<TextBoxBase, bool>("CancelOnLostFocus", false); // T5

        /// <summary>
        /// Identifies the <see cref="ValidateCommand"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<ICommand> ValidateCommandProperty = StyledProperty<ICommand>.Register<TextBoxBase, ICommand>("ValidateCommand"); // T1

        /// <summary>
        /// Identifies the <see cref="ValidateCommandParameter"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<object> ValidateCommandParameterProprty = StyledProperty<object>.Register<TextBoxBase, object>("ValidateCommandParameter"); // T1

        /// <summary>
        /// Identifies the <see cref="CancelCommand"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<ICommand> CancelCommandProperty = StyledProperty<ICommand>.Register<TextBoxBase, ICommand>("CancelCommand"); // T1

        /// <summary>
        /// Identifies the <see cref="CancelCommandParameter"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<object> CancelCommandParameterProprty = StyledProperty<object>.Register<TextBoxBase, object>("CancelCommandParameter"); // T1

        /// <summary>
        /// Raised just before the TextBox changes are validated. This event is cancellable
        /// </summary>
        public static readonly RoutedEvent ValidatingEvent = RoutedEvent.Register<TextBox, RoutedEventArgs>("Validating", RoutingStrategies.Bubble); // T21

        /// <summary>
        /// Raised when TextBox changes have been validated.
        /// </summary>
        public static readonly RoutedEvent ValidatedEvent = RoutedEvent.Register<TextBox, RoutedEventArgs>("Validated", RoutingStrategies.Bubble); // T21

        /// <summary>
        /// Raised when the TextBox changes are cancelled.
        /// </summary>
        public static readonly RoutedEvent CancelledEvent = RoutedEvent.Register<TextBox, RoutedEventArgs>("Cancelled", RoutingStrategies.Bubble); // T21

        /// <summary>
        /// Raised when TextBox Text to value binding fails during validation.
        /// </summary>
        public static readonly RoutedEvent TextToSourceValueConversionFailedEvent = RoutedEvent.Register<TextBox, RoutedEventArgs>("TextBindingFailed", RoutingStrategies.Bubble); // T21

        static TextBoxBase()
		{
			ValidateOnLostFocusProperty.Changed.AddClassHandler<TextBoxBase>(OnLostFocusActionChanged);
			CancelOnLostFocusProperty.Changed.AddClassHandler<TextBoxBase>(OnLostFocusActionChanged);

            TextProperty.OverrideMetadata(typeof(TextBoxBase), new StyledPropertyMetadata<string>(string.Empty, defaultBindingMode : BindingMode.TwoWay, OnTextChanged, true));
        }

        public TextBoxBase()
        {
            Loaded += OnLoaded;
        }

        /// <summary>
        /// Gets whether this TextBox contains a non-empty text.
        /// </summary>
        private bool _HasText = false;
		public bool HasText { get { return _HasText; } private set { SetAndRaise(HasTextProperty, ref _HasText, value); } }

        /// <summary>
        /// Gets or sets whether the associated text box should get keyboard focus when this behavior is attached.
        /// </summary>
        public bool GetFocusOnLoad { get { return (bool)GetValue(GetFocusOnLoadProperty); } set { SetValue(GetFocusOnLoadProperty, value); } }

        /// <summary>
        /// Gets or sets whether the text of the TextBox must be selected when the control gets focus.
        /// </summary>
        public bool SelectAllOnFocus { get { return (bool)GetValue(SelectAllOnFocusProperty); } set { SetValue(SelectAllOnFocusProperty, value); } }

        /// <summary>
        /// Gets or sets the content to display when the TextBox is empty.
        /// </summary>
        public object WatermarkContent { get { return GetValue(WatermarkContentProperty); } set { SetValue(WatermarkContentProperty, value); } }

        /// <summary>
        /// Gets or sets the template of the content to display when the TextBox is empty.
        /// </summary>
        public DataTemplate WatermarkContentTemplate { get { return (DataTemplate)GetValue(WatermarkContentTemplateProperty); } set { SetValue(WatermarkContentTemplateProperty, value); } }

        /// <summary>
        /// Gets or sets whether the validation should happen when the user press <b>Enter</b>.
        /// </summary>
        public bool ValidateWithEnter { get { return (bool)GetValue(ValidateWithEnterProperty); } set { SetValue(ValidateWithEnterProperty, value); } }

        /// <summary>
        /// Gets or sets whether the validation should happen as soon as the <see cref="TextBox.Text"/> is changed.
        /// </summary>
        public bool ValidateOnTextChange { get { return (bool)GetValue(ValidateOnTextChangeProperty); } set { SetValue(ValidateOnTextChangeProperty, value); } }

        /// <summary>
        /// Gets or sets whether the validation should happen when the control losts focus.
        /// </summary>
        public bool ValidateOnLostFocus { get { return (bool)GetValue(ValidateOnLostFocusProperty); } set { SetValue(ValidateOnLostFocusProperty, value); } }

        /// <summary>
        /// Gets or sets whether the cancellation should happen when the user press <b>Escape</b>.
        /// </summary>
        public bool CancelWithEscape { get { return (bool)GetValue(CancelWithEscapeProperty); } set { SetValue(CancelWithEscapeProperty, value); } }

        /// <summary>
        /// Gets or sets whether the cancellation should happen when the control losts focus.
        /// </summary>
        public bool CancelOnLostFocus { get { return (bool)GetValue(CancelOnLostFocusProperty); } set { SetValue(CancelOnLostFocusProperty, value); } }

        /// <summary>
        /// Gets or sets the command to execute when the validation occurs.
        /// </summary>
        public ICommand ValidateCommand { get { return (ICommand)GetValue(ValidateCommandProperty); } set { SetValue(ValidateCommandProperty, value); } }

        /// <summary>
        /// Gets or sets the parameter of the command to execute when the validation occurs.
        /// </summary>
        public object ValidateCommandParameter { get { return GetValue(ValidateCommandParameterProprty); } set { SetValue(ValidateCommandParameterProprty, value); } }

        /// <summary>
        /// Gets or sets the command to execute when the cancellation occurs.
        /// </summary>
        public ICommand CancelCommand { get { return (ICommand)GetValue(CancelCommandProperty); } set { SetValue(CancelCommandProperty, value); } }

        /// <summary>
        /// Gets or sets the parameter of the command to execute when the cancellation occurs.
        /// </summary>
        public object CancelCommandParameter { get { return GetValue(CancelCommandParameterProprty); } set { SetValue(CancelCommandParameterProprty, value); } }

        /// <summary>
        /// Raised just before the TextBox changes are validated. This event is cancellable
        /// </summary>
        public event EventHandler<CancelRoutedEventArgs> Validating { add { AddHandler(ValidatingEvent, value); } remove { RemoveHandler(ValidatingEvent, value); } }

        /// <summary>
        /// Raised when TextBox changes have been validated.
        /// </summary>
        public event ValidationRoutedEventHandler<string> Validated { add { AddHandler(ValidatedEvent, value); } remove { RemoveHandler(ValidatedEvent, value); } }

        /// <summary>
        /// Raised when the TextBox changes are cancelled.
        /// </summary>
        public event EventHandler<RoutedEventArgs> Cancelled { add { AddHandler(CancelledEvent, value); } remove { RemoveHandler(CancelledEvent, value); } }

        /// <summary>
        /// Raised when TextBox Text to value binding fails during validation.
        /// </summary>
        public event EventHandler<RoutedEventArgs> TextToSourceValueConversionFailed { add { AddHandler(TextToSourceValueConversionFailedEvent, value); } remove { RemoveHandler(TextToSourceValueConversionFailedEvent, value); } }

        protected internal bool HasChangesToValidate { get; set; }

        /// <summary>
        /// Validates the current changes in the TextBox. Does nothing is there are no changes.
        /// </summary>
        public void Validate()
        {
            if (IsReadOnly || !HasChangesToValidate || validating)
                return;

            var cancelRoutedEventArgs = new CancelRoutedEventArgs(ValidatingEvent);
            OnValidating(cancelRoutedEventArgs);
            if (cancelRoutedEventArgs.Cancel)
                return;

            RaiseEvent(cancelRoutedEventArgs);
            if (cancelRoutedEventArgs.Cancel)
                return;

            if (!IsTextCompatibleWithValueBinding(Text))
            {
                var textBindingFailedArgs = new RoutedEventArgs(TextToSourceValueConversionFailedEvent);
                RaiseEvent(textBindingFailedArgs);
                // We allow this to continue through since it'll revert itself through later code.
            }

            validating = true;
            var coercedText = CoerceTextForValidation(Text);
            SetCurrentValue(TextProperty, coercedText);

//             BindingExpression expression = GetBindingExpression(TextProperty);
//             try
//             {
//                 expression?.UpdateSource();
//             }
//             catch (TargetInvocationException ex) when (ex.InnerException is InvalidCastException)
//             {
//                 var textBindingFailedArgs = new RoutedEventArgs(TextToSourceValueConversionFailedEvent);
//                 RaiseEvent(textBindingFailedArgs);
//             }

            ClearUndoStack();

            var validatedArgs = new ValidationRoutedEventArgs<string>(ValidatedEvent, coercedText);
            OnValidated();

            RaiseEvent(validatedArgs);
            if (ValidateCommand != null && ValidateCommand.CanExecute(ValidateCommandParameter))
                ValidateCommand.Execute(ValidateCommandParameter);
            validating = false;
            HasChangesToValidate = false;
        }

        /// <summary>
        /// Validates the content of the TextBox even if no changes occurred.
        /// </summary>
        public void ForceValidate()
        {
            HasChangesToValidate = true;
            Validate();
        }

        /// <summary>
        /// Cancels the current changes in the TextBox.
        /// </summary>
        public void Cancel()
        {
            if (IsReadOnly)
                return;

//             BindingExpression expression = GetBindingExpression(TextProperty);
//             expression?.UpdateTarget();

            ClearUndoStack();

            var cancelledArgs = new RoutedEventArgs(CancelledEvent);
            OnCancelled();
            RaiseEvent(cancelledArgs);

            if (CancelCommand != null && CancelCommand.CanExecute(CancelCommandParameter))
                CancelCommand.Execute(CancelCommandParameter);
        }

        /// <summary>
        /// Raised when the text of the TextBox changes.
        /// </summary>
        /// <param name="oldValue">The old value of the <see cref="TextBox.Text"/> property.</param>
        /// <param name="newValue">The new value of the <see cref="TextBox.Text"/> property.</param>
        protected virtual void OnTextChanged(string newValue)
        {
        }

        /// <summary>
        /// Raised when the text of the TextBox is being validated.
        /// </summary>
        /// <param name="e">The event argument.</param>
        protected virtual void OnValidating(CancelRoutedEventArgs e)
        {
        }

        /// <summary>
        /// Raised when the current changes have has been validated.
        /// </summary>
        protected virtual void OnValidated()
        {
        }

        /// <summary>
        /// Raised when the current changes have been cancelled.
        /// </summary>
        protected virtual void OnCancelled()
        {
        }

        /// <summary>
        /// Preliminary check during validation to see if the text is in a valid format.
        /// </summary>
        protected virtual bool IsTextCompatibleWithValueBinding(string text)
        {
            return true;
        }

        /// <summary>
        /// Coerces the text during the validation process. This method is invoked by <see cref="Validate"/>.
        /// </summary>
        /// <param name="baseValue">The value to coerce.</param>
        /// <returns>The coerced value.</returns>
        protected virtual string CoerceTextForValidation(string baseValue)
        {
            return MaxLength > 0 && baseValue.Length > MaxLength ? baseValue.Substring(0, MaxLength) : baseValue;
        }

        /// <inheritdoc/>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (IsReadOnly)
            {
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Enter && ValidateWithEnter)
            {
                Validate();
            }
            if (e.Key == Key.Escape && CancelWithEscape)
            {
                Cancel();
            }
        }

        protected override void OnGotFocus(GotFocusEventArgs e)
		{
			base.OnGotFocus(e);

            if (SelectAllOnFocus)
            {
                SelectAll();
            }
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            if (!IsKeyboardFocusWithin)
            {
                if (SelectAllOnFocus)
                {
                    // We handle the event only when the SelectAllOnFocus property is active. If we don't handle it, base.OnMouseDown will clear the selection
                    // we're just about to do. But if we handle it, the caret won't be moved to the cursor position, which is the behavior we expect when SelectAllOnFocus is inactive.
                    e.Handled = true;
                }
                Focus();
            }
            base.OnPointerPressed(e);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (ValidateOnLostFocus && !validating)
            {
                Validate();
            }
            if (CancelOnLostFocus)
            {
                Cancel();
            }

            base.OnLostFocus(e);
        }

        private void ClearUndoStack()
        {
            var limit = UndoLimit;
            UndoLimit = 0;
            UndoLimit = limit;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (GetFocusOnLoad)
            {
                this.Focus ();
            }
        }

        private static string OnTextChanged(AvaloniaObject d, string value)
        {
            var input = (TextBoxBase)d;
            if ((input.Text != null) && (input.Text.Equals (value)))
            {
                return value; // seems to be triggering an event loop otherwise.
            }
            input.HasText = value != null && value.Length > 0;
            if (!input.validating)
                input.HasChangesToValidate = true;

            input.OnTextChanged(value);
            if (input.ValidateOnTextChange && !input.validating)
                input.Validate();

            Console.WriteLine ("Text changed: " + input.Text + " - " + value);
            return value;
        }

        private static void OnLostFocusActionChanged(AvaloniaObject d,AvaloniaPropertyChangedEventArgs e)
        {
            var input = (TextBoxBase)d;
            if (e.Property == ValidateOnLostFocusProperty && (bool)e.NewValue)
            {
                input.SetCurrentValue(CancelOnLostFocusProperty, false);
            }
            if (e.Property == CancelOnLostFocusProperty && (bool)e.NewValue)
            {
                input.SetCurrentValue(ValidateOnLostFocusProperty, false);
            }
        }
    }
}
