// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Stride.Core.Assets;

using SkiaSharp;
using Bitmap = SkiaSharp.SKBitmap;
using Rectangle = SkiaSharp.SKRect;
using Color = SkiaSharp.SKColor;

namespace Stride.Assets.SpriteFont.Compiler
{
//     using System.Drawing;
//     using System.Drawing.Imaging;
    using System.Reflection;
//     using SharpDX.DirectWrite;
//     using Factory = SharpDX.DirectWrite.Factory;

    // This code was originally taken from DirectXTk but rewritten with DirectWrite
    // for more accuracy in font rendering
    internal class SignedDistanceFieldFontImporter : IFontImporter
    {
        // Properties hold the imported font data.
        public IEnumerable<Glyph> Glyphs { get; private set; }

        public float LineSpacing { get; private set; }

        public float BaseLine { get; private set; }

        private string fontSource;
        private string msdfgenExe;
#if DEBUG
        private string tempDir;
#endif

        /// <summary>
        /// Generates and load a SDF font glyph using the msdfgen.exe
        /// </summary>
        /// <param name="c">Character code</param>
        /// <param name="width">Width of the output glyph</param>
        /// <param name="height">Height of the output glyph</param>
        /// <param name="offsetX">Left side offset of the glyph from the image border in design unit</param>
        /// <param name="offsetY">Bottom side offset of the glyph from the image border in design unit</param>
        /// <param name="scaleX">Scale factor to convert from 'shape unit' to 'pixel unit' on x-axis</param>
        /// <param name="scaleY">Scale factor to convert from 'shape unit' to 'pixel unit' on y-axis</param>
        /// <returns></returns>
        private Bitmap LoadSDFBitmap(char c, int width, int height, float offsetX, float offsetY, float scaleX, float scaleY)
        {
//             try
//             {
//                 var characterCodeArg = "0x" + Convert.ToUInt32(c).ToString("x4");
// #if DEBUG
//                 var outputFilePath = $"{tempDir}{characterCodeArg}_{Guid.NewGuid()}.bmp";
// #else
//                 var outputFilePath = Path.GetTempFileName();
// #endif
//                 var exportSizeArg = $"-size {width} {height}";
//                 var translateArg = $"-translate {offsetX} {offsetY}";
//                 var scaleArg = $"-ascale {scaleX} {scaleY}";
// 
//                 var startInfo = new ProcessStartInfo
//                 {
//                     FileName = msdfgenExe,
//                     Arguments = $"msdf -font \"{fontSource}\" {characterCodeArg} -o \"{outputFilePath}\" -format bmp {exportSizeArg} {translateArg} {scaleArg}",
//                     CreateNoWindow = true,
//                     WindowStyle = ProcessWindowStyle.Hidden,
//                     RedirectStandardError = true,
//                     RedirectStandardOutput = true,
//                     UseShellExecute = false
//                 };
//                 var msdfgenProcess = Process.Start(startInfo);
// 
//                 if (msdfgenProcess == null)
//                     return null;
// 
//                 msdfgenProcess.WaitForExit();
// 
//                 if (File.Exists(outputFilePath))
//                 {
//                     var bitmap = (Bitmap)Image.FromFile(outputFilePath);
// 
//                     Normalize(bitmap);
// 
//                     return bitmap;
//                 }
//             }
//             catch
//             {
//                 // ignore exception
//             }

            // If font generation failed for any reason, ignore it and return an empty glyph
            return new Bitmap(1, 1, SKColorType.Rgba8888, SKAlphaType.Opaque);
        }

        /// <summary>
        /// Inverts the color channels if the signed distance appears to be negative.
        /// Msdfgen will produce an inverted picture on occasion.
        /// Because we use offset we can easily detect if the corner pixel has negative (correct) or positive distance (incorrect)
        /// </summary>
        /// <param name="bitmap"></param>
        private void Normalize(Bitmap bitmap)
        {
            // Case 1 - corner pixel is negative (outside), do not invert
            var firstPixel = bitmap.GetPixel(0, 0);
            var colorChannels = 0;
            if (firstPixel.Red > 0) colorChannels++;
            if (firstPixel.Green > 0) colorChannels++;
            if (firstPixel.Blue > 0) colorChannels++;
            if (colorChannels <= 1)
                return;

            // Case 2 - corner pixel is positive (inside), invert the image
            for (var i = 0; i < bitmap.Width; i++)
                for (var j = 0; j < bitmap.Height; j++)
                {
                    var pixel = bitmap.GetPixel(i, j);

                    int invertR = ((int)255 - pixel.Red);
                    int invertG = ((int)255 - pixel.Green);
                    int invertB = ((int)255 - pixel.Blue);
//                     var invertedPixel = Color.FromArgb((invertR << 16) + (invertG << 8) + (invertB));
                    var invertedPixel = new Color((byte) invertR, (byte) invertG, (byte) invertB);

                    bitmap.SetPixel(i, j, invertedPixel);
                }
        }

        /// <inheritdoc/>
        public void Import(SpriteFontAsset options, List<char> characters)
        {
            fontSource = options.FontSource.GetFontPath();
//             if (string.IsNullOrEmpty(fontSource))
//                 return;

//             // Get the msdfgen.exe location
//             var msdfgen = ToolLocator.LocateTool("msdfgen") ?? throw new AssetException("Failed to compile a font asset, msdfgen was not found.");
// 
//             msdfgenExe = msdfgen.FullPath;
//             tempDir = $"{Environment.GetEnvironmentVariable("TEMP")}\\";
// 
//             var factory = new Factory();

            SKFont fontFace = options.FontSource.GetFontFace();

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

//             float pixelPerDesignUnit = fontSize / fontMetrics.DesignUnitsPerEm;
            // Store the font height.
            float fontMetricsDesignUnitsPerEm = 1.0f;
            LineSpacing = (float)(lineGap - fontMetrics.Ascent + fontMetrics.Descent) / fontMetricsDesignUnitsPerEm * fontSize;
//             LineSpacing = (lineGap + fontMetrics.Ascent + fontMetrics.Descent) * pixelPerDesignUnit;

            // And then the baseline is also changed in order to allow the linegap to be distributed between the top and the
            // bottom of the font:
            BaseLine = (float)(lineGap * options.LineGapBaseLineFactor - fontMetrics.Ascent) / fontMetricsDesignUnitsPerEm * fontSize;
            //     BaseLine = NewLineGap * LineGapBaseLineFactor
//             BaseLine = (lineGap * options.LineGapBaseLineFactor + fontMetrics.Ascent) * pixelPerDesignUnit;

            // Generate SDF bitmaps for each character in turn.
            foreach (var character in characters)
                glyphList.Add(ImportGlyph(fontFace, character, fontMetrics, fontSize));

            Glyphs = glyphList;
// 
//             factory.Dispose();
        }

        /// <summary>
        /// Imports a single glyph as a bitmap using the msdfgen to convert it to a signed distance field image
        /// </summary>
        /// <param name="fontFace">FontFace, use to obtain the metrics for the glyph</param>
        /// <param name="character">The glyph's character code</param>
        /// <param name="fontMetrics">Font metrics, used to obtain design units scale</param>
        /// <param name="fontSize">Requested font size. The bigger, the more precise the SDF image is going to be</param>
        /// <returns></returns>
        private Glyph ImportGlyph(SKFont fontFace, char character, SKFontMetrics fontMetrics, float fontSize)
        {
            SKPaint textPaint = new SKPaint { Color = SKColors.White, TextSize = fontSize, TextAlign = SKTextAlign.Center };
            SKRect rect = new SKRect ();
            textPaint.MeasureText ("" + character, ref rect);
            int width = (int) rect.Width;
            int height = (int) rect.Height;

            int yOffset = (int) (fontSize * ((rect.Top + rect.Bottom) / 22.0f));
            yOffset = (int) (-4 * fontSize) + (int) (rect.Top - ((rect.Top + rect.Bottom) / 2) / 2.0f); // not sure why the -4, fits text onto gizmo camera, will have to explore further.

            var advanceWidth = 2 + (int) textPaint.GetGlyphWidths ("" + character)[0];

            var pixelWidth = width + 10;
            var pixelHeight = height + 10;
//             var metrics = fontFace.GetDesignGlyphMetrics(indices, isSideways: false);
//             var metric = metrics[0];
// 
//             float pixelPerDesignUnit = fontSize / fontMetrics.DesignUnitsPerEm;
//             float fontWidthPx = (metric.AdvanceWidth - metric.LeftSideBearing - metric.RightSideBearing) * pixelPerDesignUnit;
//             float fontHeightPx = (metric.AdvanceHeight - metric.TopSideBearing - metric.BottomSideBearing) * pixelPerDesignUnit;
// 
//             float fontOffsetXPx = metric.LeftSideBearing * pixelPerDesignUnit;
//             float fontOffsetYPx = (metric.TopSideBearing - metric.VerticalOriginY) * pixelPerDesignUnit;
// 
//             float advanceWidthPx = metric.AdvanceWidth * pixelPerDesignUnit;
//             //var advanceHeight = metric.AdvanceHeight * pixelPerDesignUnit;
// 
//             const int MarginPx = 2;     // Buffer zone for the sdf image to avoid clipping
//             int bitmapWidthPx = (int)Math.Ceiling(fontWidthPx) + (2 * MarginPx);
//             int bitmapHeightPx = (int)Math.Ceiling(fontHeightPx) + (2 * MarginPx);
// 
//             float bitmapOffsetXPx = fontOffsetXPx - MarginPx;
//             float bitmapOffsetYPx = fontOffsetYPx - MarginPx;

            Bitmap bitmap;
            if (char.IsWhiteSpace(character))
            {
                bitmap = new Bitmap(1, 1, SKColorType.Rgba8888, SKAlphaType.Opaque);
            }
            else
            {
                bitmap = new Bitmap(pixelWidth, pixelHeight, SKColorType.Rgba8888, SKAlphaType.Opaque);
                SKBitmap texture =  new SKBitmap (pixelWidth, pixelHeight);
                SKCanvas bitmapCanvas = new SKCanvas(texture);
                bitmapCanvas.Clear(new SKColor(1, 1, 1)); // Bug in skiasharp seems to not clear if using perfect black.
                bitmapCanvas.DrawText("" + character, pixelWidth / 2, (pixelHeight - (rect.Bottom + rect.Top)) / 2, textPaint);
                for (int y = 0; y < pixelHeight; y++)
                {
                    for (int x = 0; x < pixelWidth; x++)
                    {
                        SKColor col = texture.GetPixel (x, y);
//                         var color = Color.FromArgb(col.Red, col.Green, col.Blue);
                        var color = new Color(col.Red, col.Green, col.Blue);

                        bitmap.SetPixel(x, y, color);
                    }
                }
//                 // sdfPixelPerDesignUnit is hardcoded from the import in this code
//                 // https://github.com/stride3d/msdfgen/blob/1af188c77822e447fe8e412420fe0fe05b782b38/ext/import-font.cpp#L126-L150
//                 const float sdfPixelPerDesignUnit = 1 / 64f;      // msdf default coordinate scale
//                 float boundLeft = metric.LeftSideBearing * sdfPixelPerDesignUnit;
//                 //float boundRight = (metric.AdvanceWidth - metric.RightSideBearing) * sdfPixelPerDesignUnit;
//                 //float boundTop = (metric.VerticalOriginY - metric.TopSideBearing) * sdfPixelPerDesignUnit;
//                 float boundBottom = (metric.VerticalOriginY  - (metric.AdvanceHeight - metric.BottomSideBearing)) * sdfPixelPerDesignUnit;
// 
//                 float glyphWidthPx = (metric.AdvanceWidth - metric.LeftSideBearing - metric.RightSideBearing) * sdfPixelPerDesignUnit;
//                 float glyphHeightPx = (metric.AdvanceHeight - metric.TopSideBearing - metric.BottomSideBearing) * sdfPixelPerDesignUnit;
// 
//                 // Need to scale from msdfgen's 'shape unit' into the final bitmap's space
//                 float scaleX = fontWidthPx / glyphWidthPx;
//                 float scaleY = fontHeightPx / glyphHeightPx;
// 
//                 // Note: msdfgen uses coordinates from bottom-left corner
//                 // so offsetY needs to calculate the offset such that it snaps to the top side of the bitmap (+ margin space)
//                 float offsetX = (MarginPx / scaleX) - boundLeft;
//                 float offsetY = ((bitmapHeightPx - MarginPx) / scaleY) - glyphHeightPx - boundBottom;
// 
//                 bitmap = LoadSDFBitmap(character, bitmapWidthPx, bitmapHeightPx, offsetX, offsetY, scaleX, scaleY);
            }

            var glyph = new Glyph(character, bitmap)
            {
                XOffset = 0,
                XAdvance = advanceWidth,
                YOffset = yOffset,
//                 XOffset = bitmapOffsetXPx,
//                 XAdvance = advanceWidthPx,
//                 YOffset = bitmapOffsetYPx,
            };
            return glyph;
        }

        private static byte LinearToGamma(byte color)
        {
            return (byte)(Math.Pow(color / 255.0f, 1 / 2.2f) * 255.0f);
        }
    }
}
