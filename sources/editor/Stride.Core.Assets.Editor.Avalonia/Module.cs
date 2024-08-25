// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System.Reflection;
using Stride.Core.Assets.Editor.Services;
using Stride.Core.Reflection;
using Stride.Core.Translation;
using Stride.Core.Translation.Providers;
using Avalonia;

namespace Stride.Core.Assets.Editor.Avalonia
{
    internal class Module
    {
        [ModuleInitializer]
        public static void Initialize()
        {
            Presentation.Quantum.ViewModels.NodeViewModel.UnsetValue = AvaloniaProperty.UnsetValue;
            
            ResourceThumbnailData.Initialize ();
            BitmapThumbnailData.Initialize ();
        }
    }
}
