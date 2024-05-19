// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;

using Avalonia.Media;
using Stride.Core.Annotations;
using Stride.Core.Presentation.Extensions;

using Stride.Core.Presentation.Interop;
using Avalonia.VisualTree;

namespace Stride.Core.Presentation.Behaviors
{
    /// <summary>
    /// This static class contains attached dependency properties that can be used as behavior to add or change features of controls.
    /// </summary>
    public class BehaviorProperties : Control
	{
		static BehaviorProperties ()
		{
			HandlesMouseWheelScrollingProperty.Changed.AddClassHandler<BehaviorProperties>(HandlesMouseWheelScrollingChanged);
			KeepTaskbarWhenMaximizedProperty.Changed.AddClassHandler<BehaviorProperties>(KeepTaskbarWhenMaximizedChanged);
		}

        /// <summary>
        /// When attached to a <see cref="ScrollViewer"/> or a control that contains a <see cref="ScrollViewer"/>, this property allows to control whether the scroll viewer should handle scrolling with the mouse wheel.
        /// </summary>
        public static readonly AttachedProperty<bool> HandlesMouseWheelScrollingProperty = AvaloniaProperty<bool>.RegisterAttached<BehaviorProperties, Control, bool>("HandlesMouseWheelScrolling", true); // T12A

        /// <summary>
        /// When attached to a <see cref="Window"/> that have the <see cref="Window.WindowStyle"/> value set to <see cref="WindowStyle.None"/>, prevent the window to expand over the taskbar when maximized.
        /// </summary>
        public static readonly AttachedProperty<bool> KeepTaskbarWhenMaximizedProperty = AvaloniaProperty<bool>.RegisterAttached<BehaviorProperties, Control, bool>("KeepTaskbarWhenMaximized", false); // T12A

        /// <summary>
        /// Gets the current value of the <see cref="HandlesMouseWheelScrollingProperty"/> dependency property attached to the given <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="target">The target <see cref="DependencyObject"/>.</param>
        /// <returns>The value of the <see cref="HandlesMouseWheelScrollingProperty"/> dependency property.</returns>
        public static bool GetHandlesMouseWheelScrolling([NotNull] AvaloniaObject target)
        {
            return (bool)target.GetValue(HandlesMouseWheelScrollingProperty);
        }

        /// <summary>
        /// Sets the value of the <see cref="HandlesMouseWheelScrollingProperty"/> dependency property attached to the given <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="target">The target <see cref="DependencyObject"/>.</param>
        /// <param name="value">The value to set.</param>
        public static void SetHandlesMouseWheelScrolling([NotNull] AvaloniaObject target, bool value)
        {
            target.SetValue(HandlesMouseWheelScrollingProperty, value);
        }

        /// <summary>
        /// Gets the current value of the <see cref="KeepTaskbarWhenMaximizedProperty"/> dependency property attached to the given <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="target">The target <see cref="DependencyObject"/>.</param>
        /// <returns>The value of the <see cref="KeepTaskbarWhenMaximizedProperty"/> dependency property.</returns>
        public static bool GetKeepTaskbarWhenMaximized([NotNull] AvaloniaObject target)
        {
            return (bool)target.GetValue(KeepTaskbarWhenMaximizedProperty);
        }

        /// <summary>
        /// Sets the value of the <see cref="KeepTaskbarWhenMaximizedProperty"/> dependency property attached to the given <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="target">The target <see cref="DependencyObject"/>.</param>
        /// <param name="value">The value to set.</param>
        public static void SetKeepTaskbarWhenMaximized([NotNull] AvaloniaObject target, bool value)
        {
            target.SetValue(KeepTaskbarWhenMaximizedProperty, value);
        }

        private static void HandlesMouseWheelScrollingChanged([NotNull] AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var scrollViewer = Avalonia.VisualTree.VisualExtensions.FindDescendantOfType<ScrollViewer>((Visual) (d as ScrollViewer ?? d));

            if (scrollViewer != null)
            {
                // Yet another internal property that should be public.
                typeof(ScrollViewer).GetProperty("HandlesMouseWheelScrolling", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(scrollViewer, e.NewValue);
            }
            else
            {
                // The framework element is not loaded yet and thus the ScrollViewer is not reachable.
                var frameworkElement = d as Control;
                if (frameworkElement != null && !frameworkElement.IsLoaded)
                {
                    // Let's delay the behavior till the scroll viewer is loaded.
                    frameworkElement.Loaded += (sender, args) =>
                    {
                        var dependencyObject = (AvaloniaObject)sender;
                        var loadedScrollViewer = Avalonia.VisualTree.VisualExtensions.FindDescendantOfType<ScrollViewer>((Visual) dependencyObject);
                        if (loadedScrollViewer != null)
                            typeof(ScrollViewer).GetProperty("HandlesMouseWheelScrolling", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(loadedScrollViewer, e.NewValue);
                    };
                }
            }
        }

        private static void KeepTaskbarWhenMaximizedChanged([NotNull] AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var window = d as Window;
            if (window == null)
                return;

            if (window.IsLoaded)
            {
//                 var hwnd = new WindowInteropHelper(window).Handle;
//                 var source = HwndSource.FromHwnd(hwnd);
//                 source?.AddHook(
//                     (IntPtr h, int msg, IntPtr wparam, IntPtr lparam, ref bool handled) => WindowProc(window, h, msg, wparam, lparam, ref handled));
            }
            else
            {
//                 window.SourceInitialized += (sender, arg) =>
//                 {
//                     var hwnd = new WindowInteropHelper(window).Handle;
//                     var source = HwndSource.FromHwnd(hwnd);
//                     source?.AddHook(
//                         (IntPtr h, int msg, IntPtr wparam, IntPtr lparam, ref bool handled) => WindowProc(window, h, msg, wparam, lparam, ref handled));
//                 };
            }
        }

        private static IntPtr WindowProc([NotNull] Window window, IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
//             switch (msg)
//             {
//                 case NativeHelper.WM_GETMINMAXINFO:
//                     var monitorInfo = WindowHelper.GetMonitorInfo(hwnd);
//                     if (monitorInfo == null)
//                         break;
// 
//                     var mmi = (NativeHelper.MINMAXINFO)Marshal.PtrToStructure(lparam, typeof(NativeHelper.MINMAXINFO));
//                     var rcWorkArea = monitorInfo.rcWork;
//                     var rcMonitorArea = monitorInfo.rcMonitor;
// 
//                     mmi.ptMaxPosition.X = Math.Abs(rcWorkArea.Left - rcMonitorArea.Left);
//                     mmi.ptMaxPosition.Y = Math.Abs(rcWorkArea.Top - rcMonitorArea.Top);
//                     // Get dpi scale
//                     var dpiScale = VisualTreeHelper.GetDpi(window);
//                     // Get maximum width and height from WPF
//                     var maxWidth = double.IsInfinity(window.MaxWidth) ? int.MaxValue : (int)(window.MaxWidth*dpiScale.DpiScaleX);
//                     var maxHeight = double.IsInfinity(window.MaxHeight) ? int.MaxValue : (int)(window.MaxHeight*dpiScale.DpiScaleY);
//                     // Constrain the size when the window is maximized to the work area so that the taskbar is not covered
//                     mmi.ptMaxSize.X = Math.Min(maxWidth, Math.Abs(rcWorkArea.Right - rcWorkArea.Left));
//                     mmi.ptMaxSize.Y = Math.Min(maxHeight, Math.Abs(rcWorkArea.Bottom - rcWorkArea.Top));
//                     // Uncomment the following lines to also constraint the maximum size that the user can manually resize the window when draggin the tracking side
//                     //mmi.ptMaxTrackSize.X = mmi.ptMaxSize.X;
//                     //mmi.ptMaxTrackSize.Y = mmi.ptMaxSize.Y;
// 
//                     Marshal.StructureToPtr(mmi, lparam, true);
//                     handled = true;
//                     break;
//             }
            return IntPtr.Zero;
        }
    }
}