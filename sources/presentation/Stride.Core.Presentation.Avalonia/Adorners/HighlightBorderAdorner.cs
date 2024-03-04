// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia;
using Avalonia.Controls;

using Avalonia.Controls.Documents;
using Avalonia.Media;
using Stride.Core.Annotations;
using Avalonia.Rendering.Composition;

namespace Stride.Core.Presentation.Adorners
{
    /// <summary>
    /// An adorner that draw a rectangle with borders over the adorned element. It can multiple possible states: Hidden, Visible, HighlightAccept and HighlightRefuse.
    /// </summary>
    public class HighlightBorderAdorner : Control
    {
		static HighlightBorderAdorner()
		{
			AcceptBorderBrushProperty.Changed.AddClassHandler<HighlightBorderAdorner>(PropertyChanged);
			AcceptBorderThicknessProperty.Changed.AddClassHandler<HighlightBorderAdorner>(PropertyChanged);
			AcceptBorderCornerRadiusProperty.Changed.AddClassHandler<HighlightBorderAdorner>(PropertyChanged);
			AcceptBackgroundBrushProperty.Changed.AddClassHandler<HighlightBorderAdorner>(PropertyChanged);
			AcceptBackgroundOpacityProperty.Changed.AddClassHandler<HighlightBorderAdorner>(PropertyChanged);
			RefuseBorderBrushProperty.Changed.AddClassHandler<HighlightBorderAdorner>(PropertyChanged);
			RefuseBorderThicknessProperty.Changed.AddClassHandler<HighlightBorderAdorner>(PropertyChanged);
			RefuseBorderCornerRadiusProperty.Changed.AddClassHandler<HighlightBorderAdorner>(PropertyChanged);
			RefuseBackgroundBrushProperty.Changed.AddClassHandler<HighlightBorderAdorner>(PropertyChanged);
			RefuseBackgroundOpacityProperty.Changed.AddClassHandler<HighlightBorderAdorner>(PropertyChanged);
			BorderBrushProperty.Changed.AddClassHandler<HighlightBorderAdorner>(PropertyChanged);
			BorderThicknessProperty.Changed.AddClassHandler<HighlightBorderAdorner>(PropertyChanged);
			BorderCornerRadiusProperty.Changed.AddClassHandler<HighlightBorderAdorner>(PropertyChanged);
			BackgroundBrushProperty.Changed.AddClassHandler<HighlightBorderAdorner>(PropertyChanged);
			BackgroundOpacityProperty.Changed.AddClassHandler<HighlightBorderAdorner>(PropertyChanged);
			StateProperty.Changed.AddClassHandler<HighlightBorderAdorner>(PropertyChanged);
		}

        /// <summary>
        /// Identifies the <see cref="AcceptBorderBrush"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<IBrush> AcceptBorderBrushProperty = StyledProperty<IBrush>.Register<HighlightBorderAdorner, IBrush>("AcceptBorderBrush", Brushes.PaleGreen);

        /// <summary>
        /// Identifies the <see cref="AcceptBorderBrush"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> AcceptBorderThicknessProperty = StyledProperty<double>.Register<HighlightBorderAdorner, double>("AcceptBorderThickness", 2.0);

        /// <summary>
        /// Identifies the <see cref="AcceptBorderBrush"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> AcceptBorderCornerRadiusProperty = StyledProperty<double>.Register<HighlightBorderAdorner, double>("AcceptBorderCornerRadius", 3.0);

        /// <summary>
        /// Identifies the <see cref="AcceptBorderBrush"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<IBrush> AcceptBackgroundBrushProperty = StyledProperty<IBrush>.Register<HighlightBorderAdorner, IBrush>("AcceptBackgroundBrush", Brushes.MediumSeaGreen);

        /// <summary>
        /// Identifies the <see cref="AcceptBorderBrush"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> AcceptBackgroundOpacityProperty = StyledProperty<double>.Register<HighlightBorderAdorner, double>("AcceptBackgroundOpacity", 0.3);

        /// <summary>
        /// Identifies the <see cref="RefuseBorderBrush"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<IBrush> RefuseBorderBrushProperty = StyledProperty<IBrush>.Register<HighlightBorderAdorner, IBrush>("RefuseBorderBrush", Brushes.Red);

        /// <summary>
        /// Identifies the <see cref="RefuseBorderBrush"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> RefuseBorderThicknessProperty = StyledProperty<double>.Register<HighlightBorderAdorner, double>("RefuseBorderThickness", 2.0);

        /// <summary>
        /// Identifies the <see cref="RefuseBorderBrush"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> RefuseBorderCornerRadiusProperty = StyledProperty<double>.Register<HighlightBorderAdorner, double>("RefuseBorderCornerRadius", 3.0);

        /// <summary>
        /// Identifies the <see cref="RefuseBorderBrush"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<IBrush> RefuseBackgroundBrushProperty = StyledProperty<IBrush>.Register<HighlightBorderAdorner, IBrush>("RefuseBackgroundBrush", Brushes.IndianRed);

        /// <summary>
        /// Identifies the <see cref="RefuseBorderBrush"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> RefuseBackgroundOpacityProperty = StyledProperty<double>.Register<HighlightBorderAdorner, double>("RefuseBackgroundOpacity", 0.3);

        /// <summary>
        /// Identifies the <see cref="BorderBrush"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<IBrush> BorderBrushProperty = StyledProperty<IBrush>.Register<HighlightBorderAdorner, IBrush>("BorderBrush", Brushes.SteelBlue);

        /// <summary>
        /// Identifies the <see cref="BorderThickness"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> BorderThicknessProperty = StyledProperty<double>.Register<HighlightBorderAdorner, double>("BorderThickness", 2.0);

        /// <summary>
        /// Identifies the <see cref="BorderCornerRadius"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> BorderCornerRadiusProperty = StyledProperty<double>.Register<HighlightBorderAdorner, double>("BorderCornerRadius", 3.0);

        /// <summary>
        /// Identifies the <see cref="BackgroundBrush"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<IBrush> BackgroundBrushProperty = StyledProperty<IBrush>.Register<HighlightBorderAdorner, IBrush>("BackgroundBrush", Brushes.LightSteelBlue);

        /// <summary>
        /// Identifies the <see cref="BorderBrush"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> BackgroundOpacityProperty = StyledProperty<double>.Register<HighlightBorderAdorner, double>("BackgroundOpacity", 0.3);

        /// <summary>
        /// Identifies the <see cref="State"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<HighlightAdornerState> StateProperty = StyledProperty<HighlightAdornerState>.Register<HighlightBorderAdorner, HighlightAdornerState>("State", HighlightAdornerState.Hidden);

        /// <summary>
        /// Initializes a new instance of the <see cref="HighlightBorderAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement"></param>
        public HighlightBorderAdorner([NotNull] Control adornedElement)
//             : base(adornedElement)
        {
        }

        /// <summary>
        /// Gets or sets the border brush when the adorner is Accepted.
        /// </summary>
        public Brush AcceptBorderBrush { get { return (Brush)GetValue(AcceptBorderBrushProperty); } set { SetValue(AcceptBorderBrushProperty, value); } }

        /// <summary>
        /// Gets or sets the border thickness when the adorner is Accepted.
        /// </summary>
        public double AcceptBorderThickness { get { return (double)GetValue(AcceptBorderThicknessProperty); } set { SetValue(AcceptBorderThicknessProperty, value); } }

        /// <summary>
        /// Gets or sets the border corner radius when the adorner is Accepted.
        /// </summary>
        public double AcceptBorderCornerRadius { get { return (double)GetValue(AcceptBorderCornerRadiusProperty); } set { SetValue(AcceptBorderCornerRadiusProperty, value); } }

        /// <summary>
        /// Gets or sets the background brush when the adorner is Accepted.
        /// </summary>
        public Brush AcceptBackgroundBrush { get { return (Brush)GetValue(AcceptBackgroundBrushProperty); } set { SetValue(AcceptBackgroundBrushProperty, value); } }

        /// <summary>
        /// Gets or sets the background opacity when the adorner is Accepted.
        /// </summary>
        public double AcceptBackgroundOpacity { get { return (double)GetValue(AcceptBackgroundOpacityProperty); } set { SetValue(AcceptBackgroundOpacityProperty, value); } }

        /// <summary>
        /// Gets or sets the border brush when the adorner is Refuseed.
        /// </summary>
        public Brush RefuseBorderBrush { get { return (Brush)GetValue(RefuseBorderBrushProperty); } set { SetValue(RefuseBorderBrushProperty, value); } }

        /// <summary>
        /// Gets or sets the border thickness when the adorner is Refuseed.
        /// </summary>
        public double RefuseBorderThickness { get { return (double)GetValue(RefuseBorderThicknessProperty); } set { SetValue(RefuseBorderThicknessProperty, value); } }

        /// <summary>
        /// Gets or sets the border corner radius when the adorner is Refuseed.
        /// </summary>
        public double RefuseBorderCornerRadius { get { return (double)GetValue(RefuseBorderCornerRadiusProperty); } set { SetValue(RefuseBorderCornerRadiusProperty, value); } }

        /// <summary>
        /// Gets or sets the background brush when the adorner is Refuseed.
        /// </summary>
        public Brush RefuseBackgroundBrush { get { return (Brush)GetValue(RefuseBackgroundBrushProperty); } set { SetValue(RefuseBackgroundBrushProperty, value); } }

        /// <summary>
        /// Gets or sets the background opacity when the adorner is Refuseed.
        /// </summary>
        public double RefuseBackgroundOpacity { get { return (double)GetValue(RefuseBackgroundOpacityProperty); } set { SetValue(RefuseBackgroundOpacityProperty, value); } }

        /// <summary>
        /// Gets or sets the border brush when the adorner visible but not highlighted.
        /// </summary>
        public Brush BorderBrush { get { return (Brush)GetValue(BorderBrushProperty); } set { SetValue(BorderBrushProperty, value); } }

        /// <summary>
        /// Gets or sets the border thickness when the adorner is visible but not highlighted.
        /// </summary>
        public double BorderThickness { get { return (double)GetValue(BorderThicknessProperty); } set { SetValue(BorderThicknessProperty, value); } }

        /// <summary>
        /// Gets or sets the border corner radius when the adorner is visible but not highlighted.
        /// </summary>
        public double BorderCornerRadius { get { return (double)GetValue(BorderCornerRadiusProperty); } set { SetValue(BorderCornerRadiusProperty, value); } }

        /// <summary>
        /// Gets or sets the background brush when the adorner is visible but not highlighted.
        /// </summary>
        public Brush BackgroundBrush { get { return (Brush)GetValue(BackgroundBrushProperty); } set { SetValue(BackgroundBrushProperty, value); } }

        /// <summary>
        /// Gets or sets the background opacity when the adorner is visible but not highlighted.
        /// </summary>
        public double BackgroundOpacity { get { return (double)GetValue(BackgroundOpacityProperty); } set { SetValue(BackgroundOpacityProperty, value); } }
        
        /// <summary>
        /// Gets or sets the state of the adorner.
        /// </summary>
        public HighlightAdornerState State { get { return (HighlightAdornerState)GetValue(StateProperty); } set { SetValue(StateProperty, value); } }

        /// <inheritdoc/>
//         public override void OnRender(ImmediateDrawingContext drawingContext)
//         {
//             var adornedElementRect = new Rect(AdornedElement.RenderSize);
//             Brush renderBrush = null;
//             Pen renderPen = null;
//             switch (State)
//             {
//                 case HighlightAdornerState.HighlightAccept:
//                     if (AcceptBackgroundBrush != null)
//                     {
//                         renderBrush = AcceptBackgroundBrush.Clone();
//                         renderBrush.Opacity = AcceptBackgroundOpacity;
//                     }
//                     if (AcceptBorderBrush != null)
//                     {
//                         renderPen = new Pen(AcceptBorderBrush, AcceptBorderThickness);
//                     }
//                     drawingContext.DrawRoundedRectangle(renderBrush, renderPen, adornedElementRect, AcceptBorderCornerRadius, AcceptBorderCornerRadius);
//                     break;
// 
//                 case HighlightAdornerState.HighlightRefuse:
//                     if (RefuseBackgroundBrush != null)
//                     {
//                         renderBrush = RefuseBackgroundBrush.Clone();
//                         renderBrush.Opacity = RefuseBackgroundOpacity;
//                     }
//                     if (RefuseBorderBrush != null)
//                     {
//                         renderPen = new Pen(RefuseBorderBrush, RefuseBorderThickness);
//                     }
//                     drawingContext.DrawRoundedRectangle(renderBrush, renderPen, adornedElementRect, RefuseBorderCornerRadius, RefuseBorderCornerRadius);
//                     break;
// 
//                 case HighlightAdornerState.Visible:
//                     if (BackgroundBrush != null)
//                     {
//                         renderBrush = BackgroundBrush.Clone();
//                         renderBrush.Opacity = BackgroundOpacity;
//                     }
//                     if (BorderBrush != null)
//                     {
//                         renderPen = new Pen(BorderBrush, BorderThickness);
//                     }
//                     drawingContext.DrawRoundedRectangle(renderBrush, renderPen, adornedElementRect, BorderCornerRadius, BorderCornerRadius);
//                     break;
//             }
//         }

        private static void PropertyChanged([NotNull] AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
//             var adorner = (Adorner)d;
//             adorner.InvalidateVisual();
        }
    }
}
