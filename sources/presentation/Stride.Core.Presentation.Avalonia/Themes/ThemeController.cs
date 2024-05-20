// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia;
using Avalonia.Controls;

namespace Stride.Core.Presentation.Themes
{
    /// <summary>
    /// This class contains properties to control theming of icons, etc.
    /// </summary>
    public class ThemeController : IThemeController
    {
        static ThemeController ()
        {
            IThemeController.themeSetter = setCurrentTheme;
        }
        
        private static void setCurrentTheme (ThemeType tt)
        {
            CurrentTheme = tt;
        }
        
        /// <summary>
        /// The main purpose of this property is for Luminosity Check feature of
        /// <see cref="ImageThemingUtilities.TransformDrawing(Media.Drawing, IconTheme, bool)"/>.
        /// </summary>
        public static readonly AttachedProperty<bool> IsDarkProperty = AvaloniaProperty<bool>.RegisterAttached<ThemeController, Control, bool>("IsDark", false);

        public static bool GetIsDark(AvaloniaObject obj)
        {
            return (bool)obj.GetValue(IsDarkProperty);
        }

        public static void SetIsDark(AvaloniaObject obj, bool value)
        {
            obj.SetValue(IsDarkProperty, value);
        }

        public static ThemeType CurrentTheme { get; set; }
    }
}
