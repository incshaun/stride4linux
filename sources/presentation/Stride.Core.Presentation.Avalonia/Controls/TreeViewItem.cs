// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Specialized;
using System.Linq;
using Avalonia;
using Avalonia.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Stride.Core.Annotations;
using Stride.Core.Presentation.Extensions;

using Avalonia.Markup.Xaml.Templates;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using System.Collections.Generic;
using System.Collections.Specialized;
using Avalonia.Interactivity;
using Avalonia.Metadata;
using Avalonia.Media;

using TreeView = Stride.Core.Presentation.Controls.TreeView;

namespace Stride.Core.Presentation.Controls
{
    /// <summary>
    /// An item of the TreeView.
    /// </summary>
    public class TreeViewItem : ExpandableItemsControl
    {
        protected override Type StyleKeyOverride { get { return typeof(TreeViewItem); } }
        
        internal double ItemTopInTreeSystem; // for virtualization purposes
        internal int HierachyLevel;// for virtualization purposes

        /// <summary>
        /// Identifies the <see cref="IsEditable"/> dependency property.
        /// </summary>
        public static StyledProperty<bool> IsEditableProperty = StyledProperty<bool>.Register<TreeViewItem, bool>(nameof(IsEditable), true); // T8K
        /// <summary>
        /// Identifies the <see cref="IsEditing"/> dependency property.
        /// </summary>
        public static StyledProperty<bool> IsEditingProperty = StyledProperty<bool>.Register<TreeViewItem, bool>(nameof(IsEditing), false); // T8N
        /// <summary>
        /// Identifies the <see cref="IsSelected"/> dependency property.
        /// </summary>
        public static StyledProperty<bool> IsSelectedProperty = StyledProperty<bool>.Register<TreeViewItem, bool>(nameof(IsSelected), false); // T8K
        /// <summary>
        /// Identifies the <see cref="TemplateEdit"/> dependency property.
        /// </summary>
        public static StyledProperty<DataTemplate> TemplateEditProperty = StyledProperty<DataTemplate>.Register<TreeViewItem, DataTemplate>(nameof(TemplateEdit), null); // T8K
        /// <summary>
        /// Identifies the <see cref="TemplateSelectorEdit"/> dependency property.
        /// </summary>
        public static StyledProperty<IDataTemplate> TemplateSelectorEditProperty = StyledProperty<IDataTemplate>.Register<TreeViewItem, IDataTemplate>(nameof(TemplateSelectorEdit), null); // T8K
        /// <summary>
        /// Identifies the <see cref="Indentation"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> IndentationProperty = StyledProperty<double>.Register<TreeViewItem, double>(nameof(Indentation), 10.0); // T2

        static TreeViewItem()
		{
			IsEditingProperty.Changed.AddClassHandler<TreeViewItem>(OnIsEditingChanged);

            // FIXME  T31

//             var vPanel = new FrameworkElementFactory(typeof(VirtualizingTreePanel));
//             vPanel.SetValue(Panel.IsItemsHostProperty, true);
//             var vPanelTemplate = new ItemsPanelTemplate { VisualTree = vPanel };
//             ItemsPanelProperty.OverrideMetadata(typeof(TreeViewItem), new FrameworkPropertyMetadata(vPanelTemplate));
// 
//             KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(TreeViewItem), new FrameworkPropertyMetadata(KeyboardNavigationMode.Continue));
//             KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(TreeViewItem), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));
//             VirtualizingPanel.ScrollUnitProperty.OverrideMetadata(typeof(TreeViewItem), new FrameworkPropertyMetadata(ScrollUnit.Item));
//             IsTabStopProperty.OverrideMetadata(typeof(TreeViewItem), new FrameworkPropertyMetadata(false));
        }

        public TreeViewItem ()
        {
             Items.CollectionChanged += OnItemsViewCollectionChanged;
        }
        
        public bool IsEditable { get { return (bool)GetValue(IsEditableProperty); } set { SetValue(IsEditableProperty, value); } }

        public bool IsEditing { get { return (bool)GetValue(IsEditingProperty); } set { SetValue(IsEditingProperty, value); } }

        public double Indentation { get { return (double)GetValue(IndentationProperty); } set { SetValue(IndentationProperty, value); } }

        public bool IsSelected { get { return (bool)GetValue(IsSelectedProperty); } set { SetValue(IsSelectedProperty, value); } }

        public DataTemplate TemplateEdit { get { return (DataTemplate)GetValue(TemplateEditProperty); } set { SetValue(TemplateEditProperty, value); } }

        public IDataTemplate TemplateSelectorEdit { get { return (IDataTemplate)GetValue(TemplateSelectorEditProperty); } set { SetValue(TemplateSelectorEditProperty, value); } }

        [DependsOn("Indentation")]
        public double Offset => ParentTreeViewItem?.Offset + Indentation ?? 0;
        
        public TreeViewItem ParentTreeViewItem => ItemsControlFromItemContainer(this) as TreeViewItem;

        public TreeView ParentTreeView { get; internal set; }

        public new bool IsVisible
        {
            get
            {
                if (!base.IsVisible)
                    return false;
                var currentItem = ParentTreeViewItem;
                while (currentItem != null)
                {
                    if (!currentItem.IsExpanded) return false;
                    currentItem = currentItem.ParentTreeViewItem;
                }

                return true;
            }
        }

        private bool CanExpandOnInput => CanExpand && IsEnabled;

        // Synchronizes the value of the child's IsVirtualizing property with that of the parent's
        internal static void IsVirtualizingPropagationHelper([NotNull] AvaloniaObject parent, [NotNull] AvaloniaObject element)
        {
//             SynchronizeValue(VirtualizingStackPanel.IsVirtualizingProperty, parent, element);
//             SynchronizeValue(VirtualizingStackPanel.VirtualizationModeProperty, parent, element);
        }

        private static void SynchronizeValue([NotNull] AvaloniaProperty dp, [NotNull] AvaloniaObject parent, [NotNull] AvaloniaObject child)
        {
            var value = parent.GetValue(dp);
            child.SetValue(dp, value);
        }

        /// <inheritdoc/>
        protected override void PrepareContainerForItemOverride(Control element, object? item, int index)
		{
			base.PrepareContainerForItemOverride(element, item, index);
            RaiseEvent(new TreeViewItemEventArgs(TreeView.PrepareItemEvent, this, (TreeViewItem)element, item));
        }

        /// <inheritdoc/>
        protected override void ClearContainerForItemOverride(Control element)
        {
            RaiseEvent(new TreeViewItemEventArgs(TreeView.ClearItemEvent, this, (TreeViewItem)element, null));
            base.ClearContainerForItemOverride(element);
        }

        /// <summary>
        /// This method is invoked when the Items property changes.
        /// </summary>
        protected void OnItemsViewCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    ParentTreeView?.ClearObsoleteItems(e.OldItems);
                    break;

                case NotifyCollectionChangedAction.Reset:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Move:
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        private static void OnIsEditingChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var item = (TreeViewItem)d;
            var newValue = (bool)e.NewValue;
            if (newValue)
            {
                item.ParentTreeView?.StartEditing(item);
            }
            else
            {
                item.ParentTreeView?.StopEditing();
            }
        }

        /// <summary>
        ///     Returns true if the item is or should be its own container.
        /// </summary>
        /// <param name="item">The item to test.</param>
        /// <returns>true if its type matches the container type.</returns>
        protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
		{
			return NeedsContainer<TreeViewItem>(item, out recycleKey);
		}

        /// <summary>
        ///     Create or identify the element used to display the given item.
        /// </summary>
        /// <returns>The container.</returns>
        protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
        {
            return new TreeViewItem();
        }

        public override string ToString()
        {
            return DataContext != null ? $"{DataContext} ({base.ToString()})" : base.ToString();
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
		{
			base.OnApplyTemplate(e);

            if (ParentTreeView?.SelectedItems != null && ParentTreeView.SelectedItems.Contains(DataContext))
            {
                IsSelected = true;
            }
        }

        internal void ForceFocus()
        {
            if (!Focus())
            {
                Dispatcher.UIThread.InvokeAsync(new Action(() => Focus()), DispatcherPriority.Input);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Handled)
                return;

            switch (e.Key)
            {
                case Key.Add:
                    if (CanExpandOnInput && !IsExpanded)
                    {
                        SetCurrentValue(IsExpandedProperty, true);
                        e.Handled = true;
                    }
                    break;

                case Key.Subtract:
                    if (CanExpandOnInput && IsExpanded)
                    {
                        SetCurrentValue(IsExpandedProperty, false);
                        e.Handled = true;
                    }
                    break;

                case Key.Left:
                case Key.Right:
                    if (LogicalLeft(e.Key))
                    {
                        if (CanExpandOnInput && IsExpanded)
                        {
                            if (IsFocused)
                            {
                                SetCurrentValue(IsExpandedProperty, false);
                            }
                            else
                            {
                                Focus();
                            }
                        }
                        else
                        {
                            ParentTreeView.SelectParentFromKey();
                        }
                    }
                    else
                    {
                        if (CanExpandOnInput)
                        {
                            if (!IsExpanded)
                            {
                                SetCurrentValue(IsExpandedProperty, true);
                            }
                            else
                            {
                                ParentTreeView.SelectNextFromKey();
                            }
                        }
                    }
                    e.Handled = true;
                    break;

                case Key.Down:
                    ParentTreeView.SelectNextFromKey();
                    e.Handled = true;
                    break;

                case Key.Up:
                    ParentTreeView.SelectPreviousFromKey();
                    e.Handled = true;
                    break;

                case Key.F2:
                    e.Handled = StartEditing();
                    break;

                case Key.Escape:
                case Key.Return:
                    StopEditing();
                    e.Handled = true;
                    break;

                case Key.Space:
                    ParentTreeView.SelectCurrentBySpace();
                    e.Handled = true;
                    break;

                case Key.Home:
                    ParentTreeView.SelectFirst();
                    e.Handled = true;
                    break;

                case Key.End:
                    ParentTreeView.SelectLast();
                    e.Handled = true;
                    break;
            }
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
//             if (IsEditing)
//             {
//                 var newFocus = e.NewFocus as AvaloniaObject;
//                 if (ReferenceEquals(newFocus, this))
//                     return;
// 
//                 if (newFocus != null && !ReferenceEquals(global::Avalonia.VisualTree.VisualExtensions.FindAncestorOfType<TreeViewItem>((Visual) newFocus), this))
//                 {
                    StopEditing();
//                 }
//             }
        }

        private bool StartEditing()
        {
            if ((TemplateEdit != null || TemplateSelectorEdit != null) && IsFocused && IsEditable)
            {
                IsEditing = true;
                return true;
            }

            return false;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.Handled)
                return;

            var key = e.Key;
            switch (key)
            {
                case Key.Left:
                case Key.Right:
                case Key.Up:
                case Key.Down:
                case Key.Add:
                case Key.Subtract:
                case Key.Space:
                    var items = TreeViewElementFinder.FindAll(ParentTreeView, false);
                    var focusedItem = items.FirstOrDefault(x => x.IsFocused);

                    focusedItem?.BringIntoView(new Rect(1, 1, 1, 1));

                    break;
            }
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
        {
            //if (e.Property.Name == "IsEditing")
            //{
            //    if ((bool)e.NewValue == false)
            //    {
            //        StopEditing();
            //    }
            //    else
            //    {
            //        ParentTreeView.IsEditingManager.SetEditedObject(this);
            //    }
            //}

            if (ParentTreeView != null && e.Property.Name == nameof(IsSelected))
            {
                if (ParentTreeView.SelectedItems.Contains(DataContext) != IsSelected)
                {
                    ParentTreeView.SelectFromProperty(this, IsSelected);
                }
            }

            base.OnPropertyChanged(e);
        }

        private bool LogicalLeft(Key key)
        {
            bool invert = (FlowDirection == FlowDirection.RightToLeft);
            return (!invert && (key == Key.Left)) || (invert && (key == Key.Right));
        }

        private void StopEditing()
        {
            IsEditing = false;
        }
    }
}
