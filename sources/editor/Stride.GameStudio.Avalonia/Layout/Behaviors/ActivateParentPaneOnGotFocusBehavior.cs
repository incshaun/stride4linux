// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Avalonia;
using Avalonia.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Xaml.Interactivity;
using Stride.Core.Presentation.Extensions;
using Avalonia.Interactivity;

namespace Stride.GameStudio.Avalonia.Layout.Behaviors
{
    // TODO: this behavior was previously broken, it might work now (migration to AvalonDock) but has not been tested!
    public class ActivateParentPaneOnGotFocusBehavior : Behavior<Control>
    {
        protected override void OnAttached()
        {
//             AssociatedObject.GotKeyboardFocus += GotFocus;
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
//             AssociatedObject.GotKeyboardFocus -= GotFocus;
        }

        private void GotFocus(object sender, RoutedEventArgs e)
        {
//             var pane = AssociatedObject.FindVisualParentOfType<LayoutAnchorableControl>();
//             if (pane != null)
//             {
//                 pane.Model.IsActive = true;
//             }
        }
    }
}
