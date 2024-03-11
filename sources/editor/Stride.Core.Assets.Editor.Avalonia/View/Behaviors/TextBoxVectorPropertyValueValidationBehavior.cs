// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System.Linq;
using Avalonia;
using Avalonia.Controls;

using Avalonia.Controls.Documents;
using Avalonia.Media;
using Avalonia.Animation;
using Avalonia.Xaml.Interactivity;
using Stride.Core.Annotations;
using Stride.Core.Extensions;
using Stride.Core.Presentation.Adorners;
using Stride.Core.Presentation.Controls;
using Stride.Core.Presentation.Core;
using Stride.Core.Presentation.Extensions;
using Avalonia.Interactivity;
using System.Linq;
using Avalonia.Animation;

namespace Stride.Core.Assets.Editor.View.Behaviors
{
    public class TextBoxVectorPropertyValueValidationBehavior : Behavior<VectorEditorBase>
    {
		static TextBoxVectorPropertyValueValidationBehavior()
		{
// 			AdornerStoryboardProperty.Changed.AddClassHandler<TextBoxVectorPropertyValueValidationBehavior>(AdornerStoryboardPropertyChanged);
			BorderBrushProperty.Changed.AddClassHandler<TextBoxVectorPropertyValueValidationBehavior>(BorderPropertyChanged);
			BorderCornerRadiusProperty.Changed.AddClassHandler<TextBoxVectorPropertyValueValidationBehavior>(BorderPropertyChanged);
			BorderThicknessProperty.Changed.AddClassHandler<TextBoxVectorPropertyValueValidationBehavior>(BorderPropertyChanged);
		}

        private TextBoxAndAdorner[] textBoxAndAdorners;

        /// <summary>
        /// Identifies the <see cref="AdornerStoryboard"/> dependency property.
        /// </summary>
//         public static readonly StyledProperty<Storyboard> AdornerStoryboardProperty = StyledProperty<Storyboard>.Register<TextBoxVectorPropertyValueValidationBehavior, Storyboard>(nameof(AdornerStoryboard), null); // T5
        /// <summary>
        /// Identifies the <see cref="BorderBrush"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<IBrush> BorderBrushProperty = StyledProperty<IBrush>.Register<TextBoxVectorPropertyValueValidationBehavior, IBrush>(nameof(BorderBrush), Brushes.IndianRed); // T5
        /// <summary>
        /// Identifies the <see cref="BorderCornerRadius"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> BorderCornerRadiusProperty = StyledProperty<double>.Register<TextBoxVectorPropertyValueValidationBehavior, double>(nameof(BorderCornerRadius), 3.0); // T5
        /// <summary>
        /// Identifies the <see cref="BorderThickness"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> BorderThicknessProperty = StyledProperty<double>.Register<TextBoxVectorPropertyValueValidationBehavior, double>(nameof(BorderThickness), 2.0); // T5

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
            AssociatedObject.Loaded += OnAssociatedObjectLoaded;

            base.OnAttached();
        }

        private void OnAssociatedObjectLoaded(object sender, RoutedEventArgs e)
        {
            var textBoxes = global::Avalonia.VisualTree.VisualExtensions.GetVisualChildren(AssociatedObject).Where (x => x is TextBoxBase).Select (x => (TextBoxBase) x);
            textBoxes.ForEach(x =>
            {
                x.Validating += OnValidating;
                x.TextToSourceValueConversionFailed += OnTextToSourceValueConversionFailed;
            });

//             var adornerLayer = AdornerLayer.GetAdornerLayer(AssociatedObject);
//             if (adornerLayer != null)
//             {
//                 textBoxAndAdorners = textBoxes.Select(textBox =>
//                 {
//                     var adorner = new HighlightBorderAdorner(textBox)
//                     {
//                         BackgroundBrush = null,
//                         BorderBrush = BorderBrush,
//                         BorderCornerRadius = BorderCornerRadius,
//                         BorderThickness = BorderThickness,
//                         State = HighlightAdornerState.Hidden,
//                     };
//                     adornerLayer.Add(adorner);
//                     return new TextBoxAndAdorner(textBox, adorner);
//                 }).ToArray();
//             }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.Loaded -= OnAssociatedObjectLoaded;
            var textBoxes = global::Avalonia.VisualTree.VisualExtensions.GetVisualChildren(AssociatedObject).Where (x => x is TextBoxBase).Select (x => (TextBoxBase) x);
            textBoxes.ForEach(x =>
            {
                x.Validating -= OnValidating;
                x.TextToSourceValueConversionFailed -= OnTextToSourceValueConversionFailed;
            });

            if (textBoxAndAdorners != null)
            {
//                 if (AdornerStoryboard != null)
//                 {
//                     textBoxAndAdorners.ForEach(tba => AdornerStoryboard.Remove(tba.Adorner));
//                 }
//                 textBoxAndAdorners.ForEach(tba => AdornerLayer.GetAdornerLayer(tba.TextBox)?.Remove(tba.Adorner));
                textBoxAndAdorners = null;
            }
        }

        private void OnValidating(object sender, CancelRoutedEventArgs e)
        {
            if (textBoxAndAdorners != null)
            {
                var adorner = textBoxAndAdorners.FirstOrDefault(x => x.TextBox == sender).Adorner;
                if (adorner != null)
                {
                    adorner.State = HighlightAdornerState.Hidden;
                }
            }
        }

        private void OnTextToSourceValueConversionFailed(object sender, RoutedEventArgs e)
        {
//             if (textBoxAndAdorners != null && AdornerStoryboard != null)
//             {
//                 var adorner = textBoxAndAdorners.FirstOrDefault(x => x.TextBox == sender).Adorner;
//                 if (adorner != null)
//                 {
//                     adorner.State = HighlightAdornerState.Visible;
//                     // Show visual indicator it has failed.
//                     AdornerStoryboard.Begin(adorner);
//                 }
//             }
        }

        private static void AdornerStoryboardPropertyChanged([NotNull] AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var behavior = (TextBoxVectorPropertyValueValidationBehavior)d;
            var textBoxAndAdorners = behavior.textBoxAndAdorners;
            if (textBoxAndAdorners != null)
            {
                foreach (var tba in textBoxAndAdorners)
                {
//                     var previousStoryboard = e.OldValue as Storyboard;
//                     if (previousStoryboard != null && tba.Adorner != null)
//                     {
//                         previousStoryboard.Remove(tba.Adorner);
//                     }
// 
//                     var newStoryboard = e.NewValue as Storyboard;
//                     if (newStoryboard != null && tba.Adorner != null)
//                     {
//                         Storyboard.SetTarget(behavior.AdornerStoryboard, tba.Adorner);
//                     }
                }
            }
        }

        private static void BorderPropertyChanged([NotNull] AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var behavior = (TextBoxVectorPropertyValueValidationBehavior)d;
            var textBoxAndAdorners = behavior.textBoxAndAdorners;
            if (textBoxAndAdorners != null)
            {
                foreach (var tba in textBoxAndAdorners)
                {
                    if (e.Property == BorderBrushProperty)
                        tba.Adorner.BorderBrush = behavior.BorderBrush;

                    if (e.Property == BorderCornerRadiusProperty)
                        tba.Adorner.BorderCornerRadius = behavior.BorderCornerRadius;

                    if (e.Property == BorderThicknessProperty)
                        tba.Adorner.BorderThickness = behavior.BorderThickness;
                }
            }
        }

        private readonly struct TextBoxAndAdorner
        {
            public readonly TextBoxBase TextBox;
            public readonly HighlightBorderAdorner Adorner;

            public TextBoxAndAdorner(TextBoxBase textBox, HighlightBorderAdorner adorner)
            {
                TextBox = textBox;
                Adorner = adorner;
            }
        }
    }
}
