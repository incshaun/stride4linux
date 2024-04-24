// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Avalonia;
using Avalonia.Controls;

using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Animation;
using Avalonia.Xaml.Interactivity;
using Stride.Core.Annotations;
using Stride.Core.Presentation.Adorners;
using Stride.Core.Presentation.Controls;
using Stride.Core.Presentation.Core;
using Avalonia.Interactivity;
using Avalonia.Animation;

namespace Stride.Core.Assets.Editor.View.Behaviors
{
    public class TextBoxPropertyValueValidationBehavior : Behavior<TextBoxBase>
    {
		static TextBoxPropertyValueValidationBehavior()
		{
// 			AdornerStoryboardProperty.Changed.AddClassHandler<TextBoxPropertyValueValidationBehavior>(AdornerStoryboardPropertyChanged);
			BorderBrushProperty.Changed.AddClassHandler<TextBoxPropertyValueValidationBehavior>(BorderPropertyChanged);
			BorderCornerRadiusProperty.Changed.AddClassHandler<TextBoxPropertyValueValidationBehavior>(BorderPropertyChanged);
			BorderThicknessProperty.Changed.AddClassHandler<TextBoxPropertyValueValidationBehavior>(BorderPropertyChanged);
		}

        private HighlightBorderAdorner adorner;

        /// <summary>
        /// Identifies the <see cref="AdornerStoryboard"/> dependency property.
        /// </summary>
//         public static readonly StyledProperty<Storyboard> AdornerStoryboardProperty = StyledProperty<Storyboard>.Register<TextBoxPropertyValueValidationBehavior, Storyboard>(nameof(AdornerStoryboard), null);
        /// <summary>
        /// Identifies the <see cref="BorderBrush"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<IBrush> BorderBrushProperty = StyledProperty<IBrush>.Register<TextBoxPropertyValueValidationBehavior, IBrush>(nameof(BorderBrush), Brushes.IndianRed);
        /// <summary>
        /// Identifies the <see cref="BorderCornerRadius"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> BorderCornerRadiusProperty = StyledProperty<double>.Register<TextBoxPropertyValueValidationBehavior, double>(nameof(BorderCornerRadius), 3.0);
        /// <summary>
        /// Identifies the <see cref="BorderThickness"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> BorderThicknessProperty = StyledProperty<double>.Register<TextBoxPropertyValueValidationBehavior, double>(nameof(BorderThickness), 2.0);

        /// <summary>
        /// Gets or sets the <see cref="Storyboard"/> associated to this behavior.
        /// </summary>
//         public Storyboard AdornerStoryboard { get { return (Storyboard)GetValue(AdornerStoryboardProperty); } set { SetValue(AdornerStoryboardProperty, value); } }
        /// <summary>
        /// Gets or sets the border brush when the adorner visible.
        /// </summary>
        public Brush BorderBrush { get { return (Brush)GetValue(BorderBrushProperty); } set { SetValue(BorderBrushProperty, value); } }
        /// <summary>
        /// Gets or sets the border corner radius when the adorner is visible.
        /// </summary>
        public double BorderCornerRadius { get { return (double)GetValue(BorderCornerRadiusProperty); } set { SetValue(BorderCornerRadiusProperty, value); } }
        /// <summary>
        /// Gets or sets the border thickness when the adorner is visible.
        /// </summary>
        public double BorderThickness { get { return (double)GetValue(BorderThicknessProperty); } set { SetValue(BorderThicknessProperty, value); } }

        protected override void OnAttached()
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(AssociatedObject);
            if (adornerLayer != null)
            {
                adorner = new HighlightBorderAdorner(AssociatedObject)
                {
                    BackgroundBrush = null,
                    BorderBrush = BorderBrush,
                    BorderCornerRadius = BorderCornerRadius,
                    BorderThickness = BorderThickness,
                    State = HighlightAdornerState.Hidden,
                };
//                 AdornerLayer.AddVisualAdorner (adorner, AdornerLayer.GetAdorner (adorner), adornerLayer);
            }

            AssociatedObject.Validating += OnValidating;
            AssociatedObject.TextToSourceValueConversionFailed += OnTextToSourceValueConversionFailed;

            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.Validating -= OnValidating;
            AssociatedObject.TextToSourceValueConversionFailed -= OnTextToSourceValueConversionFailed;

            if (adorner != null)
            {
//                 if (AdornerStoryboard != null)
//                 {
//                     AdornerStoryboard.Remove(adorner);
//                 }
//                 AdornerLayer.GetAdornerLayer(AssociatedObject)?.Remove(adorner);
            }
        }

        private void OnValidating(object sender, CancelRoutedEventArgs e)
        {
//             adorner.State = HighlightAdornerState.Hidden;
        }

        private void OnTextToSourceValueConversionFailed(object sender, RoutedEventArgs e)
        {
//             if (AdornerStoryboard != null && adorner != null)
//             {
//                 adorner.State = HighlightAdornerState.Visible;
//                 // Show visual indicator it has failed.
//                 AdornerStoryboard.Begin(adorner);
//             }
        }

        private static void AdornerStoryboardPropertyChanged([NotNull] AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var behavior = (TextBoxPropertyValueValidationBehavior)d;
            var adorner = behavior.adorner;

//             var previousStoryboard = e.OldValue as Storyboard;
//             if (previousStoryboard != null && adorner != null)
//             {
//                 previousStoryboard.Remove(adorner);
//             }

//             var newStoryboard = e.NewValue as Storyboard;
//             if (newStoryboard != null && adorner != null)
//             {
//                 Storyboard.SetTarget(behavior.AdornerStoryboard, adorner);
//             }
        }

        private static void BorderPropertyChanged([NotNull] AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var behavior = (TextBoxPropertyValueValidationBehavior)d;
            var adorner = behavior.adorner;
            if (adorner != null)
            {
                if (e.Property == BorderBrushProperty)
                    adorner.BorderBrush = behavior.BorderBrush;

                if (e.Property == BorderCornerRadiusProperty)
                    adorner.BorderCornerRadius = behavior.BorderCornerRadius;

                if (e.Property == BorderThicknessProperty)
                    adorner.BorderThickness = behavior.BorderThickness;
            }
        }
    }
}
