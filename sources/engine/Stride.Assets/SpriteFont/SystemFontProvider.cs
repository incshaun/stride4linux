// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Linq;
// using SharpDX.DirectWrite;
using Stride.Core.Assets.Compiler;
using Stride.Core;
using Stride.Core.Diagnostics;
using Stride.Assets.SpriteFont.Compiler;
using Stride.Core;
using Stride.Core.Assets.Compiler;
using Stride.Core.Diagnostics;
using Stride.Graphics.Font;
using System;
using System.Linq;

using SkiaSharp;

namespace Stride.Assets.SpriteFont
{
    [DataContract("SystemFontProvider")]
    [Display("System Font")]
    public class SystemFontProvider : FontProviderBase
    {
        private static readonly Logger Log = GlobalLogger.GetLogger("SystemFontProvider");

        public SystemFontProvider()
        {
            FontName = GetDefaultFontName();
        }

        public SystemFontProvider(string fontName)
        {
            FontName = fontName;
        }

        /// <summary>
        /// Gets or sets the name of the font family to use when the <see cref="Source"/> is not specified.
        /// </summary>
        /// <userdoc>
        /// The name of the font family to use. Only the fonts installed on the system can be used here.
        /// </userdoc>
        [DataMember(20)]
        [Display("Font Name")]
        public string FontName { get; set; }

        private string OriginalFontName; // Sometimes a font may have to be substituted, if the original is not installed.

        /// <summary>
        /// Gets or sets the style of the font. A combination of 'regular', 'bold', 'italic'. Default is 'regular'.
        /// </summary>
        /// <userdoc>
        /// The style of the font (regular / bold / italic). Note that this property is ignored is the desired style is not available in the font's source file.
        /// </userdoc>
        [DataMember(40)]
        [Display("Style")]
        public override Stride.Graphics.Font.FontStyle Style { get; set; } = Graphics.Font.FontStyle.Regular;

        /// <inheritdoc/>
        public override SKFont GetFontFace(AssetCompilerResult result = null)
        {
            var fontFamilies = SKFontManager.Default.FontFamilies;
            var typeface = SKFontManager.Default.MatchFamily (FontName);
            if (typeface == null)
            {
                result?.Error($"Cannot find system font '{FontName}'. Make sure it is installed on this machine.");
                OriginalFontName = FontName;
                FontName = SKFontManager.Default.GetFamilyName (0);
                result?.Error($"Falling back to system font '{FontName}' from {OriginalFontName}. .");
            }

            var weight = Style.IsBold() ? SKFontStyleWeight.Bold : SKFontStyleWeight.Normal;
            var style = Style.IsItalic() ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright;
            typeface = SKTypeface.FromFamilyName (FontName, weight, SKFontStyleWidth.Normal ,style);

            if (typeface == null)
            {
                result?.Error($"Cannot find style '{Style}' for font family {FontName}. Make sure it is installed on this machine.");
                return null;
            }

            return typeface.ToFont ();
        }

        public override string GetFontPath(AssetCompilerResult result = null)
        {
            SKFont font = GetFontFace ();
/*              var fontFace = new FontFace(font);

                // get the font path on the hard drive
                var file = fontFace.GetFiles().First();
                var referenceKey = file.GetReferenceKey();
                var originalLoader = (FontFileLoaderNative)file.Loader;
                var loader = originalLoader.QueryInterface<LocalFontFileLoader>();
                return loader.GetFilePath(referenceKey);
            }*/
            return "/usr/share/texlive/texmf-dist/bibtex/bst/wiley/Fonts/Arial/Arial.ttf"; // path not relevant here, return empty string. FIXME: placeholder, to fix exceptions.
        }

        /// <inheritdoc/>
        public override string GetFontName()
        {
            return FontName;
        }

        private static string GetDefaultFontName()
        {
            //Note : Both macOS and Windows contains Arial
            return OperatingSystem.IsLinux() ? "Liberation" : "Arial";
        }
    }
}
