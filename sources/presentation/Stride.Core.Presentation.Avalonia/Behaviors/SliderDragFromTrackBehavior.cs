// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using Avalonia;
using Avalonia.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
using Stride.Core.Annotations;
using Stride.Core.Presentation.Extensions;
using Avalonia.Interactivity;
using System;

namespace Stride.Core.Presentation.Behaviors
{
    public class SliderDragFromTrackBehavior : Behavior<Slider>
    {
        private bool trackMouseDown;
        private Track track;

        protected override void OnAttached()
        {
            base.OnAttached();
//             AssociatedObject.AddHandler(Control.PreviewMouseLeftButtonDownEvent, (MouseButtonEventHandler)TrackMouseEvent, true);
//             AssociatedObject.AddHandler(Control.PreviewMouseLeftButtonUpEvent, (MouseButtonEventHandler)TrackMouseEvent, true);
            AssociatedObject.Initialized += SliderInitialized;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Initialized -= SliderInitialized;
//             AssociatedObject.RemoveHandler(Control.PreviewMouseLeftButtonDownEvent, (MouseButtonEventHandler)TrackMouseEvent);
//             AssociatedObject.RemoveHandler(Control.PreviewMouseLeftButtonUpEvent, (MouseButtonEventHandler)TrackMouseEvent);
            if (track != null && track.Thumb != null)
            {
//                 track.Thumb.MouseEnter -= MouseEnter;
            }
            base.OnDetaching();
        }

        private void SliderInitialized(object sender, EventArgs e)
        {
            AssociatedObject.ApplyTemplate();

            track = global::Avalonia.VisualTree.VisualExtensions.FindDescendantOfType<Track>(AssociatedObject);
            if (track == null || track.Name != "PART_Track")
                throw new InvalidOperationException("The associated slider must have a Track child named 'PART_Track'");
//             track.Thumb.MouseEnter += MouseEnter;
            AssociatedObject.Initialized += SliderInitialized;
        }

//         private void TrackMouseEvent(object sender, [NotNull] PointerEventArgs e)
//         {
//             if (e.ChangedButton == MouseButton.Left)
//                 trackMouseDown = e.ButtonState == MouseButtonState.Pressed;
//         }
// 
//         private void MouseEnter(object sender, [NotNull] PointerEventArgs e)
//         {
//             if (trackMouseDown)
//             {
//                 var args = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left) { RoutedEvent = Control.MouseLeftButtonDownEvent };
//                 track.Thumb.RaiseEvent(args);
//                 trackMouseDown = false;
//             }
//         }
    }
}
