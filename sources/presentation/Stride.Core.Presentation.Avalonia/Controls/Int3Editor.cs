// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using Avalonia;
using Avalonia.Controls;


using Stride.Core.Mathematics;
using Avalonia.Data;

namespace Stride.Core.Presentation.Controls
{
    public class Int3Editor : VectorEditorBase<Int3?>
    {
		static Int3Editor()
		{
			XProperty.Changed.AddClassHandler<Int3Editor>(OnComponentPropertyChanged);
			YProperty.Changed.AddClassHandler<Int3Editor>(OnComponentPropertyChanged);
			ZProperty.Changed.AddClassHandler<Int3Editor>(OnComponentPropertyChanged);
		}

        /// <summary>
        /// Identifies the <see cref="X"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<int> XProperty = StyledProperty<int>.Register<Int3Editor, int>("X", 0, defaultBindingMode : BindingMode.TwoWay, coerce: CoerceComponentValueInt); // T9C

        /// <summary>
        /// Identifies the <see cref="Y"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<int> YProperty = StyledProperty<int>.Register<Int3Editor, int>("Y", 0, defaultBindingMode : BindingMode.TwoWay, coerce: CoerceComponentValueInt); // T9C

        /// <summary>
        /// Identifies the <see cref="Z"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<int> ZProperty = StyledProperty<int>.Register<Int3Editor, int>("Z", 0, defaultBindingMode : BindingMode.TwoWay, coerce: CoerceComponentValueInt); // T9C

        /// <summary>
        /// Gets or sets the X component of the <see cref="Int3"/> associated to this control.
        /// </summary>
        public int? X { get { return (int?)GetValue(XProperty); } set { SetValue(XProperty, value); } }

        /// <summary>
        /// Gets or sets the Y component of the <see cref="Int3"/> associated to this control.
        /// </summary>
        public int? Y { get { return (int?)GetValue(YProperty); } set { SetValue(YProperty, value); } }

        /// <summary>
        /// Gets or sets the Z component of the <see cref="Int3"/> associated to this control.
        /// </summary>
        public int? Z { get { return (int?)GetValue(ZProperty); } set { SetValue(ZProperty, value); } }

        /// <inheritdoc/>
        protected override void UpdateComponentsFromValue(Int3? value)
        {
            if (value != null)
            {
                SetCurrentValue(XProperty, value.Value.X);
                SetCurrentValue(YProperty, value.Value.Y);
                SetCurrentValue(ZProperty, value.Value.Z);
            }
        }

        /// <inheritdoc/>
        protected override Int3? UpdateValueFromComponent(AvaloniaProperty property)
        {
            if (property == XProperty)
                return X.HasValue && Value.HasValue ? (Int3?)new Int3(X.Value, Value.Value.Y, Value.Value.Z) : null;
            if (property == YProperty)
                return Y.HasValue && Value.HasValue ? (Int3?)new Int3(Value.Value.X, Y.Value, Value.Value.Z) : null;
            if (property == ZProperty)
                return Z.HasValue && Value.HasValue ? (Int3?)new Int3(Value.Value.X, Value.Value.Y, Z.Value) : null;

            throw new ArgumentException("Property unsupported by method UpdateValueFromComponent.");
        }

        /// <inheritdoc/>
        protected override Int3? UpateValueFromFloat(float value)
        {
            return new Int3((int)Math.Round(value, MidpointRounding.AwayFromZero));
        }
    }
}
