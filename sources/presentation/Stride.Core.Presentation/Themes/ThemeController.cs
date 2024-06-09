// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace Stride.Core.Presentation.Themes
{
    /// <summary>
    /// This class contains properties to control theming of icons, etc.
    /// </summary>
    public interface IThemeController
    {
        protected delegate void SetTheme (ThemeType tt);
        protected static SetTheme themeSetter;
        public static void SetCurrentTheme (ThemeType tt)
        {
            themeSetter (tt);
        }
    }
}
