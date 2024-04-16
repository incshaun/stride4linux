// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using Avalonia;
using Avalonia.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Controls.Shapes;
using System.Windows.Input;
using Avalonia.Styling;
using Avalonia.Data.Core;

namespace Stride.Core.Presentation.Behaviors
{
    public sealed class SelectionRectangleBehavior : MouseMoveCaptureBehaviorBase<Control>
	{
		static SelectionRectangleBehavior ()
		{
			CanvasProperty.Changed.AddClassHandler<SelectionRectangleBehavior>(OnCanvasChanged);
		}

        public static readonly StyledProperty<Canvas> CanvasProperty = StyledProperty<Canvas>.Register<SelectionRectangleBehavior, Canvas>(nameof(Canvas)); // T4A

        public static readonly StyledProperty<ICommand> CommandProperty = StyledProperty<ICommand>.Register<SelectionRectangleBehavior, ICommand>(nameof(Command)); // T1

        public static readonly StyledProperty<Style> RectangleStyleProperty = StyledProperty<Style>.Register<SelectionRectangleBehavior, Style>(nameof(RectangleStyle)); // T1

        private Point originPoint;
        private Rectangle selectionRectangle;
        
        /// <summary>
        /// Resource Key for the default SelectionRectangleStyle.
        /// </summary>
//         public static ResourceKey DefaultRectangleStyleKey { get; } = new ComponentResourceKey(typeof(SelectionRectangleBehavior), nameof(DefaultRectangleStyleKey));
        public static object DefaultRectangleStyleKey { get; } = new object ();

        public Canvas Canvas { get { return (Canvas)GetValue(CanvasProperty); } set { SetValue(CanvasProperty, value); } }

        public ICommand Command { get { return (ICommand)GetValue(CommandProperty); } set { SetValue(CommandProperty, value); } }

        public Style RectangleStyle { get { return (Style)GetValue(RectangleStyleProperty); } set { SetValue(RectangleStyleProperty, value); } }
        
        public bool IsDragging { get; private set; }

        private static void OnCanvasChanged(AvaloniaObject obj, AvaloniaPropertyChangedEventArgs e)
        {
            var behavior = (SelectionRectangleBehavior)obj;
            behavior.OnCanvasChanged(e);
        }

        ///  <inheritdoc/>
        protected override void CancelOverride()
        {
            IsDragging = false;
            Canvas.IsVisible = false;
        }

        ///  <inheritdoc/>
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
//             if (e.ChangedButton != MouseButton.Left)
            if (!e.GetCurrentPoint(AssociatedObject).Properties.IsLeftButtonPressed)
                return;

            e.Handled = true;
            CaptureMouse();
            
            originPoint = e.GetPosition(AssociatedObject);
        }

        ///  <inheritdoc/>
        protected override void OnPointerMoved(PointerEventArgs e)
        {
            if (!e.GetCurrentPoint(AssociatedObject).Properties.IsLeftButtonPressed)
//             if (e.MouseDevice.LeftButton != MouseButtonState.Pressed)
            {
                Cancel();
                return;
            }

            var point = e.GetPosition(AssociatedObject);
            if (IsDragging)
            {
                UpdateDragSelectionRect(originPoint, point);
                e.Handled = true;
            }
            else
            {
                var curMouseDownPoint = e.GetPosition(AssociatedObject);
                var dragDelta = curMouseDownPoint - originPoint;
                var SystemParametersMinimumHorizontalDragDistance = 10;
                var SystemParametersMinimumVerticalDragDistance = 10;
                if (Math.Abs(dragDelta.X) > SystemParametersMinimumHorizontalDragDistance ||
                    Math.Abs(dragDelta.Y) > SystemParametersMinimumVerticalDragDistance)
                {
                    IsDragging = true;
                    InitDragSelectionRect(originPoint, curMouseDownPoint);
                }
                e.Handled = true;
            }
        }

        ///  <inheritdoc/>
        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            if (!e.GetCurrentPoint(AssociatedObject).Properties.IsLeftButtonPressed)
//             if (e.ChangedButton != MouseButton.Left)
                return;

            e.Handled = true;
            ReleaseMouseCapture();

            if (!IsDragging)
                return;

            IsDragging = false;
            ApplyDragSelectionRect();
        }

        private void CreateSelectionRectangle()
        {
            selectionRectangle = new Rectangle();
            if (RectangleStyle != null)
            {
//                 var binding = new Binding
//                 {
//                     Path = new PropertyPath(nameof(RectangleStyle)),
//                     Source = this,
//                 };
//                 selectionRectangle.SetBinding(Control.StyleProperty, binding);
            }
            else
            {
//                 selectionRectangle.Style = selectionRectangle?.TryFindResource(DefaultRectangleStyleKey) as Style;
            }
            selectionRectangle.IsHitTestVisible = false;
        }

        private void OnCanvasChanged(AvaloniaPropertyChangedEventArgs e)
        {
            var oldCanvas = e.OldValue as Canvas;
            if (oldCanvas != null && selectionRectangle != null)
            {
                oldCanvas.Children.Remove(selectionRectangle);
            }

            var newCanvas = e.NewValue as Canvas;
            if (newCanvas == null)
                return;
            newCanvas.IsVisible = false;

            if (selectionRectangle == null)
                CreateSelectionRectangle();
            if (selectionRectangle != null)
                newCanvas.Children.Add(selectionRectangle);
        }

        /// <summary>
        /// Initialize the rectangle used for drag selection.
        /// </summary>
        private void InitDragSelectionRect(Point pt1, Point pt2)
        {
            UpdateDragSelectionRect(pt1, pt2);
            Canvas.IsVisible = true;
        }

        /// <summary>
        /// Update the position and size of the rectangle used for drag selection.
        /// </summary>
        private void UpdateDragSelectionRect(Point pt1, Point pt2)
        {
            double x, y, width, height;

            //
            // Determine x,y,width and height of the rect inverting the points if necessary.
            // 

            if (pt2.X < pt1.X)
            {
                x = pt2.X;
                width = pt1.X - pt2.X;
            }
            else
            {
                x = pt1.X;
                width = pt2.X - pt1.X;
            }

            if (pt2.Y < pt1.Y)
            {
                y = pt2.Y;
                height = pt1.Y - pt2.Y;
            }
            else
            {
                y = pt1.Y;
                height = pt2.Y - pt1.Y;
            }

            //
            // Update the coordinates of the rectangle used for drag selection.
            //
            Canvas.SetLeft(selectionRectangle, x);
            Canvas.SetTop(selectionRectangle, y);
            selectionRectangle.Width = width;
            selectionRectangle.Height = height;
        }

        /// <summary>
        /// Select all nodes that are in the drag selection rectangle.
        /// </summary>
        private void ApplyDragSelectionRect()
        {
            Canvas.IsVisible = false;

            if (Command == null)
                return;

            var x = Canvas.GetLeft(selectionRectangle);
            var y = Canvas.GetTop(selectionRectangle);
            var width = selectionRectangle.Width;
            var height = selectionRectangle.Height;
            var dragRect = new Rect(x, y, width, height);
            
            if (Command.CanExecute(dragRect))
            {
                Command.Execute(dragRect);
            }
        }
    }
}
