// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.ComponentModel;
using System.Threading;
using Avalonia;
using Avalonia.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;

using Avalonia.Interactivity;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Controls.Metadata;
using System.Windows.Input;
using Stride.Core.Presentation.Commands;

namespace Stride.Core.Presentation.Controls
{
    /// <summary>
    /// An implementation of the <see cref="TextBoxBase"/> control that provides additional features such as a proper
    /// validation/cancellation workflow, and a watermark to display when the text is empty.
    /// </summary>
    [TemplatePart(Name = "PART_TrimmedText", Type = typeof(TextBlock))]
    public class TextBox : TextBoxBase
    {
        private TextBlock trimmedTextBlock;
        private readonly Timer validationTimer;

        /// <summary>
        /// Identifies the <see cref="UseTimedValidation"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> UseTimedValidationProperty = StyledProperty<bool>.Register<TextBox, bool>("UseTimedValidation", false); // T5

        /// <summary>
        /// Identifies the <see cref="ValidationDelay"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<int> ValidationDelayProperty = StyledProperty<int>.Register<TextBox, int>("ValidationDelay", 500); // T2
        
        /// <summary>
        /// Identifies the <see cref="TrimmedText"/> dependency property.
        /// </summary>
        public static readonly DirectProperty<TextBox, string> TrimmedTextProperty = AvaloniaProperty.RegisterDirect<TextBox, string>("TrimmedText", o => o.TrimmedText); // T10H3

        /// <summary>
        /// Clears the current <see cref="Avalonia.Controls.TextBox.Text"/> of a text box.
        /// </summary>
        public static ICommand ClearTextCommand { get; }
        
        static TextBox()
		{
			UseTimedValidationProperty.Changed.AddClassHandler<TextBox>(OnUseTimedValidationPropertyChanged);

            // FIXME  T31
            ClearTextCommand = new RoutedCommand<Avalonia.Controls.TextBox>(OnClearTextCommand);
        }

        public TextBox()
        {
//             if (DesignerProperties.GetIsInDesignMode(this) == false)
//                 validationTimer = new Timer(x => Dispatcher.InvokeAsync(Validate), null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// Gets or sets whether the text should be automatically validated after a delay defined by the <see cref="ValidationDelay"/> property.
        /// </summary>
        public bool UseTimedValidation { get { return (bool)GetValue(UseTimedValidationProperty); } set { SetValue(UseTimedValidationProperty, value); } }

        /// <summary>
        /// Gets or sets the amount of time before a validation of input text happens, in milliseconds.
        /// Every change to the <see cref="TextBox.Text"/> property reset the timer to this value.
        /// </summary>
        /// <remarks>The default value is <c>500</c> milliseconds.</remarks>
        public int ValidationDelay { get { return (int)GetValue(ValidationDelayProperty); } set { SetValue(ValidationDelayProperty, value); } }
        
        /// <summary>
        /// Gets the trimmed text to display when the control does not have the focus, depending of the value of the <see cref="TextTrimming"/> property.
        /// </summary>
        private string _TrimmedText;
		public string TrimmedText { get { return _TrimmedText; } private set { SetAndRaise(TrimmedTextProperty, ref _TrimmedText, value); } }

        /// <inheritdoc/>
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
		{
			base.OnApplyTemplate(e);

            trimmedTextBlock = e.NameScope.Find<TextBlock>("PART_TrimmedText");
            if (trimmedTextBlock == null)
                throw new InvalidOperationException("A part named 'PART_TrimmedText' must be present in the ControlTemplate, and must be of type 'TextBlock'.");
        }

        /// <summary>
        /// Raised when the text of the TextBox changes.
        /// </summary>
        /// <param name="oldValue">The old value of the <see cref="TextBox.Text"/> property.</param>
        /// <param name="newValue">The new value of the <see cref="TextBox.Text"/> property.</param>
        protected override void OnTextChanged(string newValue)
        {
            if (UseTimedValidation)
            {
                if (ValidationDelay > 0.0)
                {
                    validationTimer?.Change(ValidationDelay, Timeout.Infinite);
                }
                else
                {
                    Validate();
                }
            }
            
            var availableWidth = Width;
            if (trimmedTextBlock != null)
                availableWidth -= trimmedTextBlock.Margin.Left + trimmedTextBlock.Margin.Right;

            TrimmedText = Trimming.ProcessTrimming(this, Text, availableWidth);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var arrangedSize = base.ArrangeOverride(arrangeBounds);
            var availableWidth = arrangeBounds.Width;
            if (trimmedTextBlock != null)
                availableWidth -= trimmedTextBlock.Margin.Left + trimmedTextBlock.Margin.Right;

            TrimmedText = Trimming.ProcessTrimming(this, Text, availableWidth);
            return arrangedSize;
        }

        private static void OnUseTimedValidationPropertyChanged(AvaloniaObject sender,AvaloniaPropertyChangedEventArgs e)
        {
            var txt = (TextBox)sender;
            if ((bool)e.NewValue)
            {
                txt.Validate();
            }
        }

        private static void OnClearTextCommand(Avalonia.Controls.TextBox sender)
        {
            var textBox = sender as TextBox;
            textBox?.Clear();
        }
    }
}
