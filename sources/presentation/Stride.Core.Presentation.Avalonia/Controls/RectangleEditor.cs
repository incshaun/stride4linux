// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using Avalonia;
using Avalonia.Controls;


using Stride.Core.Mathematics;
using Avalonia.Data;

namespace Stride.Core.Presentation.Controls
{
    public class RectangleEditor : VectorEditorBase<Rectangle?>
    {
		static RectangleEditor()
		{
			RectXProperty.Changed.AddClassHandler<RectangleEditor>(OnComponentPropertyChanged);
			RectYProperty.Changed.AddClassHandler<RectangleEditor>(OnComponentPropertyChanged);
			RectWidthProperty.Changed.AddClassHandler<RectangleEditor>(OnComponentPropertyChanged);
			RectHeightProperty.Changed.AddClassHandler<RectangleEditor>(OnComponentPropertyChanged);
		}

        /// <summary>
        /// Identifies the <see cref="RectX"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<int> RectXProperty = StyledProperty<int>.Register<RectangleEditor, int>("RectX", 0, defaultBindingMode : BindingMode.TwoWay, coerce: CoerceComponentValueInt); // T9C

        /// <summary>
        /// Identifies the <see cref="RectY"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<int> RectYProperty = StyledProperty<int>.Register<RectangleEditor, int>("RectY", 0, defaultBindingMode : BindingMode.TwoWay, coerce: CoerceComponentValueInt); // T9C

        /// <summary>
        /// Identifies the <see cref="RectWidth"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<int> RectWidthProperty = StyledProperty<int>.Register<RectangleEditor, int>("RectWidth", 0, defaultBindingMode : BindingMode.TwoWay, coerce: CoerceComponentValueInt); // T9C

        /// <summary>
        /// Identifies the <see cref="RectHeight"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<int> RectHeightProperty = StyledProperty<int>.Register<RectangleEditor, int>("RectHeight", 0, defaultBindingMode : BindingMode.TwoWay, coerce: CoerceComponentValueInt); // T9C

        /// <summary>
        /// Gets or sets the X component of the <see cref="Rectangle"/> associated to this control.
        /// </summary>
        public int? RectX { get { return (int?)GetValue(RectXProperty); } set { SetValue(RectXProperty, value); } }

        /// <summary>
        /// Gets or sets the Y component of the <see cref="Rectangle"/> associated to this control.
        /// </summary>
        public int? RectY { get { return (int?)GetValue(RectYProperty); } set { SetValue(RectYProperty, value); } }

        /// <summary>
        /// Gets or sets the Width component of the <see cref="Rectangle"/> associated to this control.
        /// </summary>
        public int? RectWidth { get { return (int?)GetValue(RectWidthProperty); } set { SetValue(RectWidthProperty, value); } }

        /// <summary>
        /// Gets or sets the Height component of the <see cref="Rectangle"/> associated to this control.
        /// </summary>
        public int? RectHeight { get { return (int?)GetValue(RectHeightProperty); } set { SetValue(RectHeightProperty, value); } }

        /// <inheritdoc/>
        protected override void UpdateComponentsFromValue(Rectangle? value)
        {
            if (value != null)
            {
                SetCurrentValue(RectXProperty, value.Value.X);
                SetCurrentValue(RectYProperty, value.Value.Y);
                SetCurrentValue(RectWidthProperty, value.Value.Width);
                SetCurrentValue(RectHeightProperty, value.Value.Height);
            }
        }

        /// <inheritdoc/>
        protected override Rectangle? UpdateValueFromComponent(AvaloniaProperty property)
        {
            if (property == RectXProperty)
                return RectX.HasValue && Value.HasValue ? (Rectangle?)new Rectangle(RectX.Value, Value.Value.Y, Value.Value.Width, Value.Value.Height) : null;
            if (property == RectYProperty)
                return RectY.HasValue && Value.HasValue ? (Rectangle?)new Rectangle(Value.Value.X, RectY.Value, Value.Value.Width, Value.Value.Height) : null;
            if (property == RectWidthProperty)
                return RectWidth.HasValue && Value.HasValue ? (Rectangle?)new Rectangle(Value.Value.X, Value.Value.Y, RectWidth.Value, Value.Value.Height) : null;
            if (property == RectHeightProperty)
                return RectHeight.HasValue && Value.HasValue ? (Rectangle?)new Rectangle(Value.Value.X, Value.Value.Y, Value.Value.Width, RectHeight.Value) : null;

            throw new ArgumentException("Property unsupported by method UpdateValueFromComponent.");
        }

        /// <inheritdoc/>
        protected override Rectangle? UpateValueFromFloat(float value)
        {
            var intValue = (int)Math.Round(value, MidpointRounding.AwayFromZero);
            return new Rectangle(0, 0, intValue, intValue);
        }
    }
}
