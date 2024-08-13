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
// -----------------------------------------------------------------------------
// The following code is a port of MakeSpriteFont from DirectXTk
// http://go.microsoft.com/fwlink/?LinkId=248929
// -----------------------------------------------------------------------------
// Microsoft Public License (Ms-PL)
//
// This license governs use of the accompanying software. If you use the 
// software, you accept this license. If you do not accept the license, do not
// use the software.
//
// 1. Definitions
// The terms "reproduce," "reproduction," "derivative works," and 
// "distribution" have the same meaning here as under U.S. copyright law.
// A "contribution" is the original software, or any additions or changes to 
// the software.
// A "contributor" is any person that distributes its contribution under this 
// license.
// "Licensed patents" are a contributor's patent claims that read directly on 
// its contribution.
//
// 2. Grant of Rights
// (A) Copyright Grant- Subject to the terms of this license, including the 
// license conditions and limitations in section 3, each contributor grants 
// you a non-exclusive, worldwide, royalty-free copyright license to reproduce
// its contribution, prepare derivative works of its contribution, and 
// distribute its contribution or any derivative works that you create.
// (B) Patent Grant- Subject to the terms of this license, including the license
// conditions and limitations in section 3, each contributor grants you a 
// non-exclusive, worldwide, royalty-free license under its licensed patents to
// make, have made, use, sell, offer for sale, import, and/or otherwise dispose
// of its contribution in the software or derivative works of the contribution 
// in the software.
//
// 3. Conditions and Limitations
// (A) No Trademark License- This license does not grant you rights to use any 
// contributors' name, logo, or trademarks.
// (B) If you bring a patent claim against any contributor over patents that 
// you claim are infringed by the software, your patent license from such 
// contributor to the software ends automatically.
// (C) If you distribute any portion of the software, you must retain all 
// copyright, patent, trademark, and attribution notices that are present in the
// software.
// (D) If you distribute any portion of the software in source code form, you 
// may do so only under this license by including a complete copy of this 
// license with your distribution. If you distribute any portion of the software
// in compiled or object code form, you may only do so under a license that 
// complies with this license.
// (E) The software is licensed "as-is." You bear the risk of using it. The
// contributors give no express warranties, guarantees or conditions. You may
// have additional consumer rights under your local laws which this license 
// cannot change. To the extent permitted under your local laws, the 
// contributors exclude the implied warranties of merchantability, fitness for a
// particular purpose and non-infringement.
//--------------------------------------------------------------------

using System;
// using System.Drawing;
// using System.Drawing.Imaging;
using System.Runtime.InteropServices;


using Stride.Core.Mathematics;

using SkiaSharp;
using DrawingColor = SkiaSharp.SKColor;
using Rectangle = SkiaSharp.SKRect;
using Bitmap = SkiaSharp.SKBitmap;
using PixelFormat = SkiaSharp.SKColorType;

namespace Stride.Assets.SpriteFont.Compiler
{
    // Assorted helpers for doing useful things with bitmaps.
    internal static class BitmapUtils
    {
        // Copies a rectangular area from one bitmap to another.
        public static void CopyRect(Bitmap source, Rectangle sourceRegion, Bitmap output, Rectangle outputRegion)
        {
            if (sourceRegion.Width != outputRegion.Width ||
                sourceRegion.Height != outputRegion.Height)
            {
                throw new ArgumentException();
            }

//             using (var sourceData = new PixelAccessor(source/*, ImageLockMode.ReadOnly*/, sourceRegion))
//             using (var outputData = new PixelAccessor(output/*, ImageLockMode.WriteOnly*/, outputRegion))
            {
                for (int y = 0; y < sourceRegion.Height; y++)
                {
                    for (int x = 0; x < sourceRegion.Width; x++)
                    {
//                         outputData[x, y] = sourceData[x, y];
                        output.SetPixel (x + (int) outputRegion.Left, y + (int) outputRegion.Top, source.GetPixel (x + (int) sourceRegion.Left, y + (int) sourceRegion.Top));
                    }
                }
            }
        }


        // Checks whether an area of a bitmap contains entirely the specified alpha value.
        public static bool IsAlphaEntirely(byte expectedAlpha, Bitmap bitmap, Rectangle? region = null)
        {
//             using (var bitmapData = new PixelAccessor(bitmap, ImageLockMode.ReadOnly, region))
            {
                for (int y = 0; y < region?.Height; y++)
                {
                    for (int x = 0; x < region?.Width; x++)
                    {
//                         byte alpha = bitmapData[x, y].A;
                        byte alpha = bitmap.GetPixel (x + (int) region?.Left, y + (int) region?.Top).Alpha;

                        if (alpha != expectedAlpha)
                            return false;
                    }
                }
            }

            return true;
        }


        // Checks whether a bitmap contains entirely the specified RGB value.
        public static bool IsRgbEntirely(DrawingColor expectedRgb, Bitmap bitmap)
        {
//             using (var bitmapData = new PixelAccessor(bitmap, ImageLockMode.ReadOnly))
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        DrawingColor color = bitmap.GetPixel (x, y);

                        if (color.Alpha == 0)
                            continue;

                        if ((color.Red != expectedRgb.Red) ||
                            (color.Green != expectedRgb.Green) ||
                            (color.Blue != expectedRgb.Blue))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }


        // Converts greyscale luminosity to alpha data.
        public static void ConvertGreyToAlpha(Bitmap bitmap, Rectangle region)
        {
//             using (var bitmapData = new PixelAccessor(bitmap, ImageLockMode.ReadWrite, region))
            {
                for (int y = 0; y < region.Height; y++)
                {
                    for (int x = 0; x < region.Width; x++)
                    {
                        var color = bitmap.GetPixel (x + (int) region.Left, y + (int) region.Top);

                        // Average the red, green and blue values to compute brightness.
                        var alpha = (color.Red + color.Green + color.Blue) / 3;

                        bitmap.SetPixel (x, y, new DrawingColor(255, 255, 255, (byte) alpha));
                    }
                }
            }
        }

        // Converts a bitmap to premultiplied alpha format.
        public static void PremultiplyAlphaClearType(Bitmap bitmap, bool srgb)
        {
//             using (var bitmapData = new PixelAccessor(bitmap, ImageLockMode.ReadWrite))
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        DrawingColor color = bitmap.GetPixel (x, y);

                        int a;
                        if (srgb)
                        {
                            var colorLinear = new Color4(new Stride.Core.Mathematics.Color(color.Red, color.Green, color.Blue)).ToLinear();
                            var alphaLinear = (colorLinear.R + colorLinear.G + colorLinear.B) / 3.0f;
                            a = MathUtil.Clamp((int)Math.Round(alphaLinear * 255), 0, 255);
                        }
                        else
                        {
                            a = (color.Red + color.Green + color.Blue) / 3;
                        }
                        int r = color.Red;
                        int g = color.Green;
                        int b = color.Blue;

                        bitmap.SetPixel (x, y, new DrawingColor((byte) r, (byte) g, (byte) b, (byte) a));
                    }
                }
            }
        }

        // Converts a bitmap to premultiplied alpha format.
        public static void PremultiplyAlpha(Bitmap bitmap, bool srgb)
        {
//             using (var bitmapData = new PixelAccessor(bitmap, ImageLockMode.ReadWrite))
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        DrawingColor color = bitmap.GetPixel (x, y);
                        int a = color.Alpha;
                        int r;
                        int g;
                        int b;
                        if (srgb)
                        {
                            var colorLinear = new Color4(new Stride.Core.Mathematics.Color(color.Red, color.Green, color.Blue)).ToLinear();
                            colorLinear *= color.Alpha / 255.0f;
                            var colorSRgb = (Stride.Core.Mathematics.Color)colorLinear.ToSRgb();
                            r = colorSRgb.R;
                            g = colorSRgb.G;
                            b = colorSRgb.B;
                        }
                        else
                        {
                            r = color.Red * a / 255;
                            g = color.Green * a / 255;
                            b = color.Blue * a / 255;
                        }

                        bitmap.SetPixel (x, y, new DrawingColor((byte) r, (byte) g, (byte) b, (byte) a));
                    }
                }
            }
        }


        // To avoid filtering artifacts when scaling or rotating fonts that do not use premultiplied alpha,
        // make sure the one pixel border around each glyph contains the same RGB values as the edge of the
        // glyph itself, but with zero alpha. This processing is an elaborate no-op when using premultiplied
        // alpha, because the premultiply conversion will change the RGB of all such zero alpha pixels to black.
        public static void PadBorderPixels(Bitmap bitmap, Rectangle region)
        {
//             using (var bitmapData = new PixelAccessor(bitmap, ImageLockMode.ReadWrite))
            {
                // Pad the top and bottom.
                for (int x = (int) region.Left; x < (int) region.Right; x++)
                {
                    CopyBorderPixel(bitmap, x, (int) region.Top, x, (int) region.Top - 1);
                    CopyBorderPixel(bitmap, x, (int) region.Bottom - 1, x, (int) region.Bottom);
                }

                // Pad the left and right.
                for (int y = (int) region.Top; y < (int) region.Bottom; y++)
                {
                    CopyBorderPixel(bitmap, (int) region.Left, y, (int) region.Left - 1, y);
                    CopyBorderPixel(bitmap, (int) region.Right - 1, y, (int) region.Right, y);
                }

                // Pad the four corners.
                CopyBorderPixel(bitmap, (int) region.Left, (int) region.Top, (int) region.Left - 1, (int) region.Top - 1);
                CopyBorderPixel(bitmap, (int) region.Right - 1, (int) region.Top, (int) region.Right, (int) region.Top - 1);
                CopyBorderPixel(bitmap, (int) region.Left, (int) region.Bottom - 1, (int) region.Left - 1, (int) region.Bottom);
                CopyBorderPixel(bitmap, (int) region.Right - 1, (int) region.Bottom - 1, (int) region.Right, (int) region.Bottom);
            }
        }


        // Copies a single pixel within a bitmap, preserving RGB but forcing alpha to zero.
        static void CopyBorderPixel(/*PixelAccessor*/Bitmap bitmap, int sourceX, int sourceY, int destX, int destY)
        {
            DrawingColor color = bitmap.GetPixel(sourceX, sourceY);

            bitmap.SetPixel (destX, destY, new DrawingColor(color.Red, color.Green, color.Blue, 0));
        }

        
        // Converts a bitmap to the specified pixel format.
//         public static Bitmap ChangePixelFormat(Bitmap bitmap, PixelFormat format)
        public static Bitmap ChangePixelFormat(Bitmap bitmap, PixelFormat format)
        {
//             Rectangle bounds = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

//             return bitmap.Clone(bounds, format);
            return bitmap.Copy (format);
        }


        // Helper for locking a bitmap and efficiently reading or writing its pixels.
        public sealed class PixelAccessor : IDisposable
        {
            // Constructor locks the bitmap.
            public PixelAccessor(Bitmap bitmap/*, ImageLockMode mode*/, Rectangle? region = null)
            {
                this.bitmap = bitmap;

                Region = region.GetValueOrDefault(new Rectangle(0, 0, bitmap.Width, bitmap.Height));

//                data = bitmap.LockBits(Region, mode, PixelFormat.Format32bppArgb);
            }


            // Dispose unlocks the bitmap.
            public void Dispose()
            {
//                 if (data != null)
//                 {
//                     bitmap.UnlockBits(data);
// 
//                     data = null;
//                 }
            }


            // Query what part of the bitmap is locked.
            public Rectangle Region { get; private set; }


            // Get or set a pixel value.
//             public DrawingColor this[int x, int y]
//             {
//                 get
//                 {
//                     return new DrawingColor((uint) Marshal.ReadInt32(PixelAddress(x, y)));
//                 }
// 
//                 set
//                 {
//                     Marshal.WriteInt32(PixelAddress(x, y), (int) (uint) value); 
//                 }
//             }


            // Helper computes the address of the specified pixel.
//             unsafe IntPtr PixelAddress(int x, int y)
//             {
//                 return new IntPtr((byte*)data.Scan0 + (y * data.Stride) + (x * sizeof(int)));
//             }

            // Fields.
            Bitmap bitmap;
//             BitmapData data;
        }
    }
}
