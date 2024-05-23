// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
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
using Stride.Core.Mathematics;
using Stride.Core.Presentation.Core;
using Stride.Core.Presentation.Extensions;

using Avalonia.Interactivity;
using Avalonia.Controls.Primitives;
using System;
using Avalonia.Data;
using Avalonia.Controls.Metadata;
using System.Windows.Input;
using Stride.Core.Presentation.Commands;

namespace Stride.Core.Presentation.Controls
{
    /// <summary>
    /// An enum describing when the related <see cref="NumericTextBox"/> should be validated, when the user uses the mouse to change its value.
    /// </summary>
    public enum MouseValidationTrigger
    {
        /// <summary>
        /// The validation occurs every time the mouse moves.
        /// </summary>
        OnMouseMove,
        /// <summary>
        /// The validation occurs when the mouse button is released.
        /// </summary>
        OnMouseUp,
    }

    public class RepeatButtonPressedRoutedEventArgs : RoutedEventArgs
    {
        public RepeatButtonPressedRoutedEventArgs(NumericTextBox.RepeatButtons button, RoutedEvent routedEvent)
            : base(routedEvent)
        {
            Button = button;
        }

        public NumericTextBox.RepeatButtons Button { get; private set; }
    }

    /// <summary>
    /// A specialization of the <see cref="TextBoxBase"/> control that can be used for numeric values.
    /// It contains a <see cref="Value"/> property that is updated on validation.
    /// </summary>
    /// PART_IncreaseButton") as RepeatButton;
    [TemplatePart(Name = "PART_IncreaseButton", Type = typeof(RepeatButton))]
    [TemplatePart(Name = "PART_DecreaseButton", Type = typeof(RepeatButton))]
    [TemplatePart(Name = "PART_ContentHost", Type = typeof(ScrollViewer))]
    public class NumericTextBox : TextBoxBase
    {
        public enum RepeatButtons
        {
            IncreaseButton,
            DecreaseButton,
        }

        private RepeatButton increaseButton;
        private RepeatButton decreaseButton;
        // FIXME: turn back private
        internal ScrollViewer contentHost;
        private bool updatingValue;

        /// <summary>
        /// The amount of pixel to move the mouse in order to add/remove a <see cref="SmallChange"/> to the current <see cref="Value"/>.
        /// </summary>
        public static readonly double DragSpeed = 3;

        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double?> ValueProperty = StyledProperty<double?>.Register<NumericTextBox, double?>(nameof(Value), 0.0, defaultBindingMode : BindingMode.TwoWay); // T9

        /// <summary>
        /// Identifies the <see cref="DecimalPlaces"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<int> DecimalPlacesProperty = StyledProperty<int>.Register<NumericTextBox, int>(nameof(DecimalPlaces), -1); // T8

        /// <summary>
        /// Identifies the <see cref="Minimum"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> MinimumProperty = StyledProperty<double>.Register<NumericTextBox, double>(nameof(Minimum), double.MinValue); // T8

        /// <summary>
        /// Identifies the <see cref="Maximum"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> MaximumProperty = StyledProperty<double>.Register<NumericTextBox, double>(nameof(Maximum), double.MaxValue); // T8

        /// <summary>
        /// Identifies the <see cref="ValueRatio"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> ValueRatioProperty = StyledProperty<double>.Register<NumericTextBox, double>(nameof(ValueRatio), default(double)); // T4

        /// <summary>
        /// Identifies the <see cref="LargeChange"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> LargeChangeProperty = StyledProperty<double>.Register<NumericTextBox, double>(nameof(LargeChange), 10.0); // T2

        /// <summary>
        /// Identifies the <see cref="SmallChange"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> SmallChangeProperty = StyledProperty<double>.Register<NumericTextBox, double>(nameof(SmallChange), 1.0); // T2

        /// <summary>
        /// Identifies the <see cref="DisplayUpDownButtons"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> DisplayUpDownButtonsProperty = StyledProperty<bool>.Register<NumericTextBox, bool>(nameof(DisplayUpDownButtons), true); // T2

        /// <summary>
        /// Identifies the <see cref="AllowMouseDrag"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> AllowMouseDragProperty = StyledProperty<bool>.Register<NumericTextBox, bool>(nameof(AllowMouseDrag), true); // T2

        /// <summary>
        /// Identifies the <see cref="MouseValidationTrigger"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<MouseValidationTrigger> MouseValidationTriggerProperty = StyledProperty<MouseValidationTrigger>.Register<NumericTextBox, MouseValidationTrigger>(nameof(MouseValidationTrigger), MouseValidationTrigger.OnMouseUp); // T2

        /// <summary>
        /// Raised when the <see cref="Value"/> property has changed.
        /// </summary>
        public static readonly RoutedEvent ValueChangedEvent = RoutedEvent.Register<NumericTextBox, RoutedEventArgs>("ValueChanged", RoutingStrategies.Bubble); // T21

        /// <summary>
        /// Raised when one of the repeat button is pressed.
        /// </summary>
        public static readonly RoutedEvent RepeatButtonPressedEvent = RoutedEvent.Register<NumericTextBox, RoutedEventArgs>("RepeatButtonPressed", RoutingStrategies.Bubble); // T21

        /// <summary>
        /// Raised when one of the repeat button is released.
        /// </summary>
        public static readonly RoutedEvent RepeatButtonReleasedEvent = RoutedEvent.Register<NumericTextBox, RoutedEventArgs>("RepeatButtonReleased", RoutingStrategies.Bubble); // T21

        /// <summary>
        /// Increases the current value with the value of the <see cref="LargeChange"/> property.
        /// </summary>
        public static ICommand LargeIncreaseCommand { get; }

        /// <summary>
        /// Increases the current value with the value of the <see cref="SmallChange"/> property.
        /// </summary>
        public static ICommand SmallIncreaseCommand { get; }

        /// <summary>
        /// Decreases the current value with the value of the <see cref="LargeChange"/> property.
        /// </summary>
        public static ICommand LargeDecreaseCommand { get; }

        /// <summary>
        /// Decreases the current value with the value of the <see cref="SmallChange"/> property.
        /// </summary>
        public static ICommand SmallDecreaseCommand { get; }

        /// <summary>
        /// Resets the current value to zero.
        /// </summary>
        public static ICommand ResetValueCommand { get; }

        static NumericTextBox()
		{
			ValueRatioProperty.Changed.AddClassHandler<NumericTextBox>(ValueRatioChanged);
			DecimalPlacesProperty.Changed.AddClassHandler<NumericTextBox>(OnDecimalPlacesPropertyChanged);
			MinimumProperty.Changed.AddClassHandler<NumericTextBox>(OnMinimumPropertyChanged);
			MaximumProperty.Changed.AddClassHandler<NumericTextBox>(OnMaximumPropertyChanged);
			ValueProperty.Changed.AddClassHandler<NumericTextBox>(OnValuePropertyChanged);

            // FIXME  T31
            ScrollViewer.HorizontalScrollBarVisibilityProperty.OverrideMetadata(typeof(NumericTextBox), new StyledPropertyMetadata<ScrollBarVisibility>(ScrollBarVisibility.Hidden, coerce: OnForbiddenPropertyChanged));
            ScrollViewer.VerticalScrollBarVisibilityProperty.OverrideMetadata(typeof(NumericTextBox), new StyledPropertyMetadata<ScrollBarVisibility>(ScrollBarVisibility.Hidden, coerce: OnForbiddenPropertyChanged));
            AcceptsReturnProperty.OverrideMetadata(typeof(NumericTextBox), new StyledPropertyMetadata<bool>(false, coerce: OnForbiddenPropertyChanged));
            AcceptsTabProperty.OverrideMetadata(typeof(NumericTextBox), new StyledPropertyMetadata<bool>(false, coerce: OnForbiddenPropertyChanged));

            // Since the NumericTextBox is not focusable itself, we have to bind the commands to the inner text box of the control.
            // The handlers will then find the parent that is a NumericTextBox and process the command on this control if it is found.
            LargeIncreaseCommand = new RoutedCommand<Avalonia.Controls.TextBox>(OnLargeIncreaseCommand);
//             CommandManager.RegisterClassInputBinding(typeof(Avalonia.Controls.TextBox), new InputBinding(LargeIncreaseCommand, new KeyGesture(Key.PageUp)));
//             CommandManager.RegisterClassInputBinding(typeof(Avalonia.Controls.TextBox), new InputBinding(LargeIncreaseCommand, new KeyGesture(Key.Up, KeyModifiers.Shift)));

            LargeDecreaseCommand = new RoutedCommand<Avalonia.Controls.TextBox>(OnLargeDecreaseCommand);
//             CommandManager.RegisterClassInputBinding(typeof(Avalonia.Controls.TextBox), new InputBinding(LargeDecreaseCommand, new KeyGesture(Key.PageDown)));
//             CommandManager.RegisterClassInputBinding(typeof(Avalonia.Controls.TextBox), new InputBinding(LargeDecreaseCommand, new KeyGesture(Key.Down, KeyModifiers.Shift)));

            SmallIncreaseCommand = new RoutedCommand<Avalonia.Controls.TextBox>(OnSmallIncreaseCommand);
//             CommandManager.RegisterClassInputBinding(typeof(Avalonia.Controls.TextBox), new InputBinding(SmallIncreaseCommand, new KeyGesture(Key.Up)));

            SmallDecreaseCommand = new RoutedCommand<Avalonia.Controls.TextBox>(OnSmallDecreaseCommand);
//             CommandManager.RegisterClassInputBinding(typeof(Avalonia.Controls.TextBox), new InputBinding(SmallDecreaseCommand, new KeyGesture(Key.Down)));

            ResetValueCommand = new RoutedCommand<Avalonia.Controls.TextBox>(OnResetValueCommand);
        }

        /// <summary>
        /// Gets or sets the current value of the <see cref="NumericTextBox"/>.
        /// </summary>
        public double? Value { get { return (double?)GetValue(ValueProperty); } set { SetValue(ValueProperty, value); } }

        /// <summary>
        /// Gets or sets the number of decimal places displayed in the <see cref="NumericTextBox"/>.
        /// </summary>
        public int DecimalPlaces { get { return (int)GetValue(DecimalPlacesProperty); } set { SetValue(DecimalPlacesProperty, value); } }

        /// <summary>
        /// Gets or sets the minimum value that can be set on the <see cref="Value"/> property.
        /// </summary>
        public double Minimum { get { return (double)GetValue(MinimumProperty); } set { SetValue(MinimumProperty, value); } }

        /// <summary>
        /// Gets or sets the maximum value that can be set on the <see cref="Value"/> property.
        /// </summary>
        public double Maximum { get { return (double)GetValue(MaximumProperty); } set { SetValue(MaximumProperty, value); } }

        /// <summary>
        /// Gets or sets the ratio of the <see cref="NumericTextBox.Value"/> between <see cref="NumericTextBox.Minimum"/> (0.0) and
        /// <see cref="NumericTextBox.Maximum"/> (1.0).
        /// </summary>
        public double ValueRatio { get { return (double)GetValue(ValueRatioProperty); } set { SetValue(ValueRatioProperty, value); } }

        /// <summary>
        /// Gets or sets the value to be added to or substracted from the <see cref="NumericTextBox.Value"/>.
        /// </summary>
        public double LargeChange { get { return (double)GetValue(LargeChangeProperty); } set { SetValue(LargeChangeProperty, value); } }

        /// <summary>
        /// Gets or sets the value to be added to or substracted from the <see cref="NumericTextBox.Value"/>.
        /// </summary>
        public double SmallChange { get { return (double)GetValue(SmallChangeProperty); } set { SetValue(SmallChangeProperty, value); } }

        /// <summary>
        /// Gets or sets whether to display Up and Down buttons on the side of the <see cref="NumericTextBox"/>.
        /// </summary>
        public bool DisplayUpDownButtons { get { return (bool)GetValue(DisplayUpDownButtonsProperty); } set { SetValue(DisplayUpDownButtonsProperty, value); } }

        /// <summary>
        /// Gets or sets whether dragging the value of the <see cref="NumericTextBox"/> is enabled.
        /// </summary>
        public bool AllowMouseDrag { get { return (bool)GetValue(AllowMouseDragProperty); } set { SetValue(AllowMouseDragProperty, value); } }

        /// <summary>
        /// Gets or sets when the <see cref="NumericTextBox"/> should be validated when the user uses the mouse to change its value.
        /// </summary>
        public MouseValidationTrigger MouseValidationTrigger { get { return (MouseValidationTrigger)GetValue(MouseValidationTriggerProperty); } set { SetValue(MouseValidationTriggerProperty, value); } }

        /// <summary>
        /// Raised when the <see cref="Value"/> property has changed.
        /// </summary>
        public event EventHandler<RoutedEventArgs> ValueChanged { add { AddHandler(ValueChangedEvent, value); } remove { RemoveHandler(ValueChangedEvent, value); } }

        /// <summary>
        /// Raised when one of the repeat button is pressed.
        /// </summary>
        public event EventHandler<RepeatButtonPressedRoutedEventArgs> RepeatButtonPressed { add { AddHandler(RepeatButtonPressedEvent, value); } remove { RemoveHandler(RepeatButtonPressedEvent, value); } }

        /// <summary>
        /// Raised when one of the repeat button is released.
        /// </summary>
        public event EventHandler<RepeatButtonPressedRoutedEventArgs> RepeatButtonReleased { add { AddHandler(RepeatButtonReleasedEvent, value); } remove { RemoveHandler(RepeatButtonReleasedEvent, value); } }

        /// <inheritdoc/>
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
		{
			base.OnApplyTemplate(e);

            increaseButton = e.NameScope.Find<RepeatButton>("PART_IncreaseButton");
            if (increaseButton == null)
                throw new InvalidOperationException("A part named 'PART_IncreaseButton' must be present in the ControlTemplate, and must be of type 'RepeatButton'.");

            decreaseButton = e.NameScope.Find<RepeatButton>("PART_DecreaseButton");
            if (decreaseButton == null)
                throw new InvalidOperationException("A part named 'PART_DecreaseButton' must be present in the ControlTemplate, and must be of type 'RepeatButton'.");

            contentHost = e.NameScope.Find<ScrollViewer>("PART_ContentHost");
            if (contentHost == null)
                throw new InvalidOperationException("A part named 'PART_ContentHost' must be present in the ControlTemplate, and must be of type 'ScrollViewer'.");

            var increasePressedWatcher = new DependencyPropertyWatcher(increaseButton);
            increasePressedWatcher.RegisterValueChangedHandler(Button.IsPressedProperty, RepeatButtonIsPressedChanged);
            var decreasePressedWatcher = new DependencyPropertyWatcher(decreaseButton);
            decreasePressedWatcher.RegisterValueChangedHandler(Button.IsPressedProperty, RepeatButtonIsPressedChanged);
            var textValue = FormatValue(Value);

            SetCurrentValue(TextProperty, textValue);
        }

        /// <summary>
        /// Raised when the <see cref="Value"/> property has changed.
        /// </summary>
        protected virtual void OnValueChanged(double? oldValue, double? newValue)
        {
        }

        /// <inheritdoc/>
        protected override void OnInitialized()
		{
			base.OnInitialized();
            var textValue = FormatValue(Value);
            SetCurrentValue(TextProperty, textValue);
        }

        /// <inheritdoc/>
        protected sealed override void OnCancelled()
        {
//             var expression = GetBindingExpression(ValueProperty);
//             expression?.UpdateTarget();

            var textValue = FormatValue(Value);
            SetCurrentValue(TextProperty, textValue);
        }

        /// <inheritdoc/>
        protected sealed override void OnValidated()
        {
            double? value;
            if (TryParseValue(Text, out var parsedValue))
            {
                value = parsedValue;
            }
            else
            {
                value = Value;
            }
            SetCurrentValue(ValueProperty, value);

//             var expression = GetBindingExpression(ValueProperty);
//             expression?.UpdateSource();
        }

        protected override bool IsTextCompatibleWithValueBinding(string text)
        {
            return TryParseValue(text, out _);
        }

        /// <inheritdoc/>
        [NotNull]
        protected override string CoerceTextForValidation(string baseValue)
        {
            baseValue = base.CoerceTextForValidation(baseValue);
            double? value;
            if (TryParseValue(baseValue, out var parsedValue))
            {
                value = parsedValue;

                if (value > Maximum)
                {
                    value = Maximum;
                }
                if (value < Minimum)
                {
                    value = Minimum;
                }
            }
            else
            {
                value = Value;
            }

            return FormatValue(value);
        }

        [NotNull]
        protected string FormatValue(double? value)
        {
            if (!value.HasValue)
                return string.Empty;

            var decimalPlaces = DecimalPlaces;
            var coercedValue = decimalPlaces < 0 ? value.Value : Math.Round(value.Value, decimalPlaces);
            return coercedValue.ToString(CultureInfo.InvariantCulture);
        }

        private void RepeatButtonIsPressedChanged(object sender, EventArgs e)
        {
            var repeatButton = (RepeatButton)sender;
            if (ReferenceEquals(repeatButton, increaseButton))
            {
                RaiseEvent(new RepeatButtonPressedRoutedEventArgs(RepeatButtons.IncreaseButton, repeatButton.IsPressed ? RepeatButtonPressedEvent : RepeatButtonReleasedEvent));
            }
            if (ReferenceEquals(repeatButton, decreaseButton))
            {
                RaiseEvent(new RepeatButtonPressedRoutedEventArgs(RepeatButtons.DecreaseButton, repeatButton.IsPressed ? RepeatButtonPressedEvent : RepeatButtonReleasedEvent));
            }
        }

        private void OnValuePropertyChanged(double? oldValue, double? newValue)
        {
            if (newValue.HasValue && newValue.Value > Maximum)
            {
                SetCurrentValue(ValueProperty, Maximum);
                return;
            }
            if (newValue.HasValue && newValue.Value < Minimum)
            {
                SetCurrentValue(ValueProperty, Minimum);
                return;
            }

            var textValue = FormatValue(newValue);
            updatingValue = true;
            SetCurrentValue(TextProperty, textValue);
            SetCurrentValue(ValueRatioProperty, newValue.HasValue ? MathUtil.InverseLerp(Minimum, Maximum, newValue.Value) : 0.0);
            updatingValue = false;

//             RaiseEvent(new AvaloniaPropertyChangedEventArgs<double?>(oldValue, newValue, ValueChangedEvent));
            RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
            OnValueChanged(oldValue, newValue);
        }

        private void UpdateValue(double value)
        {
            if (IsReadOnly == false)
            {
                SetCurrentValue(ValueProperty, value);
            }
        }

        /// <summary>
        /// tries to parse the value of the textbox into a double, accommodating various cultural settings and preferences
        /// </summary>
        /// <param name="value">text value of the textbox</param>
        /// <param name="result">the resulting numeric value if parsing is successful</param>
        /// <returns>whether parsing the value was successful</returns>
        private static bool TryParseValue(ReadOnlySpan<char> value, out double result)
        {
            //thousands are disallowed as they might lead to decimal seperators falsely being interpreted as thousands
            const NumberStyles numberStyle = NumberStyles.Any & ~NumberStyles.AllowThousands;

            //try parsing a hex string
            var span = value.TrimStart('0');
            if (span.StartsWith("x", StringComparison.OrdinalIgnoreCase) || span.StartsWith("#", StringComparison.OrdinalIgnoreCase))
            {
                var span2 = span.TrimStart(stackalloc[] {'x', '#'});
                if (double.TryParse(span2, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out result))
                    return true;
            }
            //Try parsing in the current culture
            else if (double.TryParse(value, numberStyle, CultureInfo.CurrentCulture, out result) ||
                //or in neutral culture
                double.TryParse(value, numberStyle, CultureInfo.InvariantCulture, out result) ||
                //or as fallback a culture that has ',' as comma separator
                double.TryParse(value, numberStyle, CultureInfo.GetCultureInfo("de-DE"), out result))
            {
                return true;
            }

            return false;
        }

        private static void OnValuePropertyChanged(AvaloniaObject sender,AvaloniaPropertyChangedEventArgs e)
        {
            ((NumericTextBox)sender).OnValuePropertyChanged((double?)e.OldValue, (double?)e.NewValue);
        }

        private static void OnDecimalPlacesPropertyChanged(AvaloniaObject sender,AvaloniaPropertyChangedEventArgs e)
        {
            var numericInput = (NumericTextBox)sender;
            numericInput.CoerceValue(ValueProperty);
        }

        private static void OnMinimumPropertyChanged(AvaloniaObject sender,AvaloniaPropertyChangedEventArgs e)
        {
            var numericInput = (NumericTextBox)sender;
            var needValidation = false;
            if (numericInput.Maximum < numericInput.Minimum)
            {
                numericInput.SetCurrentValue(MaximumProperty, numericInput.Minimum);
                needValidation = true;
            }
            if (numericInput.Value < numericInput.Minimum)
            {
                numericInput.SetCurrentValue(ValueProperty, numericInput.Minimum);
                needValidation = true;
            }

            // Do not overwrite the Value, it is already correct!
            numericInput.updatingValue = true;
            numericInput.SetCurrentValue(ValueRatioProperty, numericInput.Value.HasValue ? MathUtil.InverseLerp(numericInput.Minimum, numericInput.Maximum, numericInput.Value.Value) : 0.0);
            numericInput.updatingValue = false;

            if (needValidation)
            {
                numericInput.Validate();
            }
        }

        private static void OnMaximumPropertyChanged(AvaloniaObject sender,AvaloniaPropertyChangedEventArgs e)
        {
            var numericInput = (NumericTextBox)sender;
            var needValidation = false;
            if (numericInput.Minimum > numericInput.Maximum)
            {
                numericInput.SetCurrentValue(MinimumProperty, numericInput.Maximum);
                needValidation = true;
            }
            if (numericInput.Value > numericInput.Maximum)
            {
                numericInput.SetCurrentValue(ValueProperty, numericInput.Maximum);
                needValidation = true;
            }

            // Do not overwrite the Value, it is already correct!
            numericInput.updatingValue = true;
            numericInput.SetCurrentValue(ValueRatioProperty, numericInput.Value.HasValue ? MathUtil.InverseLerp(numericInput.Minimum, numericInput.Maximum, numericInput.Value.Value) : 0.0);
            numericInput.updatingValue = false;

            if (needValidation)
            {
                numericInput.Validate();
            }
        }

        private static void ValueRatioChanged(AvaloniaObject d,AvaloniaPropertyChangedEventArgs e)
        {
            var control = (NumericTextBox)d;
            if (control != null && !control.updatingValue)
                control.UpdateValue(MathUtil.Lerp(control.Minimum, control.Maximum, (double)e.NewValue));
        }

        private static void UpdateValueCommand([NotNull] object sender, Func<NumericTextBox, double> getValue, bool validate = true)
        {
            var control = sender as NumericTextBox ?? global::Avalonia.VisualTree.VisualExtensions.FindAncestorOfType<NumericTextBox>((Visual) ((Avalonia.Controls.TextBox)sender));
            if (control != null)
            {
                var value = getValue(control);
                control.UpdateValue(value);
                control.SelectAll();
                if (validate)
                    control.Validate();
            }
        }

        private static void OnLargeIncreaseCommand([NotNull] Avalonia.Controls.TextBox sender)
        {
            UpdateValueCommand(sender, x => (x.Value ?? x.Minimum) + x.LargeChange);
        }

        private static void OnLargeDecreaseCommand([NotNull] Avalonia.Controls.TextBox sender)
        {
            UpdateValueCommand(sender, x => (x.Value ?? x.Maximum) - x.LargeChange);
        }

        private static void OnSmallIncreaseCommand([NotNull] Avalonia.Controls.TextBox sender)
        {
            UpdateValueCommand(sender, x => (x.Value ?? x.Minimum) + x.SmallChange);
        }

        private static void OnSmallDecreaseCommand([NotNull] Avalonia.Controls.TextBox sender)
        {
            UpdateValueCommand(sender, x => (x.Value ?? x.Maximum) - x.SmallChange);
        }

        private static void OnResetValueCommand([NotNull] Avalonia.Controls.TextBox sender)
        {
            UpdateValueCommand(sender, x => 0.0, false);
        }

        private static ScrollBarVisibility OnForbiddenPropertyChanged([NotNull] AvaloniaObject d, ScrollBarVisibility v)
        {
            return v;
        }
        
        private static bool OnForbiddenPropertyChanged([NotNull] AvaloniaObject d, bool v)
        {
//             var metadata = e.Property.GetMetadata(d);
//             if (!Equals(e.NewValue, metadata.DefaultValue))
//             {
//                 var message = $"The value of the property '{e.Property.Name}' cannot be different from the value '{metadata.DefaultValue}'";
//                 throw new InvalidOperationException(message);
//             }
            return v;
        }
    }
}
