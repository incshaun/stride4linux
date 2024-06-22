// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Stride.Core.Assets.Templates;
using Stride.Core.Extensions;
using Stride.Core.IO;
using Stride.Core.Presentation.ViewModels;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Stride.Core.Assets.Editor.Components.TemplateDescriptions.ViewModels
{
    public class TemplateDescriptionHelper
    {
        [ModuleInitializer]
        internal static void TemplateDescriptionHelperInit ()
        {
            TemplateDescriptionViewModel.bitmapLoadFunction = LoadImage;
        }
        
        private static Bitmap LoadImage(string path)
        {
            try
            {
                if (!path.StartsWith("pack:", StringComparison.Ordinal) && !File.Exists(path))
                {
                    return null;
                }

                if (path.Equals ("pack://application:,,,/Stride.Core.Assets.Editor;component/Resources/Images/default-template-icon.png"))
                {
                    return new Bitmap (AssetLoader.Open(new Uri("avares://Stride.Core.Assets.Editor.Avalonia/Resources/Images/default-template-icon.png")));
                }

                return new Bitmap(path);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
