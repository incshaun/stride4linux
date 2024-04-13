// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;

using Avalonia.Input;
using Avalonia.Media;
using Stride.Core.Annotations;

using Avalonia.Interactivity;
using System;
using Avalonia.Media;

namespace Stride.Core.Presentation.Controls
{
    public delegate void CustomRenderRoutedEventHandler(object sender, CustomRenderRoutedEventArgs e);

    public class CustomRenderRoutedEventArgs : RoutedEventArgs
    {
        public DrawingContext DrawingContext { get; private set; }

        public CustomRenderRoutedEventArgs(RoutedEvent routedEvent, DrawingContext drawingContext)
        {
            RoutedEvent = routedEvent;
            DrawingContext = drawingContext;
        }
    }

    public delegate void RoutedDependencyPropertyChangedEventHandler(object sender, RoutedDependencyPropertyChangedEventArgs e);

    public class RoutedDependencyPropertyChangedEventArgs : RoutedEventArgs
    {
        public object OldValue { get; private set; }
        public object NewValue { get; private set; }
        public AvaloniaProperty DependencyProperty { get; private set; }

        public RoutedDependencyPropertyChangedEventArgs(RoutedEvent routedEvent, AvaloniaProperty dependencyProperty, object oldValue, object newValue)
        {
            RoutedEvent = routedEvent;
            DependencyProperty = dependencyProperty;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    public class ScaleBar : Control
    {
        public static readonly StyledProperty<DrawingContext> CustomDrawingContextProperty = StyledProperty<DrawingContext>.Register<ScaleBar, DrawingContext>("CustomDrawingContext", null); // T8

        public static readonly StyledProperty<IBrush> BackgroundProperty = StyledProperty<IBrush>.Register<ScaleBar, IBrush>("Background", Brushes.Transparent); // T8

        public static readonly StyledProperty<Pen> LargeTickPenProperty = StyledProperty<Pen>.Register<ScaleBar, Pen>("LargeTickPen", new Pen(Brushes.Black,  1.0)); // T7A

        public static readonly StyledProperty<Pen> SmallTickPenProperty = StyledProperty<Pen>.Register<ScaleBar, Pen>("SmallTickPen", new Pen(Brushes.Gray,  1.0)); // T7A

        public static readonly StyledProperty<double> LargeTickTopProperty = StyledProperty<double>.Register<ScaleBar, double>("LargeTickTop", 0.625); // T8

        public static readonly StyledProperty<double> LargeTickBottomProperty = StyledProperty<double>.Register<ScaleBar, double>("LargeTickBottom", 1.0); // T8

        public static readonly StyledProperty<double> SmallTickTopProperty = StyledProperty<double>.Register<ScaleBar, double>("SmallTickTop", 0.75); // T8

        public static readonly StyledProperty<double> SmallTickBottomProperty = StyledProperty<double>.Register<ScaleBar, double>("SmallTickBottom", 1.0); // T8

        public static readonly StyledProperty<int> DecimalCountRoundingProperty = StyledProperty<int>.Register<ScaleBar, int>("DecimalCountRounding", 6, coerce : CoerceDecimalCountRoundingPropertyValue); // T9E

        public static readonly StyledProperty<Point> TextPositionOriginProperty = StyledProperty<Point>.Register<ScaleBar, Point>("TextPositionOrigin", new Point(0.5,  0.0)); // T7A

        public static readonly StyledProperty<double> TextPositionProperty = StyledProperty<double>.Register<ScaleBar, double>("TextPosition", 0.0); // T8

        public static readonly StyledProperty<IBrush> ForegroundProperty = StyledProperty<IBrush>.Register<ScaleBar, IBrush>("Foreground", Brushes.Black); // T8

        public static readonly StyledProperty<Typeface> FontProperty = StyledProperty<Typeface>.Register<ScaleBar, Typeface>("Font", new Typeface("Meiryo")); // T7B

        public static readonly StyledProperty<double> FontSizeProperty = StyledProperty<double>.Register<ScaleBar, double>("FontSize", 9.0); // T8

        public static readonly StyledProperty<double> StartUnitProperty = StyledProperty<double>.Register<ScaleBar, double>("StartUnit", 0.0); // T8

        public static readonly StyledProperty<double> MinimumUnitsPerTickProperty = StyledProperty<double>.Register<ScaleBar, double>("MinimumUnitsPerTick", 1e-12); // T9E

        public static readonly StyledProperty<double> MaximumUnitsPerTickProperty = StyledProperty<double>.Register<ScaleBar, double>("MaximumUnitsPerTick", 1e12); // T9E

        public static readonly StyledProperty<double> UnitsPerTickProperty = StyledProperty<double>.Register<ScaleBar, double>("UnitsPerTick", 1.0, coerce: CoerceUnitsPerTickPropertyValue); // T9C

        public static readonly StyledProperty<double> PixelsPerTickProperty = StyledProperty<double>.Register<ScaleBar, double>("PixelsPerTick", 100.0, coerce: CoercePixelsPerTickPropertyValue); // T9C

        private static readonly DirectProperty<ScaleBar, double> AdjustedUnitsPerTickPropertyKey = AvaloniaProperty.RegisterDirect<ScaleBar, double>(nameof (AdjustedUnitsPerTick), o => o.AdjustedUnitsPerTick); // T10H2

        private static readonly DirectProperty<ScaleBar, double> AdjustedPixelsPerTickPropertyKey = AvaloniaProperty.RegisterDirect<ScaleBar, double>(nameof (AdjustedPixelsPerTick), o => o.AdjustedPixelsPerTick); // T10H2

        private static readonly DirectProperty<ScaleBar, double> PixelsPerUnitPropertyKey = AvaloniaProperty.RegisterDirect<ScaleBar, double>(nameof (PixelsPerUnit), o => o.PixelsPerUnit); // T10H2

        private static readonly DirectProperty<ScaleBar, double> AdjustedPixelsPerUnitPropertyKey = AvaloniaProperty.RegisterDirect<ScaleBar, double>(nameof (AdjustedPixelsPerUnit), o => o.AdjustedPixelsPerUnit); // T10H2

        public static readonly StyledProperty<bool> IsAliasedProperty = StyledProperty<bool>.Register<ScaleBar, bool>("IsAliased", true); // T9E

        public static readonly StyledProperty<bool> IsTextVisibleProperty = StyledProperty<bool>.Register<ScaleBar, bool>("IsTextVisible", true); // T8

        public static readonly StyledProperty<bool> IsSmallTickVisibleProperty = StyledProperty<bool>.Register<ScaleBar, bool>("IsSmallTickVisible", true); // T8

        public static readonly StyledProperty<bool> IsZoomingOnMouseWheelProperty = StyledProperty<bool>.Register<ScaleBar, bool>("IsZoomingOnMouseWheel", false); // T8

        public static readonly StyledProperty<double> MouseWheelZoomCoeficientProperty = StyledProperty<double>.Register<ScaleBar, double>("MouseWheelZoomCoeficient", 1.1); // T8

        public static readonly StyledProperty<bool> IsDraggingOnLeftMouseButtonProperty = StyledProperty<bool>.Register<ScaleBar, bool>("IsDraggingOnLeftMouseButton", false); // T8

        public static readonly StyledProperty<bool> IsDraggingOnRightMouseButtonProperty = StyledProperty<bool>.Register<ScaleBar, bool>("IsDraggingOnRightMouseButton", false); // T8

        public static readonly StyledProperty<UnitSystem> UnitSystemProperty = StyledProperty<UnitSystem>.Register<ScaleBar, UnitSystem>("UnitSystem", null); // T9E

        public static readonly StyledProperty<string> SymbolProperty = StyledProperty<string>.Register<ScaleBar, string>("UnitSymbol", null); // T8

        public static readonly StyledProperty<double> TickTextUnitDividerProperty = StyledProperty<double>.Register<ScaleBar, double>("TickTextUnitDivider", 1.0); // T8
    
        public static readonly RoutedEvent BeforeRenderEvent = RoutedEvent.Register<ScaleBar, RoutedEventArgs>("BeforeRender", RoutingStrategies.Bubble); // T21

        public static readonly RoutedEvent AfterRenderEvent = RoutedEvent.Register<ScaleBar, RoutedEventArgs>("AfterRender", RoutingStrategies.Bubble); // T21

        public static readonly RoutedEvent BeforeTicksRenderEvent = RoutedEvent.Register<ScaleBar, RoutedEventArgs>("BeforeTicksRender", RoutingStrategies.Bubble); // T21

        public static readonly RoutedEvent AfterTicksRenderEvent = RoutedEvent.Register<ScaleBar, RoutedEventArgs>("AfterTicksRender", RoutingStrategies.Bubble); // T21

        public static readonly RoutedEvent ScaleChangingEvent = RoutedEvent.Register<ScaleBar, RoutedEventArgs>("ScaleChanging", RoutingStrategies.Bubble); // T21

        public static readonly RoutedEvent ScaleChangedEvent = RoutedEvent.Register<ScaleBar, RoutedEventArgs>("ScaleChanged", RoutingStrategies.Bubble); // T21

        static ScaleBar()
		{
			MinimumUnitsPerTickProperty.Changed.AddClassHandler<ScaleBar>(OnUnitsPerTickPropertyChanged);
			MaximumUnitsPerTickProperty.Changed.AddClassHandler<ScaleBar>(OnUnitsPerTickPropertyChanged);
			IsAliasedProperty.Changed.AddClassHandler<ScaleBar>(OnIsAliasedPropertyChanged);
			UnitSystemProperty.Changed.AddClassHandler<ScaleBar>(OnUnitSystemPropertyChanged);
			UnitsPerTickProperty.Changed.AddClassHandler<ScaleBar>(OnUnitsPerTickPropertyChanged);
			PixelsPerTickProperty.Changed.AddClassHandler<ScaleBar>(OnPixelsPerTickPropertyChanged);
		}

		public ScaleBar()
		{
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            SetValue(AdjustedUnitsPerTickPropertyKey, UnitsPerTick);
            SetValue(AdjustedPixelsPerTickPropertyKey, PixelsPerTick);
            RenderOptions.SetEdgeMode(this, IsAliased ? EdgeMode.Aliased : EdgeMode.Unspecified);
            InvalidateVisual();
        }

        public event EventHandler<RoutedEventArgs> BeforeRender
        {
            add { AddHandler(BeforeRenderEvent, value); }
            remove { RemoveHandler(BeforeRenderEvent, value); }
        }

        public event EventHandler<RoutedEventArgs> AfterRender
        {
            add { AddHandler(AfterRenderEvent, value); }
            remove { RemoveHandler(AfterRenderEvent, value); }
        }

        public event CustomRenderRoutedEventHandler BeforeTicksRender
        {
            add { AddHandler(BeforeTicksRenderEvent, value); }
            remove { RemoveHandler(BeforeTicksRenderEvent, value); }
        }

        public event CustomRenderRoutedEventHandler AfterTicksRender
        {
            add { AddHandler(AfterTicksRenderEvent, value); }
            remove { RemoveHandler(AfterTicksRenderEvent, value); }
        }

        public event RoutedDependencyPropertyChangedEventHandler ScaleChanging
        {
            add { AddHandler(ScaleChangingEvent, value); }
            remove { RemoveHandler(ScaleChangingEvent, value); }
        }

        public event RoutedDependencyPropertyChangedEventHandler ScaleChanged
        {
            add { AddHandler(ScaleChangedEvent, value); }
            remove { RemoveHandler(ScaleChangedEvent, value); }
        }

        private void RaiseBeforeRenderEvent()
        {
            RaiseEvent(new RoutedEventArgs(BeforeRenderEvent));
        }

        private void RaiseAfterRenderEvent()
        {
            RaiseEvent(new RoutedEventArgs(AfterRenderEvent));
        }

        private void RaiseBeforeTicksRenderEvent(DrawingContext drawingContext)
        {
            RaiseEvent(new CustomRenderRoutedEventArgs(BeforeTicksRenderEvent, drawingContext));
        }

        private void RaiseAfterTicksRenderEvent(DrawingContext drawingContext)
        {
            RaiseEvent(new CustomRenderRoutedEventArgs(AfterTicksRenderEvent, drawingContext));
        }

        private void RaiseScaleChangingEvent(AvaloniaProperty dependencyProperty, object oldValue, object newValue)
        {
            RaiseEvent(new RoutedDependencyPropertyChangedEventArgs(ScaleChangingEvent, dependencyProperty, oldValue, newValue));
        }

        private void RaiseScaleChangedEvent(AvaloniaProperty dependencyProperty, object oldValue, object newValue)
        {
            RaiseEvent(new RoutedDependencyPropertyChangedEventArgs(ScaleChangedEvent, dependencyProperty, oldValue, newValue));
        }

        private void SetScaleChangingProperty([NotNull] AvaloniaProperty dependencyProperty, object value)
        {
            var oldValue = GetValue(dependencyProperty);
            RaiseScaleChangingEvent(dependencyProperty, oldValue, value);
            SetValue(dependencyProperty, value);
            RaiseScaleChangedEvent(dependencyProperty, oldValue, value);
        }

        private void SetScaleChangingProperty([NotNull] AvaloniaProperty dependencyPropertyKey, [NotNull] AvaloniaProperty dependencyProperty, object value)
        {
            var oldValue = GetValue(dependencyProperty);
            RaiseScaleChangingEvent(dependencyProperty, oldValue, value);
            SetValue(dependencyPropertyKey, value);
            RaiseScaleChangedEvent(dependencyProperty, oldValue, value);
        }

        public DrawingContext CustomDrawingContext
        {
            get { return (DrawingContext)GetValue(CustomDrawingContextProperty); }
            set { SetValue(CustomDrawingContextProperty, value); }
        }

        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        public Pen LargeTickPen
        {
            get { return (Pen)GetValue(LargeTickPenProperty); }
            set { SetValue(LargeTickPenProperty, value); }
        }

        public Pen SmallTickPen
        {
            get { return (Pen)GetValue(SmallTickPenProperty); }
            set { SetValue(SmallTickPenProperty, value); }
        }

        /// <summary>
        /// Gets or sets the relative top (Y) coordinate of the drawn large ticks. This is a dependency property.
        /// </summary>
        /// <remarks>The coordinate is relative, that means 0.0 is top and 1.0 is bottom.
        /// The coordinate can be set to less than 0.0 or more than 1.0 where additional offset is needed.</remarks>
        public double LargeTickTop
        {
            get { return (double)GetValue(LargeTickTopProperty); }
            set { SetValue(LargeTickTopProperty, value); }
        }

        /// <summary>
        /// Gets or sets the relative bottom (Y) coordinate of the drawn large ticks. This is a dependency property.
        /// </summary>
        /// <remarks>The coordinate is relative, that means 0.0 is top and 1.0 is bottom.
        /// The coordinate can be set to less than 0.0 or more than 1.0 where additional offset is needed.</remarks>
        public double LargeTickBottom
        {
            get { return (double)GetValue(LargeTickBottomProperty); }
            set { SetValue(LargeTickBottomProperty, value); }
        }

        /// <summary>
        /// Gets or sets the relative top (Y) coordinate of the drawn small ticks. This is a dependency property.
        /// </summary>
        /// <remarks>The coordinate is relative, that means 0.0 is top and 1.0 is bottom.
        /// The coordinate can be set to less than 0.0 or more than 1.0 where additional offset is needed.</remarks>
        public double SmallTickTop
        {
            get { return (double)GetValue(SmallTickTopProperty); }
            set { SetValue(SmallTickTopProperty, value); }
        }

        /// <summary>
        /// Gets or sets the relative bottom (Y) coordinate of the drawn small ticks. This is a dependency property.
        /// </summary>
        /// <remarks>The coordinate is relative, that means 0.0 is top and 1.0 is bottom.
        /// The coordinate can be set to less than 0.0 or more than 1.0 where additional offset is needed.</remarks>
        public double SmallTickBottom
        {
            get { return (double)GetValue(SmallTickBottomProperty); }
            set { SetValue(SmallTickBottomProperty, value); }
        }

        public int DecimalCountRounding
        {
            get { return (int)GetValue(DecimalCountRoundingProperty); }
            set { SetValue(DecimalCountRoundingProperty, value); }
        }

        /// <summary>
        /// Gets or sets the center point of drawn text, relative to the bounds of the drawn text itself. This is a dependency property.
        /// </summary>
        /// <remarks>Each coordinate axis can be set to less than 0.0 or more than 1.0 where additional offset is needed.</remarks>
        public Point TextPositionOrigin
        {
            get { return (Point)GetValue(TextPositionOriginProperty); }
            set { SetValue(TextPositionOriginProperty, value); }
        }

        /// <summary>
        /// Gets or sets the relative top (Y) coordinate of the center of the drawn text. This is a dependency property.
        /// </summary>
        /// <remarks>The coordinate is relative, that means 0.0 is top and 1.0 is bottom.
        /// The coordinate can be set to less than 0.0 or more than 1.0 where additional offset is needed.</remarks>
        public double TextPosition
        {
            get { return (double)GetValue(TextPositionProperty); }
            set { SetValue(TextPositionProperty, value); }
        }

        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        public Typeface Font
        {
            get { return (Typeface)GetValue(FontProperty); }
            set { SetValue(FontProperty, value); }
        }

        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public double StartUnit
        {
            get { return (double)GetValue(StartUnitProperty); }
            set { SetScaleChangingProperty(StartUnitProperty, value); }
        }

        public double MinimumUnitsPerTick
        {
            get { return (double)GetValue(MinimumUnitsPerTickProperty); }
            set { SetScaleChangingProperty(MinimumUnitsPerTickProperty, value); }
        }

        public double MaximumUnitsPerTick
        {
            get { return (double)GetValue(MaximumUnitsPerTickProperty); }
            set { SetScaleChangingProperty(MaximumUnitsPerTickProperty, value); }
        }

        public double UnitsPerTick
        {
            get { return (double)GetValue(UnitsPerTickProperty); }
            set { SetScaleChangingProperty(UnitsPerTickProperty, value); }
        }

        private double _AdjustedUnitsPerTick;
		public double AdjustedUnitsPerTick { get { return _AdjustedUnitsPerTick; } set { SetAndRaise(AdjustedUnitsPerTickPropertyKey, ref _AdjustedUnitsPerTick, value); } }

        public double PixelsPerTick
        {
            get { return (double)GetValue(PixelsPerTickProperty); }
            set { SetScaleChangingProperty(PixelsPerTickProperty, value); }
        }

        private double _AdjustedPixelsPerTick;
		public double AdjustedPixelsPerTick { get { return _AdjustedPixelsPerTick; } set { SetAndRaise(AdjustedPixelsPerTickPropertyKey, ref _AdjustedPixelsPerTick, value); } }

		private double _PixelsPerUnit;
		public double PixelsPerUnit { get { return _PixelsPerUnit; } set { SetAndRaise(PixelsPerUnitPropertyKey, ref _PixelsPerUnit, value); } }

        private double _AdjustedPixelsPerUnit;
		public double AdjustedPixelsPerUnit { get { return _AdjustedPixelsPerUnit; } set { SetAndRaise(AdjustedPixelsPerUnitPropertyKey, ref _AdjustedPixelsPerUnit, value); } }

        public bool IsAliased
        {
            get { return (bool)GetValue(IsAliasedProperty); }
            set { SetValue(IsAliasedProperty, value); }
        }

        public bool IsTextVisible
        {
            get { return (bool)GetValue(IsTextVisibleProperty); }
            set { SetValue(IsTextVisibleProperty, value); }
        }

        public bool IsSmallTickVisible
        {
            get { return (bool)GetValue(IsSmallTickVisibleProperty); }
            set { SetValue(IsSmallTickVisibleProperty, value); }
        }

        public bool IsZoomingOnMouseWheel
        {
            get { return (bool)GetValue(IsZoomingOnMouseWheelProperty); }
            set { SetValue(IsZoomingOnMouseWheelProperty, value); }
        }

        public double MouseWheelZoomCoeficient
        {
            get { return (double)GetValue(MouseWheelZoomCoeficientProperty); }
            set { SetValue(MouseWheelZoomCoeficientProperty, value); }
        }

        public bool IsDraggingOnLeftMouseButton
        {
            get { return (bool)GetValue(IsDraggingOnLeftMouseButtonProperty); }
            set { SetValue(IsDraggingOnLeftMouseButtonProperty, value); }
        }

        public bool IsDraggingOnRightMouseButton
        {
            get { return (bool)GetValue(IsDraggingOnRightMouseButtonProperty); }
            set { SetValue(IsDraggingOnRightMouseButtonProperty, value); }
        }

        public UnitSystem UnitSystem
        {
            get { return (UnitSystem)GetValue(UnitSystemProperty); }
            set { SetValue(UnitSystemProperty, value); }
        }

        public string UnitSymbol
        {
            get { return (string)GetValue(SymbolProperty); }
            set { SetValue(SymbolProperty, value); }
        }

        public double TickTextUnitDivider
        {
            get { return (double)GetValue(TickTextUnitDividerProperty); }
            set { SetValue(TickTextUnitDividerProperty, value); }
        }

        private void UpdatePixelInfo()
        {
            AdjustedPixelsPerTick = PixelsPerTick * AdjustedUnitsPerTick / UnitsPerTick;
            AdjustedPixelsPerUnit = AdjustedPixelsPerTick / AdjustedUnitsPerTick;
            PixelsPerUnit = PixelsPerTick / UnitsPerTick;
        }

        private static void OnUnitSystemPropertyChanged(AvaloniaObject sender,AvaloniaPropertyChangedEventArgs e)
        {
            var scalebar = (ScaleBar)sender;
            scalebar.AdjustUnitIntervalWithUnitSystem(scalebar.UnitsPerTick);
            scalebar.UpdatePixelInfo();
        }

        private static void OnUnitsPerTickPropertyChanged(AvaloniaObject sender,AvaloniaPropertyChangedEventArgs e)
        {
            var scalebar = (ScaleBar)sender;
            scalebar.AdjustUnitIntervalWithUnitSystem((double)e.NewValue);
            scalebar.UpdatePixelInfo();
        }

        private static void OnPixelsPerTickPropertyChanged(AvaloniaObject sender,AvaloniaPropertyChangedEventArgs e)
        {
            var scalebar = (ScaleBar)sender;
            scalebar.UpdatePixelInfo();
        }

        private static void OnIsAliasedPropertyChanged([NotNull] AvaloniaObject sender,AvaloniaPropertyChangedEventArgs e)
        {
            RenderOptions.SetEdgeMode((Visual) sender, (bool)e.NewValue ? EdgeMode.Aliased : EdgeMode.Unspecified);
        }

        private static double CoerceUnitsPerTickPropertyValue(AvaloniaObject sender, double value)
        {
            var scalebar = (ScaleBar)sender;
            return scalebar.MinimumUnitsPerTick < scalebar.MaximumUnitsPerTick ? Math.Min(scalebar.MaximumUnitsPerTick, Math.Max(scalebar.MinimumUnitsPerTick, (double)value)) : value;
        }

        [NotNull]
        private static double CoercePixelsPerTickPropertyValue(AvaloniaObject sender, [NotNull] double value)
        {
            return Math.Max(10.0, (double)value);
        }

        [NotNull]
        private static int CoerceDecimalCountRoundingPropertyValue(AvaloniaObject sender, int value)
        {
            return Math.Max(0, 12);
        }

        public double GetPixelAt(double unit)
        {
            return ((unit - StartUnit) * AdjustedPixelsPerTick) / AdjustedUnitsPerTick;
        }

        public double GetUnitAt(double pixel)
        {
            return StartUnit + (pixel * AdjustedUnitsPerTick) / AdjustedPixelsPerTick;
        }

        public void SetUnitAt(double unit, double pixel)
        {
            StartUnit = unit - (pixel * AdjustedUnitsPerTick) / AdjustedPixelsPerTick;
            InvalidateVisual();
        }

        public void SetUnitsPerTickAt(double unitsPerTick, double pixel)
        {
            var unit = GetUnitAt(pixel);
            UnitsPerTick = unitsPerTick;
            SetUnitAt(unit, pixel);
        }

        public void SetPixelsPerTickAt(double pixelsPerTick, double pixel)
        {
            var unit = GetUnitAt(pixel);
            PixelsPerTick = pixelsPerTick;
            SetUnitAt(unit, pixel);
        }

        private Pen largeTickPen;
        private Pen smallTickPen;

        private Point textPositionOrigin;
        private double textPosition;
        private bool isTextVisible;
        private Brush foreground;
        private Typeface font;
        private double fontSize;

        private double actualWidth;
        private double actualHeight;

        private double largeTickTopPosition;
        private double largeTickBottomPosition;
        private double smallTickTopPosition;
        private double smallTickBottomPosition;

        private int adjustedSmallIntervalPerTick = 10;

        public override void Render(DrawingContext localDrawingContext)
        {
            var drawingContext = CustomDrawingContext ?? localDrawingContext;

            if (AdjustedPixelsPerTick.Equals(0.0))
                SetValue(AdjustedPixelsPerTickPropertyKey, PixelsPerTick);

            actualWidth = Width;
            actualHeight = Height;

            largeTickTopPosition = actualHeight * LargeTickTop;
            largeTickBottomPosition = actualHeight * LargeTickBottom;
            smallTickTopPosition = actualHeight * SmallTickTop;
            smallTickBottomPosition = actualHeight * SmallTickBottom;

            largeTickPen = LargeTickPen;
            smallTickPen = SmallTickPen;

            var isSmallTickVisible = IsSmallTickVisible;

            textPositionOrigin = TextPositionOrigin;
            textPosition = TextPosition;

            isTextVisible = IsTextVisible;
            if (isTextVisible)
            {
                foreground = Foreground;
                font = Font;
                fontSize = FontSize;
            }

            var adjustedUnitsPerTick = AdjustedUnitsPerTick;
            var adjustedPixelsPerTick = AdjustedPixelsPerTick;
            var decimalCountRounding = DecimalCountRounding;

            var currentUnit = (int)(StartUnit / adjustedUnitsPerTick) * adjustedUnitsPerTick;
            var currentPixel = ((currentUnit - StartUnit) / adjustedUnitsPerTick) * adjustedPixelsPerTick;

            var smallIntevalLength = (1.0 / adjustedSmallIntervalPerTick);

            //if (StartUnit >= 0.0)
            //{
            //    currentPixel += adjustedPixelsPerTick;
            //    currentUnit += adjustedUnitsPerTick;
            //    currentUnit = Math.Round(currentUnit, decimalCountRounding);
            //}

            RaiseBeforeRenderEvent();

            drawingContext.DrawRectangle(Background, null, new Rect(0.0, 0.0, actualWidth, actualHeight));

            RaiseBeforeTicksRenderEvent(drawingContext);

            if (isSmallTickVisible)
            {
                for (var i = 0; i < adjustedSmallIntervalPerTick - 1; i++)
                {
                    var smallLeft = currentPixel - ((i + 1) * adjustedPixelsPerTick) * smallIntevalLength;
                    if (smallLeft < 0.0)
                        break;
                    DrawSmallTick(drawingContext, smallLeft);
                }
            }

            if (currentPixel < 0.0)
            {
                currentPixel += adjustedPixelsPerTick * Math.Ceiling(Math.Abs(currentPixel) / adjustedPixelsPerTick);
            }

            while (currentPixel < actualWidth)
            {
                DrawLargeTick(drawingContext, currentUnit, currentPixel + 1.0);

                if (isSmallTickVisible)
                {
                    for (var i = 0; i < adjustedSmallIntervalPerTick - 1; i++)
                    {
                        var smallLeft = currentPixel + ((i + 1) * adjustedPixelsPerTick) * smallIntevalLength;
                        if (smallLeft > actualWidth)
                            break;
                        DrawSmallTick(drawingContext, smallLeft + 1.0);
                    }
                }

                currentPixel += adjustedPixelsPerTick;
                currentUnit += adjustedUnitsPerTick;
            }

            RaiseAfterTicksRenderEvent(drawingContext);

            RaiseAfterRenderEvent();
        }

        private static double AdjustUnitInterval(double value)
        {
            // computing cannot be done on negative values
            var negative = (value <= 0.0f);
            if (negative)
                value = -value;

            var log = Math.Log10(value);
            var log0 = Math.Pow(10.0, Math.Floor(log));

            double result;

            log = value / log0;
            if (log < (1.0f + 2.0f) * 0.5f) result = log0;
            else if (log < (2.0f + 5.0f) * 0.5f) result = log0 * 2.0f;
            else if (log < (5.0f + 10.0f) * 0.5f) result = log0 * 5.0f;
            else result = log0 * 10.0f;

            if (negative)
                result = -result;

            return result;
        }

        private static bool IsCloser(double value, double other, double reference)
        {
            return Math.Abs(value - reference) < Math.Abs(other - reference);
        }

        private static bool IsCloseEnoughToMultiply([NotNull] List<double> sortedGroupings, double value, double target)
        {
            var result = true;
            var index = sortedGroupings.FindIndex(x => x.Equals(value));

            if (index > 0 && sortedGroupings[index - 1] > target)
                result = false;

            if (index < sortedGroupings.Count - 1 && sortedGroupings[index + 1] < target)
                result = false;

            return result;
        }

        private void AdjustUnitIntervalWithUnitSystem(double value)
        {
            TickTextUnitDivider = 1.0;
            
            if (UnitSystem == null)
            {
                AdjustedUnitsPerTick = AdjustUnitInterval(value);
                return;
            }

            UnitSymbol = UnitSystem.Symbol;

            var scaledValue = value * 1.5;
            var referenceUnitsPerTick = AdjustedUnitsPerTick;
            var hasResult = false;
            var allGrouping = new List<double>();
            UnitSystem.GetAllGroupingValues(ref allGrouping);
            allGrouping.Sort();

            // Check if there is a grouping matching our value
            foreach (var grouping in UnitSystem.GroupingValues)
            {
                if (!hasResult || IsCloser(grouping.LargeIntervalSize, referenceUnitsPerTick, scaledValue))
                {
                    AdjustedUnitsPerTick = grouping.LargeIntervalSize;
                    referenceUnitsPerTick = AdjustedUnitsPerTick;
                    adjustedSmallIntervalPerTick = grouping.SmallIntervalCount;
                    hasResult = true;
                }

                // If the grouping is multipliable using the default grouping method ({1/2/5}*10^n), check for a better value
                if (grouping.IsMultipliable)
                {
                    var val = AdjustUnitInterval(scaledValue / grouping.LargeIntervalSize) * grouping.LargeIntervalSize;

                    if (IsCloseEnoughToMultiply(allGrouping, grouping.LargeIntervalSize, scaledValue) && IsCloser(val, referenceUnitsPerTick, scaledValue))
                    {
                        AdjustedUnitsPerTick = val;
                        referenceUnitsPerTick = grouping.LargeIntervalSize;
                        adjustedSmallIntervalPerTick = grouping.SmallIntervalCount;
                    }
                }
            }

            // When there is no grouping, use the default grouping method
            if (UnitSystem.GroupingValues.Count == 0)
            {
                AdjustedUnitsPerTick = AdjustUnitInterval(scaledValue);
                referenceUnitsPerTick = 1;
                adjustedSmallIntervalPerTick = 10;
            }

            // Check if a conversion may fit better our scale
            foreach (var conversion in UnitSystem.Conversions)
            {
                // Check if there is a grouping matching our value
                foreach (var grouping in conversion.GroupingValues)
                {
                    var groupingValue = grouping.LargeIntervalSize * conversion.Value;

                    if (IsCloser(groupingValue, referenceUnitsPerTick, scaledValue))
                    {
                        AdjustedUnitsPerTick = groupingValue;
                        referenceUnitsPerTick = groupingValue;
                        adjustedSmallIntervalPerTick = grouping.SmallIntervalCount;
                        TickTextUnitDivider = conversion.Value;
                        UnitSymbol = conversion.Symbol;
                    }

                    // If the grouping is multipliable using the default grouping method ({1/2/5}*10^n), check for a better value
                    if (grouping.IsMultipliable)
                    {
                        var val = AdjustUnitInterval(scaledValue / groupingValue) * groupingValue;

                        if (IsCloseEnoughToMultiply(allGrouping, groupingValue, scaledValue) && IsCloser(val, referenceUnitsPerTick, scaledValue))
                        {
                            AdjustedUnitsPerTick = val;
                            referenceUnitsPerTick = groupingValue;
                            adjustedSmallIntervalPerTick = grouping.SmallIntervalCount;
                            TickTextUnitDivider = conversion.Value;
                            UnitSymbol = conversion.Symbol;
                        }
                    }
                }

                // When there is no grouping, use the default grouping method
                if (conversion.GroupingValues.Count == 0)
                {
                    var val = conversion.Value;
                    var canMultiply = true;
                    if (conversion.IsMultipliable)
                    {
                        canMultiply = IsCloseEnoughToMultiply(allGrouping, conversion.Value, scaledValue);
                        val *= AdjustUnitInterval(scaledValue / conversion.Value);
                    }
                    if (canMultiply && IsCloser(val, referenceUnitsPerTick, scaledValue))
                    {
                        AdjustedUnitsPerTick = val;
                        referenceUnitsPerTick = conversion.Value;
                        adjustedSmallIntervalPerTick = 10;
                        TickTextUnitDivider = conversion.Value;
                        UnitSymbol = conversion.Symbol;
                    }
                }
            }
        }

        protected virtual void DrawLargeTick([NotNull] DrawingContext drawingContext, double unit, double position)
        {
            if (isTextVisible)
            {
                var symbol = UnitSymbol ?? "";
                var dividedUnit = !TickTextUnitDivider.Equals(0.0) ? unit / TickTextUnitDivider : unit;
                dividedUnit = Math.Round(dividedUnit, 6);

                var ft = new FormattedText(dividedUnit + symbol, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, font, fontSize, foreground);
                drawingContext.DrawText(ft, new Point(position - ft.Width * textPositionOrigin.X, (textPosition * actualHeight) - (ft.Height * textPositionOrigin.Y)));
            }

            drawingContext.DrawLine(largeTickPen, new Point(position, largeTickTopPosition), new Point(position, largeTickBottomPosition));
        }

        protected virtual void DrawSmallTick([NotNull] DrawingContext drawingContext, double position)
        {
            drawingContext.DrawLine(smallTickPen, new Point(position, smallTickTopPosition), new Point(position, smallTickBottomPosition));
        }

        protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
        {
            base.OnPointerWheelChanged(e);

            if (IsZoomingOnMouseWheel)
            {
                double MouseWheelZoomCoeficient = 1.0;
                var coeficient = e.Delta.Length >= 0.0 ? MouseWheelZoomCoeficient : 1.0 / MouseWheelZoomCoeficient;
                var pos = e.GetPosition(this);

                ZoomAtPosition(pos.X, coeficient, false);
//                 ZoomAtPosition(pos.X, coeficient, Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift));
            }

            e.Handled = true;
        }

        public void ZoomAtPosition(double position, double coeficient, bool affectPixelsPerTick)
        {
            if (affectPixelsPerTick)
                SetPixelsPerTickAt(PixelsPerTick * coeficient, position);
            else
                SetUnitsPerTickAt(UnitsPerTick / coeficient, position);
        }

        private bool isDraggingScale;

        public bool StartDraggingScale()
        {
            if (isDraggingScale)
                return true;

//             isDraggingScale = CaptureMouse();
// 
//             mouseDelta = Mouse.GetPosition(this);
            return isDraggingScale;
        }

        public bool EndDraggingScale()
        {
            if (!isDraggingScale)
                return true;

//             isDraggingScale = !Mouse.Capture(null);
            return !isDraggingScale;
        }

        private Point mouseDelta;

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            if (isDraggingScale)
            {
                var delta = e.GetPosition(this) - mouseDelta;
                mouseDelta = e.GetPosition(this);
                StartUnit -= delta.X / PixelsPerUnit;
            }
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                if (IsDraggingOnLeftMouseButton)
                {
                    StartDraggingScale();
                }
            }
            if (e.GetCurrentPoint(this).Properties.IsRightButtonPressed)
            {
                if (IsDraggingOnRightMouseButton)
                {
                    StartDraggingScale();
                }
            }
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                if (IsDraggingOnLeftMouseButton)
                {
                    EndDraggingScale();
                }
            }
            if (!e.GetCurrentPoint(this).Properties.IsRightButtonPressed)
            {
                if (IsDraggingOnRightMouseButton)
                {
                    EndDraggingScale();
                }
            }
        }
    }
}
