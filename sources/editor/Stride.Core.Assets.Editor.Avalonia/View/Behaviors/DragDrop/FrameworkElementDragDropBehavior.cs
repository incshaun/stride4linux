// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;

using Stride.Core.Extensions;

namespace Stride.Core.Assets.Editor.View.Behaviors
{
    public class FrameworkElementDragDropBehavior : DragDropBehavior<Control, Control>
    {
        protected override IEnumerable<object> GetItemsToDrag(Control container)
        {
            return AssociatedObject.DataContext?.ToEnumerable<object>() ?? Enumerable.Empty<object>();
        }

        protected override Control GetContainer(object source)
        {
            return AssociatedObject;
        }
    }
}
