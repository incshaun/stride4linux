// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Xaml.Interactivity;
using Stride.Core.Assets.Editor.View.Controls;
using Stride.Core.Presentation.Controls;
using Stride.Core.Presentation.Extensions;

namespace Stride.Core.Assets.Editor.View.Behaviors
{
    public class BringSelectionToViewBehavior : Behavior<EditableContentListBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += SelectionChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.SelectionChanged -= SelectionChanged;
            base.OnDetaching();
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AssociatedObject.SelectedIndex >= 0)
            {
                var panel = global::Avalonia.VisualTree.VisualExtensions.FindDescendantOfType<VirtualizingTilePanel>(AssociatedObject);
                if (panel != null)
                {
                    panel.ScrollToIndexedItem(AssociatedObject.SelectedIndex);
                }
            }
        }
    }
}
