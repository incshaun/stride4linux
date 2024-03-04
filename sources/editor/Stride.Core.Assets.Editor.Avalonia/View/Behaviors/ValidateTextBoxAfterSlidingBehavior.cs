// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using Avalonia;
using Avalonia.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

using Stride.Core.Presentation.Extensions;
using Avalonia.Interactivity;
using System;

namespace Stride.Core.Assets.Editor.View.Behaviors
{
    using TextBoxBase = Presentation.Controls.TextBoxBase;

    public class ValidateTextBoxAfterSlidingBehavior : Behavior<Slider>
    {
        public static readonly StyledProperty<TextBoxBase> TextBoxProperty = StyledProperty<TextBoxBase>.Register<ValidateTextBoxAfterSlidingBehavior, TextBoxBase>(nameof(TextBox)); // T1

        public TextBoxBase TextBox { get { return (TextBoxBase)GetValue(TextBoxProperty); } set { SetValue(TextBoxProperty, value); } }

        protected override void OnAttached()
        {
            AssociatedObject.AddHandler(Thumb.DragCompletedEvent, (EventHandler)OnDragCompleted);
            AssociatedObject.ValueChanged += OnValueChanged;
            AssociatedObject.KeyUp += OnKeyUp;
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.RemoveHandler(Thumb.DragCompletedEvent, (EventHandler)OnDragCompleted);
            AssociatedObject.ValueChanged -= OnValueChanged;
            AssociatedObject.KeyUp -= OnKeyUp;
            base.OnDetaching();
        }

        private void OnValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
//             if (!Avalonia.VisualTree.VisualExtensions.FindDescendantOfType<Thumb>(AssociatedObject)?.IsDragging ?? true)
                ValidateTextBox();
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.PageUp || e.Key == Key.Down || e.Key == Key.PageDown || e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Home || e.Key == Key.End)
                ValidateTextBox();
        }

        private void OnDragCompleted(object sender, EventArgs e)
        {
            ValidateTextBox();
        }

        private void ValidateTextBox()
        {
            TextBox?.Validate();
        }
    }
}
