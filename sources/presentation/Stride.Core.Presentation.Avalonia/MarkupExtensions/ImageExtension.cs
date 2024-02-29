// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Stride.Core.Annotations;
using Stride.Core.Presentation.Themes;
using Avalonia.Media.Imaging;

namespace Stride.Core.Presentation.MarkupExtensions
{
    
    public class ImageExtension : MarkupExtension
    {
        private readonly IImage source;
        private readonly int width;
        private readonly int height;
        private readonly BitmapInterpolationMode scalingMode;

        public ImageExtension(IImage source)
        {
            this.source = source;
            width = -1;
            height = -1;
        }

        public ImageExtension(IImage source, int width, int height)
            : this(source, width, height,  BitmapInterpolationMode.Unspecified)
        {
        }

        public ImageExtension(IImage source, int width, int height, BitmapInterpolationMode scalingMode)
        {
            if (width < 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height < 0) throw new ArgumentOutOfRangeException(nameof(height));
            this.source = source;
            this.width = width;
            this.height = height;
            this.scalingMode = scalingMode;
        }

        [NotNull]
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var image = new Image { Source = source };
            if (source is DrawingImage drawingImage)
            {
                image.Source = new DrawingImage()
                {
                    Drawing = ImageThemingUtilities.TransformDrawing((source as DrawingImage)?.Drawing, ThemeController.CurrentTheme.GetThemeBase().GetIconTheme())
                };
            }

             RenderOptions.SetBitmapInterpolationMode(image, scalingMode);
            if (width >= 0 && height >= 0)
            {
                image.Width = width;
                image.Height = height;
            }
            return image;
        }
    }
}
