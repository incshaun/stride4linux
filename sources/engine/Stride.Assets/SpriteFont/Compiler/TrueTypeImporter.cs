// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
//
// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;

using SkiaSharp;
using Color = SkiaSharp.SKColor;
using Rectangle = SkiaSharp.SKRect;
using Bitmap = SkiaSharp.SKBitmap;

using Stride.Graphics.Font;

namespace Stride.Assets.SpriteFont.Compiler
{
//     using System.Drawing;
//     using System.Drawing.Imaging;

    // This code was originally taken from DirectXTk but rewritten with DirectWrite
    // for more accuracy in font rendering
    internal class TrueTypeImporter : IFontImporter
    {
        // Properties hold the imported font data.
        public IEnumerable<Glyph> Glyphs { get; private set; }

        public float LineSpacing { get; private set; }

        public float BaseLine { get; private set; }

        public void Import(SpriteFontAsset options, List<char> characters)
        {
            var fontFace = options.FontSource.GetFontFace();
            
            var fontMetrics = fontFace.Metrics;

            // Create a bunch of GDI+ objects.
            var fontSize = options.FontType.Size;

            var glyphList = new List<Glyph>();

            // Remap the LineMap coming from the font with a user defined remapping
            // Note:
            // We are remapping the lineMap to allow to shrink the LineGap and to reposition it at the top and/or bottom of the 
            // font instead of using only the top
            // According to http://stackoverflow.com/questions/13939264/how-to-determine-baseline-position-using-directwrite#comment27947684_14061348
            // (The response is from a MSFT employee), the BaseLine should be = LineGap + Ascent but this is not what
            // we are experiencing when comparing with MSWord (LineGap + Ascent seems to offset too much.)
            //
            // So we are first applying a factor to the line gap:
            //     NewLineGap = LineGap * LineGapFactor
            var lineGap = fontMetrics.Leading * options.LineGapFactor;

            // Store the font height.
            float fontMetricsDesignUnitsPerEm = 1.0f;
            LineSpacing = (float)(lineGap - fontMetrics.Ascent + fontMetrics.Descent) / fontMetricsDesignUnitsPerEm * fontSize;

            // And then the baseline is also changed in order to allow the linegap to be distributed between the top and the 
            // bottom of the font:
            BaseLine = (float)(lineGap * options.LineGapBaseLineFactor - fontMetrics.Ascent) / fontMetricsDesignUnitsPerEm * fontSize;

            // Rasterize each character in turn.
            foreach (var character in characters)
                glyphList.Add(ImportGlyph(fontFace, character, fontMetrics, fontSize, options.FontType.AntiAlias));

            Glyphs = glyphList;
        }
        
        private Glyph ImportGlyph(SKFont fontFace, char character, SKFontMetrics fontMetrics, float fontSize, FontAntiAliasMode antiAliasMode)
        {
            SKPaint textPaint = new SKPaint { Color = SKColors.White, TextSize = fontSize, TextAlign = SKTextAlign.Center };
            Rectangle rect = new Rectangle ();
            string text = "" + character;
//             text = "-";
            textPaint.MeasureText (text, ref rect);
            int width = (int) rect.Width;
            int height = (int) rect.Height;

            float xOffset = (int) (0 * fontSize);
            float yOffset = (int) (fontSize * ((rect.Top + rect.Bottom) / 22.0f));
            yOffset = (int) (-4 * fontSize) + (int) (rect.Top - ((rect.Top + rect.Bottom) / 2) / 2.0f); // not sure why the -4, fits text onto gizmo camera, will have to explore further.
//             yOffset = (int) (-4f * fontSize);
//             yOffset = -0.5f;

            var advanceWidth = 2 + (int) textPaint.GetGlyphWidths (text)[0];

            var pixelWidth = width + 10;
            var pixelHeight = height + 10;

            Bitmap bitmap;
            if (char.IsWhiteSpace(character))
            {
                bitmap = new Bitmap(1, 1, SKColorType.Rgba8888, SKAlphaType.Opaque);
            }
            else
            {
//                 bitmap = new Bitmap(pixelWidth, pixelHeight, SKColorType.Rgba8888/*PixelFormat.Format32bppArgb*/, SKAlphaType.Opaque);
                bitmap = new Bitmap (pixelWidth, pixelHeight);
                SKCanvas bitmapCanvas = new SKCanvas(bitmap);
                bitmapCanvas.Clear(new SKColor(1, 1, 1)); // Bug in skiasharp seems to not clear if using perfect black.
                Console.WriteLine ("Imp: " + character + " - " + rect + " - " + pixelWidth + " : " + pixelHeight + " -- " + yOffset + " -- " + width + " - " + height + " :: " + advanceWidth + " --- " + rect.MidX + " - " + rect.MidY + " -- " + fontSize);
//                 bitmapCanvas.DrawText("XTesting" + character, pixelWidth / 2, (pixelHeight - (rect.Bottom + rect.Top)) / 2, textPaint);
                bitmapCanvas.DrawText(text, pixelWidth / 2 - rect.Left, pixelHeight / 2 - rect.MidY, textPaint);
//                 for (int y = 0; y < pixelHeight; y++)
//                 {
//                     Console.Write (y + ": ");
//                     for (int x = 0; x < pixelWidth; x++)
//                     {
//                         Console.Write (bitmap.GetPixel (x, y).Red > 50 ? "X" : " " );
//                     }
//                     Console.WriteLine ();
//                 }
//                 for (int y = 0; y < pixelHeight; y++)
//                 {
//                     for (int x = 0; x < pixelWidth; x++)
//                     {
//                         SKColor col = texture.GetPixel (x, y);
//                         var color = Color.FromArgb(col.Red, col.Green, col.Blue);
// 
//                         bitmap.SetPixel(x, y, color);
//                     }
//                 }
            }

            var glyph = new Glyph(character, bitmap)
            {
                XOffset = xOffset,
                XAdvance = advanceWidth,
                YOffset = yOffset,
            };
            return glyph;
        }

        private static byte LinearToGamma(byte color)
        {
            return (byte)(Math.Pow(color / 255.0f, 1 / 2.2f) * 255.0f);
        }
    }
}
