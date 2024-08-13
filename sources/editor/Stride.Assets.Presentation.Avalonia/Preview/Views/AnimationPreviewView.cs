// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using Avalonia;
using Avalonia.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Xaml.Interactivity;
using Stride.Core.Presentation.Controls;
using Stride.Core.Presentation.Core;
using Stride.Editor.Annotations;
using Stride.Editor.Preview.View;
using Avalonia.Animation;

namespace Stride.Assets.Presentation.Preview.Views
{
    public class ScaleFromSliderBehavior : Behavior<ScaleBar>
    {
		static ScaleFromSliderBehavior()
		{
			SliderProperty.Changed.AddClassHandler<ScaleFromSliderBehavior>(SliderChanged);
		}

        public static readonly StyledProperty<Slider> SliderProperty = StyledProperty<Slider>.Register<ScaleFromSliderBehavior, Slider>("Slider", null); // T5
        private static DependencyPropertyWatcher watcher;

        public Slider Slider { get { return (Slider)GetValue(SliderProperty); } set { SetValue(SliderProperty, value); } }

        private static void SliderChanged(AvaloniaObject d,AvaloniaPropertyChangedEventArgs e)
        {
            var behavior = (ScaleFromSliderBehavior)d;
            var slider = (Slider)e.NewValue;
            slider.LayoutUpdated += behavior.SliderLayoutUpdated;
            watcher = new DependencyPropertyWatcher(slider);
            watcher.RegisterValueChangedHandler(RangeBase.MinimumProperty, behavior.SliderLayoutUpdated);
            watcher.RegisterValueChangedHandler(RangeBase.MaximumProperty, behavior.SliderLayoutUpdated);
        }

        private void SliderLayoutUpdated(object sender, EventArgs e)
        {
            if (double.IsNaN(Slider.Minimum) || double.IsNaN(Slider.Maximum) || Slider.Minimum >= Slider.Maximum)
                return;

            var range = Slider.Maximum - Slider.Minimum;
            var tickCount = range / AssociatedObject.UnitsPerTick;
            var width = Slider.Width;
            AssociatedObject.StartUnit = Slider.Minimum;
            AssociatedObject.PixelsPerTick = width / tickCount;
        }
    }

    [AssetPreviewView<AnimationPreview>]
    public class AnimationPreviewView : StridePreviewView
    {
        static AnimationPreviewView()
        {
            // FIXME  T31
        }
    }
}
