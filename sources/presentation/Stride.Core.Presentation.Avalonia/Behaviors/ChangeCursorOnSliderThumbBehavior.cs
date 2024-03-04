// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

using Stride.Core.Presentation.Extensions;

namespace Stride.Core.Presentation.Behaviors
{
    public class ChangeCursorOnSliderThumbBehavior : DeferredBehaviorBase<Slider>
    {
        protected override void OnAttachedAndLoaded()
        {
            var thumb = Avalonia.VisualTree.VisualExtensions.FindDescendantOfType<Thumb>(AssociatedObject);
            if (thumb != null)
                thumb.Cursor = new Cursor(StandardCursorType.SizeWestEast);

            base.OnAttachedAndLoaded();
        }
    }
}
