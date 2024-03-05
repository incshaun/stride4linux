// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using Avalonia;
using Avalonia.Controls;

using Stride.Core.Annotations;
using Stride.Core.Mathematics;
using Avalonia.Data;

namespace Stride.Core.Presentation.Controls
{
    public class Vector4Editor : VectorEditor<Vector4?>
    {
        /// <summary>
        /// Identifies the <see cref="X"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<float> XProperty = StyledProperty<float>.Register<Vector4Editor, float>("X", 0.0f, defaultBindingMode : BindingMode.TwoWay, coerce: CoerceComponentValue); // T9C

        /// <summary>
        /// Identifies the <see cref="Y"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<float> YProperty = StyledProperty<float>.Register<Vector4Editor, float>("Y", 0.0f, defaultBindingMode : BindingMode.TwoWay, coerce: CoerceComponentValue); // T9C

        /// <summary>
        /// Identifies the <see cref="Z"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<float> ZProperty = StyledProperty<float>.Register<Vector4Editor, float>("Z", 0.0f, defaultBindingMode : BindingMode.TwoWay, coerce: CoerceComponentValue); // T9C

        /// <summary>
        /// Identifies the <see cref="W"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<float> WProperty = StyledProperty<float>.Register<Vector4Editor, float>("W", 0.0f, defaultBindingMode : BindingMode.TwoWay, coerce: CoerceComponentValue); // T9C

        /// <summary>
        /// Identifies the <see cref="Length"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<float> LengthProperty = StyledProperty<float>.Register<Vector4Editor, float>("Length", 0.0f, defaultBindingMode : BindingMode.TwoWay, coerce: CoerceLengthValue); // T9C

        static Vector4Editor()
		{
			XProperty.Changed.AddClassHandler<Vector4Editor>(OnComponentPropertyChanged);
			YProperty.Changed.AddClassHandler<Vector4Editor>(OnComponentPropertyChanged);
			ZProperty.Changed.AddClassHandler<Vector4Editor>(OnComponentPropertyChanged);
			WProperty.Changed.AddClassHandler<Vector4Editor>(OnComponentPropertyChanged);
			LengthProperty.Changed.AddClassHandler<Vector4Editor>(OnComponentPropertyChanged);

            
        }

        /// <summary>
        /// Gets or sets the X component (in Cartesian coordinate system) of the <see cref="Vector4"/> associated to this control.
        /// </summary>
        public float? X { get { return (float?)GetValue(XProperty); } set { SetValue(XProperty, value); } }

        /// <summary>
        /// Gets or sets the Y component (in Cartesian coordinate system) of the <see cref="Vector4"/> associated to this control.
        /// </summary>
        public float? Y { get { return (float?)GetValue(YProperty); } set { SetValue(YProperty, value); } }

        /// <summary>
        /// Gets or sets the Z component (in Cartesian coordinate system) of the <see cref="Vector4"/> associated to this control.
        /// </summary>
        public float? Z { get { return (float?)GetValue(ZProperty); } set { SetValue(ZProperty, value); } }

        /// <summary>
        /// Gets or sets the W component (in Cartesian coordinate system) of the <see cref="Vector4"/> associated to this control.
        /// </summary>
        public float? W { get { return (float?)GetValue(WProperty); } set { SetValue(WProperty, value); } }

        /// <summary>
        /// The length (in polar coordinate system) of the <see cref="Vector4"/> associated to this control.
        /// </summary>
        public float? Length { get { return (float?)GetValue(LengthProperty); } set { SetValue(LengthProperty, value); } }

        /// <inheritdoc/>
        protected override void UpdateComponentsFromValue(Vector4? value)
        {
            if (value != null)
            {
                SetCurrentValue(XProperty, value.Value.X);
                SetCurrentValue(YProperty, value.Value.Y);
                SetCurrentValue(ZProperty, value.Value.Z);
                SetCurrentValue(WProperty, value.Value.W);
                SetCurrentValue(LengthProperty, value.Value.Length());
            }
        }

        /// <inheritdoc/>
        protected override Vector4? UpdateValueFromComponent(AvaloniaProperty property)
        {
            switch (EditingMode)
            {
                case VectorEditingMode.Normal:
                    if (property == XProperty)
                        return X.HasValue && Value.HasValue ? (Vector4?)new Vector4(X.Value, Value.Value.Y, Value.Value.Z, Value.Value.W) : null;
                    if (property == YProperty)
                        return Y.HasValue && Value.HasValue ? (Vector4?)new Vector4(Value.Value.X, Y.Value, Value.Value.Z, Value.Value.W) : null;
                    if (property == ZProperty)
                        return Z.HasValue && Value.HasValue ? (Vector4?)new Vector4(Value.Value.X, Value.Value.Y, Z.Value, Value.Value.W) : null;
                    if (property == WProperty)
                        return W.HasValue && Value.HasValue ? (Vector4?)new Vector4(Value.Value.X, Value.Value.Y, Value.Value.Z, W.Value) : null;
                    break;

                case VectorEditingMode.AllComponents:
                    if (property == XProperty)
                        return X.HasValue ? (Vector4?)new Vector4(X.Value) : null;
                    if (property == YProperty)
                        return Y.HasValue ? (Vector4?)new Vector4(Y.Value) : null;
                    if (property == ZProperty)
                        return Z.HasValue ? (Vector4?)new Vector4(Z.Value) : null;
                    if (property == WProperty)
                        return W.HasValue ? (Vector4?)new Vector4(W.Value) : null;
                    break;

                case VectorEditingMode.Length:
                    if (property == LengthProperty)
                        return Length.HasValue ? (Vector4?)FromLength(Value ?? Vector4.One, Length.Value) : null;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(EditingMode));
            }

            throw new ArgumentException($"Property {property} is unsupported by method {nameof(UpdateValueFromComponent)} in {EditingMode} mode.");
        }

        /// <inheritdoc/>
        protected override Vector4? UpateValueFromFloat(float value)
        {
            return new Vector4(value);
        }

        /// <summary>
        /// Coerce the value of the Length so it is always positive
        /// </summary>
        [NotNull]
        private static float CoerceLengthValue(AvaloniaObject sender, float baseValue)
        {
            baseValue = CoerceComponentValue(sender, baseValue);
            return Math.Max(0.0f, (float)baseValue);
        }

        private static Vector4 FromLength(Vector4 value, float length)
        {
            var newValue = value;
            newValue.Normalize();
            newValue *= length;
            return newValue;
        }
    }
}
