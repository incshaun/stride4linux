// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;

using Stride.Core.Annotations;
using Stride.Core.Presentation.Interop;

namespace Stride.Core.Presentation.Extensions
{
    /// <summary>
    /// Extension helpers for the <see cref="Window"/> class.
    /// </summary>
    public static class WindowHelper
    {
        /// <summary>
        /// Moves the <paramref cref="window"/> to the center of the given <paramref cref="area"/>.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="area">The area in WPF coordinates (see remarks).</param>
        /// <remarks>
        /// Because of monitor DPI, WPF screen coordinates and virtual screen coordinates can be different.
        /// To convert a <see cref="Rect"/> in virtual screen coordinates to WPF screen coordinates:
        /// <list type="number">
        /// <item>Use <see cref="VisualExtensions.RectFromScreen(Avalonia.Media.Visual,Rect)"/></item>
        /// <item>Offset the result <see cref="Rect"/> by the window top-left corner: <c>rect.Offset(window.Left, window.Top)</c></item>
        /// </list>
        /// To convert a <see cref="Rect"/> in WPF screen coordinates to virtual screen coordinates:
        /// <list type="number">
        /// <item>Un-offset the result <see cref="Rect"/> by the window top-left corner: <c>rect.Offset(-window.Left, -window.Top)</c></item>
        /// <item>Use <see cref="VisualExtensions.RectToScreen(Avalonia.Media.Visual,Rect)"/></item>
        /// </list>
        /// </remarks>
        public static void CenterToArea([NotNull] this Window window, Rect area)
        {
            if (window == null) throw new ArgumentNullException(nameof(window));
            if (area == default) return;

            window.Position = new PixelPoint ((int) (Math.Abs(area.Width - window.Width) / 2 + area.Left), (int) (Math.Abs(area.Height - window.Height) / 2 + area.Top));
        }

        /// <summary>
        /// Gets the position of the cursor in the WPF screen coordinates of the given <paramref name="window"/>.
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        /// <remarks>
        /// This method does not rely on <see cref="System.Windows.Input.Mouse"/> but calls native code to retrieve the position of the cursor.
        /// <br/>
        /// Because of monitor DPI, WPF screen coordinates and virtual screen coordinates can be different.
        /// To convert a <see cref="Point"/> in virtual screen coordinates to WPF screen coordinates:
        /// <list type="number">
        /// <item>Use <see cref="Avalonia.Media.Visual.PointFromScreen"/></item>
        /// <item>Offset the result <see cref="Point"/> by the window top-left corner: <c>point.Offset(window.Left, window.Top)</c></item>
        /// </list>
        /// To convert a <see cref="Point"/> in WPF screen coordinates to virtual screen coordinates:
        /// <list type="number">
        /// <item>Un-offset the result <see cref="Rect"/> by the window top-left corner: <c>point.Offset(-window.Left, -window.Top)</c></item>
        /// <item>Use <see cref="Avalonia.Media.Visual.PointToScreen"/></item>
        /// </list>
        /// </remarks>
        /// <seealso cref="NativeHelper.GetCursorPos"/>
        /// <seealso cref="Avalonia.Media.Visual.PointFromScreen"/>
        public static Point GetCursorScreenPosition([NotNull] this Window window)
        {
//             if (window == null) throw new ArgumentNullException(nameof(window));
// 
//             NativeHelper.POINT point;
//             NativeHelper.GetCursorPos(out point);
//             var position = window.PointFromScreen((Point)point);
//             position.Offset(window.Left, window.Top);
//             return position;
return new Point (0, 0);            
        }

        /// <summary>
        /// Gets the size of the screen monitor for this <paramref cref="window"/>.
        /// </summary>
        /// <param name="window">The window.</param>>
        /// <returns>The size of the screen monitor for this <paramref cref="window"/> in WPF screen coordinates (see remarks).</returns>
        /// <remarks>
        /// Because of monitor DPI, WPF screen coordinates and virtual screen coordinates can be different.
        /// To convert a <see cref="Rect"/> in virtual screen coordinates to WPF screen coordinates:
        /// <list type="number">
        /// <item>Use <see cref="VisualExtensions.RectFromScreen(Avalonia.Media.Visual,Rect)"/></item>
        /// <item>Offset the result <see cref="Rect"/> by the window top-left corner: <c>rect.Offset(window.Left, window.Top)</c></item>
        /// </list>
        /// To convert a <see cref="Rect"/> in WPF screen coordinates to virtual screen coordinates:
        /// <list type="number">
        /// <item>Un-offset the result <see cref="Rect"/> by the window top-left corner: <c>rect.Offset(-window.Left, -window.Top)</c></item>
        /// <item>Use <see cref="VisualExtensions.RectToScreen(Avalonia.Media.Visual,Rect)"/></item>
        /// </list>
        /// </remarks>
        public static Rect GetScreenSize([NotNull] this Window window)
        {
//             var monitor = GetMonitorInfo(new WindowInteropHelper(window).Handle);
//             if (monitor == null) return Rect.Empty;
// 
//             var area = (Rect)monitor.rcMonitor;
//             var rect = window.RectFromScreen(ref area);
//             rect.Offset(window.Left, window.Top);
//             return rect;
            Screen screen = window.Screens.Primary;
            return new Rect (new Point (screen.Bounds.TopLeft.X, screen.Bounds.TopLeft.Y), new Point (screen.Bounds.BottomRight.X, screen.Bounds.BottomRight.Y));
        }

        /// <summary>
        /// Gets the available work area for this <paramref cref="window"/> on the current screen.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns>The available work area on the current screen in WPF screen coordinates (see remarks).</returns>
        /// <remarks>
        /// Because of monitor DPI, WPF screen coordinates and virtual screen coordinates can be different.
        /// To convert a <see cref="Rect"/> in virtual screen coordinates to WPF screen coordinates:
        /// <list type="number">
        /// <item>Use <see cref="VisualExtensions.RectFromScreen(Avalonia.Media.Visual,Rect)"/></item>
        /// <item>Offset the result <see cref="Rect"/> by the window top-left corner: <c>rect.Offset(window.Left, window.Top)</c></item>
        /// </list>
        /// To convert a <see cref="Rect"/> in WPF screen coordinates to virtual screen coordinates:
        /// <list type="number">
        /// <item>Un-offset the result <see cref="Rect"/> by the window top-left corner: <c>rect.Offset(-window.Left, -window.Top)</c></item>
        /// <item>Use <see cref="VisualExtensions.RectToScreen(Avalonia.Media.Visual,Rect)"/></item>
        /// </list>
        /// </remarks>
        public static Rect GetWorkArea([NotNull] this Window window)
        {
//             if (window == null) throw new ArgumentNullException(nameof(window));
// 
//             var monitor = GetMonitorInfo(new WindowInteropHelper(window).Handle);
//             if (monitor == null) return Rect.Empty;
//             
//             var area = (Rect)monitor.rcWork;
//             var rect = window.RectFromScreen(ref area);
//             rect.Offset(window.Left, window.Top);
//             return rect;
            PixelRect? prect = window.Screens.ScreenFromWindow(window)?.WorkingArea;
            return new Rect (new Point ((double) prect?.X, (double) prect?.Y), new Point ((double) prect?.Width, (double) prect?.Height));
        }

        /// <summary>
        /// Moves and resize the <paramref cref="window"/> to make it fill the whole given <paramref cref="area"/>.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="area">The area in WPF coordinates (see remarks).</param>
        /// <remarks>
        /// Because of monitor DPI, WPF screen coordinates and virtual screen coordinates can be different.
        /// To convert a <see cref="Rect"/> in virtual screen coordinates to WPF screen coordinates:
        /// <list type="number">
        /// <item>Use <see cref="VisualExtensions.RectFromScreen(Avalonia.Media.Visual,Rect)"/></item>
        /// <item>Offset the result <see cref="Rect"/> by the window top-left corner: <c>rect.Offset(window.Left, window.Top)</c></item>
        /// </list>
        /// To convert a <see cref="Rect"/> in WPF screen coordinates to virtual screen coordinates:
        /// <list type="number">
        /// <item>Un-offset the result <see cref="Rect"/> by the window top-left corner: <c>rect.Offset(-window.Left, -window.Top)</c></item>
        /// <item>Use <see cref="VisualExtensions.RectToScreen(Avalonia.Media.Visual,Rect)"/></item>
        /// </list>
        /// </remarks>
        public static void FillArea([NotNull] this Window window, Rect area)
        {
            if (window == null) throw new ArgumentNullException(nameof(window));

            window.Width = area.Width;
            window.Height = area.Height;
            window.Position = new PixelPoint ((int) area.Left, (int) area.Top);
        }

        #region Internals
        // FIXME: this should be turned private. Review usage in BehaviorProperties.
//         [CanBeNull]
//         internal static NativeHelper.MONITORINFO GetMonitorInfo(IntPtr hWnd)
//         {
//             var monitor = NativeHelper.MonitorFromWindow(hWnd, NativeHelper.MONITOR_DEFAULTTONEAREST);
//             if (monitor != IntPtr.Zero)
//             {
//                 var monitorInfo = new NativeHelper.MONITORINFO();
//                 NativeHelper.GetMonitorInfo(monitor, monitorInfo);
//                 return monitorInfo;
//             }
// 
//             return null;
//         }
        #endregion // Internals
    }
}
