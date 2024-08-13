// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Avalonia;
using Avalonia.Controls;

using Stride.Editor.Annotations;
using Stride.Editor.Preview.View;

namespace Stride.Assets.Presentation.Preview.Views
{
    [AssetPreviewView<HeightmapPreview>]
    public class HeightmapPreviewView : StridePreviewView
    {
        static HeightmapPreviewView()
        {
            // FIXME  T31
        }
    }
}
