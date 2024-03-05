// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using Avalonia;
using Avalonia.Controls;


using Stride.Core.Mathematics;
using Avalonia.Data;

namespace Stride.Core.Presentation.Controls
{
    public class Int4Editor : VectorEditorBase<Int4?>
    {
		static Int4Editor()
		{
			XProperty.Changed.AddClassHandler<Int4Editor>(OnComponentPropertyChanged);
			YProperty.Changed.AddClassHandler<Int4Editor>(OnComponentPropertyChanged);
			ZProperty.Changed.AddClassHandler<Int4Editor>(OnComponentPropertyChanged);
			WProperty.Changed.AddClassHandler<Int4Editor>(OnComponentPropertyChanged);
		}

        /// <summary>
        /// Identifies the <see cref="X"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<int> XProperty = StyledProperty<int>.Register<Int4Editor, int>("X", 0, defaultBindingMode : BindingMode.TwoWay, coerce: CoerceComponentValueInt); // T9C

        /// <summary>
        /// Identifies the <see cref="Y"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<int> YProperty = StyledProperty<int>.Register<Int4Editor, int>("Y", 0, defaultBindingMode : BindingMode.TwoWay, coerce: CoerceComponentValueInt); // T9C

        /// <summary>
        /// Identifies the <see cref="Z"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<int> ZProperty = StyledProperty<int>.Register<Int4Editor, int>("Z", 0, defaultBindingMode : BindingMode.TwoWay, coerce: CoerceComponentValueInt); // T9C

        /// <summary>
        /// Identifies the <see cref="W"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<int> WProperty = StyledProperty<int>.Register<Int4Editor, int>("W", 0, defaultBindingMode : BindingMode.TwoWay, coerce: CoerceComponentValueInt); // T9C

        /// <summary>
        /// Gets or sets the X component of the <see cref="Int4"/> associated to this control.
        /// </summary>
        public int? X { get { return (int?)GetValue(XProperty); } set { SetValue(XProperty, value); } }

        /// <summary>
        /// Gets or sets the Y component of the <see cref="Int4"/> associated to this control.
        /// </summary>
        public int? Y { get { return (int?)GetValue(YProperty); } set { SetValue(YProperty, value); } }

        /// <summary>
        /// Gets or sets the Z component of the <see cref="Int4"/> associated to this control.
        /// </summary>
        public int? Z { get { return (int?)GetValue(ZProperty); } set { SetValue(ZProperty, value); } }

        /// <summary>
        /// Gets or sets the W component of the <see cref="Int4"/> associated to this control.
        /// </summary>
        public int? W { get { return (int?)GetValue(WProperty); } set { SetValue(WProperty, value); } }

        /// <inheritdoc/>
        protected override void UpdateComponentsFromValue(Int4? value)
        {
            if (value != null)
            {
                SetCurrentValue(XProperty, value.Value.X);
                SetCurrentValue(YProperty, value.Value.Y);
                SetCurrentValue(ZProperty, value.Value.Z);
                SetCurrentValue(WProperty, value.Value.W);
            }
        }

        /// <inheritdoc/>
        protected override Int4? UpdateValueFromComponent(AvaloniaProperty property)
        {
            if (property == XProperty)
                return X.HasValue && Value.HasValue ? (Int4?)new Int4(X.Value, Value.Value.Y, Value.Value.Z, Value.Value.W) : null;
            if (property == YProperty)
                return Y.HasValue && Value.HasValue ? (Int4?)new Int4(Value.Value.X, Y.Value, Value.Value.Z, Value.Value.W) : null;
            if (property == ZProperty)
                return Z.HasValue && Value.HasValue ? (Int4?)new Int4(Value.Value.X, Value.Value.Y, Z.Value, Value.Value.W) : null;
            if (property == WProperty)
                return W.HasValue && Value.HasValue ? (Int4?)new Int4(Value.Value.X, Value.Value.Y, Value.Value.Z, W.Value) : null;

            throw new ArgumentException("Property unsupported by method UpdateValueFromComponent.");
        }

        /// <inheritdoc/>
        protected override Int4? UpateValueFromFloat(float value)
        {
            return new Int4((int)Math.Round(value, MidpointRounding.AwayFromZero));
        }
    }
}
