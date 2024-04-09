// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Xaml.Interactivity;
using Stride.Core.Annotations;
using Stride.Core.Presentation.Extensions;


// Remark: The drag'n'drop is pretty broken in WPF, especially the DragLeave event (see https://social.msdn.microsoft.com/Forums/vstudio/en-US/d326384b-e182-4f48-ab8b-841a2c2ca4ab/whats-up-with-dragleave-and-egetposition?forum=wpf&prof=required)
using System;

namespace Stride.Core.Presentation.Behaviors
{
    public class DragOverAutoScrollBehavior : Behavior<Control>
    {
        private readonly object lockObject = new object();
        private bool scrollStarted;
        private CancellationTokenSource cancellationTokenSource;
        private Dock? edgeUnderMouse;

        public static readonly StyledProperty<Thickness> ScrollBorderThicknessProperty = StyledProperty<Thickness>.Register<DragOverAutoScrollBehavior, Thickness>("ScrollBorderThickness", new Thickness(32)); // T2A

        public static readonly StyledProperty<double> DelaySecondsProperty = StyledProperty<double>.Register<DragOverAutoScrollBehavior, double>("DelaySeconds", 0.5); // T2

        public static readonly StyledProperty<double> ScrollingSpeedWidthProperty = StyledProperty<double>.Register<DragOverAutoScrollBehavior, double>("ScrollingSpeed", 300.0); // T2

        public static readonly StyledProperty<bool> VerticalScrollProperty = StyledProperty<bool>.Register<DragOverAutoScrollBehavior, bool>("VerticalScroll", true); // T2

        public static readonly StyledProperty<bool> HorizontalScrollProperty = StyledProperty<bool>.Register<DragOverAutoScrollBehavior, bool>("HorizontalScroll", true); // T2

        public Thickness ScrollBorderThickness { get { return (Thickness)GetValue(ScrollBorderThicknessProperty); } set { SetValue(ScrollBorderThicknessProperty, value); } }
        
        public double DelaySeconds { get { return (double)GetValue(DelaySecondsProperty); } set { SetValue(DelaySecondsProperty, value); } }

        public double ScrollingSpeed { get { return (double)GetValue(ScrollingSpeedWidthProperty); } set { SetValue(ScrollingSpeedWidthProperty, value); } }

        public bool VerticalScroll { get { return (bool)GetValue(VerticalScrollProperty); } set { SetValue(VerticalScrollProperty, value); } }
        
        public bool HorizontalScroll { get { return (bool)GetValue(HorizontalScrollProperty); } set { SetValue(HorizontalScrollProperty, value); } }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AddHandler(DragDrop.DragOverEvent, DragOver);
            AssociatedObject.AddHandler(DragDrop.DragLeaveEvent, DragLeave);
            AssociatedObject.AddHandler(DragDrop.DropEvent, Drop);
        }

        private void Drop(object sender, DragEventArgs e)
        {
            StopScroll();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.RemoveHandler(DragDrop.DragOverEvent, DragOver);
            AssociatedObject.RemoveHandler(DragDrop.DragLeaveEvent, DragLeave);
            AssociatedObject.RemoveHandler(DragDrop.DropEvent, Drop);
            base.OnDetaching();
        }

        private void DragOver(object sender, DragEventArgs e)
        {
            var position = e.GetPosition (AssociatedObject);
//             var position = AssociatedObject.GetCursorRelativePosition();
            lock (lockObject)
            {
                edgeUnderMouse = GetEdgeUnderMouse(position);
            }
            if (edgeUnderMouse != null)
            {
                StartScroll();
            }
        }

        private void DragLeave(object sender, DragEventArgs e)
        {
            var position = e.GetPosition (AssociatedObject);
//             var position = AssociatedObject.GetCursorRelativePosition();
            if (position.X <= 0 || position.Y <= 0 || position.X >= AssociatedObject.Width || position.Y >= AssociatedObject.Height)
            {
                edgeUnderMouse = null;
            }
        }

        private void StopScroll()
        {
            if (!scrollStarted)
                return;

            cancellationTokenSource.Cancel();
            scrollStarted = false;
        }

        private void StartScroll()
        {
            if (scrollStarted)
                return;

            cancellationTokenSource = new CancellationTokenSource();
            var scrollViewer = global::Avalonia.VisualTree.VisualExtensions.FindDescendantOfType<ScrollViewer>(AssociatedObject);
            var delaySeconds = DelaySeconds;
            if (scrollViewer != null)
            {
                scrollStarted = true;
                Task.Run(() => ScrollTask(scrollViewer, delaySeconds), cancellationTokenSource.Token);
            }
        }

        private async Task ScrollTask([NotNull] ScrollViewer scrollViewer, double delaySeconds)
        {
            const int refreshDelay = 25;

            await Task.Delay((int)TimeSpan.FromSeconds(delaySeconds).TotalMilliseconds);

            while (scrollStarted)
            {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        if (edgeUnderMouse.HasValue)
                        {
                            var offset = ScrollingSpeed * refreshDelay / 1000.0;
                            switch (edgeUnderMouse.Value)
                            {
                                case Dock.Left:
                                    scrollViewer.Offset = scrollViewer.Offset - new Vector (offset, 0.0);
//                                     scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - offset);
                                    break;
                                case Dock.Top:
                                    scrollViewer.Offset = scrollViewer.Offset - new Vector (0.0, offset);
//                                     scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - offset);
                                    break;
                                case Dock.Right:
                                    scrollViewer.Offset = scrollViewer.Offset + new Vector (offset, 0.0);
//                                     scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + offset);
                                    break;
                                case Dock.Bottom:
                                    scrollViewer.Offset = scrollViewer.Offset + new Vector (0.0, offset);
//                                     scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + offset);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                        else
                            StopScroll();
                    });
                await Task.Delay(refreshDelay);
            }
        }

        private Dock? GetEdgeUnderMouse(Point point)
        {
            var scrollViewer = global::Avalonia.VisualTree.VisualExtensions.FindDescendantOfType<ScrollViewer>(AssociatedObject);
            Size RenderSize = scrollViewer.Extent;
            if (point.X >= 0 && point.X <= ScrollBorderThickness.Left)
                return Dock.Left;
            if (point.X <= RenderSize.Width && RenderSize.Width - point.X <= ScrollBorderThickness.Right)
                return Dock.Right;
            if (point.Y >= 0 && point.Y <= ScrollBorderThickness.Top)
                return Dock.Top;
            if (point.Y <= RenderSize.Height && RenderSize.Height - point.Y <= ScrollBorderThickness.Bottom)
                return Dock.Bottom;

            return null;
        }
    }
}
