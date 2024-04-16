// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

#region Copyright and license
// Some parts of this file were inspired by OxyPlot (https://github.com/oxyplot/oxyplot)
/*
The MIT license (MTI)
https://opensource.org/licenses/MIT

Copyright (c) 2014 OxyPlot contributors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal 
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is 
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion

using Avalonia;
using Avalonia.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Controls.Shapes;
using Stride.Core.Annotations;
using Stride.Core.Presentation.Extensions;

using Avalonia.Controls.Primitives;
using Avalonia.Controls.Metadata;

namespace Stride.Core.Presentation.Controls
{
    using MathUtil = Stride.Core.Mathematics.MathUtil;

    [TemplatePart(Name = HorizontalLinePartName, Type = typeof(Line))]
    [TemplatePart(Name = VerticalLinePartName, Type = typeof(Line))]
    public class TrackerControl : TemplatedControl
    {
        /// <summary>
        /// The name of the part for the horizontal line.
        /// </summary>
        private const string HorizontalLinePartName = "PART_HorizontalLine";

        /// <summary>
        /// The name of the part for  the vertical line.
        /// </summary>
        private const string VerticalLinePartName = "PART_VerticalLine";

        /// <summary>
        /// Identifies the <see cref="HorizontalLineVisibility"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> HorizontalLineVisibilityProperty = StyledProperty<bool>.Register<TrackerControl, bool>(nameof(HorizontalLineVisibility), true); // T2

        /// <summary>
        /// Identifies the <see cref="LineExtents"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<Rect> LineExtentsProperty = StyledProperty<Rect>.Register<TrackerControl, Rect>(nameof(LineExtents)); // T1

        /// <summary>
        /// Identifies the <see cref="LineStroke"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<IBrush> LineStrokeProperty = StyledProperty<IBrush>.Register<TrackerControl, IBrush>(nameof(LineStroke)); // T1

        /// <summary>
        /// Identifies the <see cref="LineThickness"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<Thickness> LineThicknessProperty = StyledProperty<Thickness>.Register<TrackerControl, Thickness>(nameof(LineThickness)); // T1

        /// <summary>
        /// Identifies the <see cref="Position"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<Point> PositionProperty = StyledProperty<Point>.Register<TrackerControl, Point>(nameof(Position), new Point()); // T4

        /// <summary>
        /// Identifies the <see cref="TrackMouse"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> TrackMouseProperty = StyledProperty<bool>.Register<TrackerControl, bool>(nameof(TrackMouse), false); // T5

        /// <summary>
        /// Identifies the <see cref="VerticalLineVisibility"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> VerticalLineVisibilityProperty = StyledProperty<bool>.Register<TrackerControl, bool>(nameof(VerticalLineVisibility), true); // T2

        private Line horizontalLine;
        private Line verticalLine;
        private Control parent;

        static TrackerControl()
		{
			PositionProperty.Changed.AddClassHandler<TrackerControl>(OnPositionChanged);
			TrackMouseProperty.Changed.AddClassHandler<TrackerControl>(OnTrackMouseChanged);

            // FIXME  T31
        }

        public bool HorizontalLineVisibility { get { return (bool)GetValue(HorizontalLineVisibilityProperty); }  set { SetValue(HorizontalLineVisibilityProperty, value); } }
        
        public Rect LineExtents { get { return (Rect)GetValue(LineExtentsProperty); }  set { SetValue(LineExtentsProperty, value); } }

        public IBrush LineStroke { get { return (IBrush)GetValue(LineStrokeProperty); }  set { SetValue(LineStrokeProperty, value); } }

        public Thickness LineThickness { get { return (Thickness)GetValue(LineThicknessProperty); } set { SetValue(LineThicknessProperty, value); } }

        public Point Position { get { return (Point)GetValue(PositionProperty); } set { SetValue(PositionProperty, value); } }

        public bool TrackMouse { get { return (bool)GetValue(TrackMouseProperty); } set { SetValue(TrackMouseProperty, value); } }
        
        public bool VerticalLineVisibility { get { return (bool)GetValue(VerticalLineVisibilityProperty); }  set { SetValue(VerticalLineVisibilityProperty, value); } }

        private static void OnPositionChanged([NotNull] AvaloniaObject sender, AvaloniaPropertyChangedEventArgs e)
        {
            ((TrackerControl)sender).OnPositionChanged();
        }

        private static void OnTrackMouseChanged([NotNull] AvaloniaObject sender, AvaloniaPropertyChangedEventArgs e)
        {
            ((TrackerControl)sender).OnTrackMouseChanged(e);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
		{
			base.OnApplyTemplate(e);

            horizontalLine = e.NameScope.Find<Line>(HorizontalLinePartName);
            verticalLine = e.NameScope.Find<Line>(VerticalLinePartName);

            if (parent != null && TrackMouse)
                parent.PointerMoved -= OnMouseMove;
            parent = global::Avalonia.VisualTree.VisualExtensions.FindAncestorOfType<Control>(this);
            if (TrackMouse)
                parent.PointerMoved += OnMouseMove;
        }

        private void OnMouseMove(object sender, [NotNull] PointerEventArgs e)
        {
            if (!TrackMouse)
                return;
            Position = e.GetPosition(this);
        }
        
        private void OnPositionChanged()
        {
            UpdatePositionAndBorder();
        }

        private void OnTrackMouseChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if (parent == null)
                return;

            if ((bool)e.NewValue)
                parent.PointerMoved += OnMouseMove;
            else
                parent.PointerMoved -= OnMouseMove;
        }

        private void UpdatePositionAndBorder()
        {
            if (parent == null)
                return;

            var width = parent.Width;
            var height = parent.Height;
            var lineExtents = LineExtents;
            var pos = Position;

            if (horizontalLine != null)
            {
                if (LineExtents.Width > 0)
                {
                    var posY = MathUtil.Clamp(pos.Y, lineExtents.Top, lineExtents.Bottom);
                    horizontalLine.StartPoint = new Point (lineExtents.Left, posY);
                    horizontalLine.EndPoint = new Point (lineExtents.Right, posY);
                }
                else
                {
                    horizontalLine.StartPoint = new Point (0, pos.Y);
                    horizontalLine.EndPoint = new Point (width, pos.Y);
                }
            }

            if (verticalLine != null)
            {
                if (LineExtents.Width > 0)
                {
                    var posX = MathUtil.Clamp(pos.X, lineExtents.Left, lineExtents.Right);
                    verticalLine.StartPoint = new Point (posX, lineExtents.Top);
                    verticalLine.EndPoint = new Point (posX, lineExtents.Bottom);
                }
                else
                {
                    verticalLine.StartPoint = new Point (pos.X, 0);
                    verticalLine.EndPoint = new Point (pos.X, height);
                }
            }
        }
    }
}
