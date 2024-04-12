// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using Avalonia;
using Avalonia.Controls;

using Stride.Core.Presentation.Controls;
using Avalonia.Data;

namespace Stride.Assets.Presentation.AssetEditors.UIEditor.Views
{
    using Thickness = Stride.UI.Thickness;

    public class ThicknessEditor : VectorEditorBase<Thickness?>
    {
		static ThicknessEditor()
		{
			BackProperty.Changed.AddClassHandler<ThicknessEditor>(OnComponentPropertyChanged);
			BottomProperty.Changed.AddClassHandler<ThicknessEditor>(OnComponentPropertyChanged);
			FrontProperty.Changed.AddClassHandler<ThicknessEditor>(OnComponentPropertyChanged);
			LeftProperty.Changed.AddClassHandler<ThicknessEditor>(OnComponentPropertyChanged);
			RightProperty.Changed.AddClassHandler<ThicknessEditor>(OnComponentPropertyChanged);
			TopProperty.Changed.AddClassHandler<ThicknessEditor>(OnComponentPropertyChanged);
		}

        /// <summary>
        /// Identifies the <see cref="Back"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<float> BackProperty = StyledProperty<float>.Register<ThicknessEditor, float>(nameof(Back), 0.0f, defaultBindingMode : BindingMode.TwoWay, coerce: CoerceComponentValue); // T9C

        /// <summary>
        /// Identifies the <see cref="Bottom"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<float> BottomProperty = StyledProperty<float>.Register<ThicknessEditor, float>(nameof(Bottom), 0.0f, defaultBindingMode : BindingMode.TwoWay, coerce: CoerceComponentValue); // T9C

        /// <summary>
        /// Identifies the <see cref="Front"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<float> FrontProperty = StyledProperty<float>.Register<ThicknessEditor, float>(nameof(Front), 0.0f, defaultBindingMode : BindingMode.TwoWay, coerce: CoerceComponentValue); // T9C

        /// <summary>
        /// Identifies the <see cref="Left"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<float> LeftProperty = StyledProperty<float>.Register<ThicknessEditor, float>(nameof(Left), 0.0f, defaultBindingMode : BindingMode.TwoWay, coerce: CoerceComponentValue); // T9C

        /// <summary>
        /// Identifies the <see cref="Right"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<float> RightProperty = StyledProperty<float>.Register<ThicknessEditor, float>(nameof(Right), 0.0f, defaultBindingMode : BindingMode.TwoWay, coerce: CoerceComponentValue); // T9C

        /// <summary>
        /// Identifies the <see cref="Top"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<float> TopProperty = StyledProperty<float>.Register<ThicknessEditor, float>(nameof(Top), 0.0f, defaultBindingMode : BindingMode.TwoWay, coerce: CoerceComponentValue); // T9C

        /// <summary>
        /// Gets or sets the <see cref="Thickness.Back"/> component of the <see cref="Thickness"/> associated to this control.
        /// </summary>
        public float? Back { get { return (float)GetValue(BackProperty); } set { SetValue(BackProperty, value); } }

        /// <summary>
        /// Gets or sets the <see cref="Thickness.Bottom"/> component of the <see cref="Thickness"/> associated to this control.
        /// </summary>
        public float? Bottom { get { return (float)GetValue(BottomProperty); } set { SetValue(BottomProperty, value); } }

        /// <summary>
        /// Gets or sets the <see cref="Thickness.Front"/> component of the <see cref="Thickness"/> associated to this control.
        /// </summary>
        public float? Front { get { return (float)GetValue(FrontProperty); } set { SetValue(FrontProperty, value); } }

        /// <summary>
        /// Gets or sets the <see cref="Thickness.Left"/> component of the <see cref="Thickness"/> associated to this control.
        /// </summary>
        public float? Left { get { return (float)GetValue(LeftProperty); } set { SetValue(LeftProperty, value); } }
        
        /// <summary>
        /// Gets or sets the <see cref="Thickness.Right"/> component of the <see cref="Thickness"/> associated to this control.
        /// </summary>
        public float? Right { get { return (float)GetValue(RightProperty); } set { SetValue(RightProperty, value); } }

        /// <summary>
        /// Gets or sets the <see cref="Thickness.Top"/> component of the <see cref="Thickness"/> associated to this control.
        /// </summary>
        public float? Top { get { return (float)GetValue(TopProperty); } set { SetValue(TopProperty, value); } }

        /// <inheritdoc/>
        protected override void UpdateComponentsFromValue(Thickness? value)
        {
            if (value != null)
            {
                SetCurrentValue(LeftProperty, value.Value.Left);
                SetCurrentValue(TopProperty, value.Value.Top);
                SetCurrentValue(BackProperty, value.Value.Back);
                SetCurrentValue(RightProperty, value.Value.Right);
                SetCurrentValue(BottomProperty, value.Value.Bottom);
                SetCurrentValue(FrontProperty, value.Value.Front);
            }
        }

        /// <inheritdoc/>
        protected override Thickness? UpdateValueFromComponent(AvaloniaProperty property)
        {
            if (property == LeftProperty)
                return Left.HasValue && Value.HasValue ? (Thickness?)new Thickness(Left.Value, Value.Value.Top, Value.Value.Back, Value.Value.Right, Value.Value.Bottom, Value.Value.Front) : null;
            if (property == TopProperty)
                return Top.HasValue && Value.HasValue ? (Thickness?)new Thickness(Value.Value.Left, Top.Value, Value.Value.Back, Value.Value.Right, Value.Value.Bottom, Value.Value.Front) : null;
            if (property == BackProperty)
                return Back.HasValue && Value.HasValue ? (Thickness?)new Thickness(Value.Value.Left, Value.Value.Top, Back.Value, Value.Value.Right, Value.Value.Bottom, Value.Value.Front) : null;
            if (property == RightProperty)
                return Right.HasValue && Value.HasValue ? (Thickness?)new Thickness(Value.Value.Left, Value.Value.Top, Value.Value.Back, Right.Value, Value.Value.Bottom, Value.Value.Front) : null;
            if (property == BottomProperty)
                return Bottom.HasValue && Value.HasValue ? (Thickness?)new Thickness(Value.Value.Left, Value.Value.Top, Value.Value.Back, Value.Value.Right, Bottom.Value, Value.Value.Front) : null;
            if (property == FrontProperty)
                return Front.HasValue && Value.HasValue ? (Thickness?)new Thickness(Value.Value.Left, Value.Value.Top, Value.Value.Back, Value.Value.Right, Value.Value.Bottom, Front.Value) : null;

            throw new ArgumentException("Property unsupported by method UpdateValueFromComponent.");
        }

        /// <inheritdoc/>
        protected override Thickness? UpateValueFromFloat(float value)
        {
            return Thickness.UniformCuboid(value);
        }
    }
}
