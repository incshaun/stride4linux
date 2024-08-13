// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using Avalonia;
using Avalonia.Controls;

using Avalonia.Input;
using Avalonia.Threading;
using Stride.Core.Annotations;
using Stride.Assets.Presentation.AssetEditors.AssetCompositeGameEditor.ViewModels;
using Avalonia.Interactivity;
using System.Windows.Input;
using Stride.Core.Presentation.Commands;

namespace Stride.Assets.Presentation.AssetEditors.AssetCompositeGameEditor.Views
{
    using TreeView = Stride.Core.Presentation.Controls.TreeView;
    using TreeViewItem = Stride.Core.Presentation.Controls.TreeViewItem;

    internal static class AssetCompositeHierarchyTreeViewHelper
    {
        static AssetCompositeHierarchyTreeViewHelper()
        {
//             CommandManager.RegisterClassCommandBinding(typeof(TreeView), new CommandBinding(CollapseAllItems, OnCollapseAllItems));
//             CommandManager.RegisterClassCommandBinding(typeof(TreeView), new CommandBinding(ExpandAllItems, OnExpandAllItems));
        }

        public static ICommand CollapseAllItems { get; } = new RoutedCommand<TreeView>(OnCollapseAllItems);

        public static ICommand ExpandAllItems { get; } = new RoutedCommand<TreeView>(OnExpandAllItems);

        private static void OnCollapseAllItems(TreeView sender)
        {
            var treeView = (TreeView)sender;
            for (var i = 0; i < treeView.Items.Count; i++)
            {
                var viewItem = treeView.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItem;
                if (viewItem == null)
                    continue;

                CollapseElementRecursively(viewItem);
            }
            // Reset scrolling to the top element
            (treeView.ItemContainerGenerator.ContainerFromIndex(0) as Control)?.BringIntoView();
        }

        private static void OnExpandAllItems(TreeView sender)
        {
            var treeView = (TreeView)sender;
            foreach (AssetCompositeItemViewModel item in treeView.Items)
            {
                ExpandElementRecursively(treeView, item);
            }
        }

        private static void CollapseElementRecursively([NotNull] TreeViewItem viewItem)
        {
            for (var i = 0; i < viewItem.Items.Count; i++)
            {
                var child = viewItem.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItem;
                if (child == null)
                    continue;

                CollapseElementRecursively(child);
            }
            viewItem.IsExpanded = false;
        }

        private static void ExpandElementRecursively([NotNull] TreeView treeView, AssetCompositeItemViewModel item)
        {
            // Since treeview can have virtualization and also a lot of items, we dispatch the recursive expansion amongst successive frames
            Dispatcher.UIThread.InvokeAsync(new Action(() =>
            {
                var viewItem = treeView.GetTreeViewItemFor(item);
                if (viewItem == null)
                    return;

                viewItem.IsExpanded = true;
                foreach (var child in item.EnumerateChildren())
                {
                    ExpandElementRecursively(treeView, child);
                }
            }), DispatcherPriority.ApplicationIdle);
        }
    }
}
