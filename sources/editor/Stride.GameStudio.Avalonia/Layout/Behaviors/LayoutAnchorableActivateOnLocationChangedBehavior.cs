// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Stride.Core.Assets.Editor.View.Behaviors;
// using AvalonDock.Layout;
using Dock.Model.Avalonia.Controls;

namespace Stride.GameStudio.Avalonia.Layout.Behaviors
{
    /// <summary>
    /// An implementation of the <see cref="ActivateOnLocationChangedBehavior{T}"/> for the <see cref="LayoutAnchorable"/> control.
    /// </summary>
    public class LayoutAnchorableActivateOnLocationChangedBehavior : ActivateOnLocationChangedBehavior<Document>
    {
        protected override void Activate()
        {
//             AssociatedObject.Show();
//             AssociatedObject.IsSelected = true;
        }
    }
}
