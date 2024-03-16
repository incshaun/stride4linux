// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;

using Stride.Core.Presentation.Collections;
using Stride.Core.Presentation.Extensions;
using Avalonia.Interactivity;
using System;
using Avalonia.Data;

namespace Stride.Core.Presentation.Controls
{
    public class PropertyView : Avalonia.Controls.TreeView 
    {
        private readonly ObservableList<PropertyViewItem> properties = new ObservableList<PropertyViewItem>();

        /// <summary>
        /// Identifies the <see cref="HighlightedItem"/> dependency property.
        /// </summary>
        public static readonly DirectProperty<PropertyView, PropertyViewItem> HighlightedItemPropertyKey = AvaloniaProperty.RegisterDirect<PropertyView, PropertyViewItem>(nameof (HighlightedItem), o => o.HighlightedItem);

        /// <summary>
        /// Identifies the <see cref="HoveredItem"/> dependency property.
        /// </summary>
        public static readonly DirectProperty<PropertyView, PropertyViewItem> HoveredItemPropertyKey = AvaloniaProperty.RegisterDirect<PropertyView, PropertyViewItem>(nameof (HoveredItem), o => o.HoveredItem);

        /// <summary>
        /// Identifies the <see cref="KeyboardActiveItem"/> dependency property.
        /// </summary>
        public static readonly DirectProperty<PropertyView, PropertyViewItem> KeyboardActiveItemPropertyKey = AvaloniaProperty.RegisterDirect<PropertyView, PropertyViewItem>(nameof (KeyboardActiveItem), o => o.KeyboardActiveItem);

        /// <summary>
        /// Identifies the <see cref="NameColumnSize"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<GridLength> NameColumnSizeProperty = StyledProperty<GridLength>.Register<PropertyView, GridLength>(nameof(NameColumnSize), new GridLength(150), defaultBindingMode : BindingMode.TwoWay);

        /// <summary>
        /// Identifies the PreparePropertyItem event.
        /// This attached routed event may be raised by the PropertyGrid itself or by a PropertyItemBase containing sub-items.
        /// </summary>
        public static readonly RoutedEvent PrepareItemEvent = RoutedEvent.Register<PropertyView, RoutedEventArgs>("PrepareItem", RoutingStrategies.Bubble);

        /// <summary>
        /// Identifies the ClearPropertyItem event.
        /// This attached routed event may be raised by the PropertyGrid itself or by a
        /// PropertyItemBase containing sub items.
        /// </summary>
        public static readonly RoutedEvent ClearItemEvent = RoutedEvent.Register<PropertyView, RoutedEventArgs>("ClearItem", RoutingStrategies.Bubble);

        static PropertyView()
        {
            
        }

        public PropertyView()
        {
//             IsKeyboardFocusWithinChanged += OnIsKeyboardFocusWithinChanged;
        }

        public IReadOnlyCollection<PropertyViewItem> Properties => properties;

        /// <summary>
        /// Gets the <see cref="PropertyViewItem"/> that is currently highlighted by the mouse cursor.
        /// </summary>
        private PropertyViewItem _HighlightedItem;
		public PropertyViewItem HighlightedItem { get { return _HighlightedItem; } private set { SetAndRaise(HighlightedItemPropertyKey, ref _HighlightedItem, value); } }

        /// <summary>
        /// Gets the <see cref="PropertyViewItem"/> that is currently hovered by the mouse cursor.
        /// </summary>
        private PropertyViewItem _HoveredItem;
		public PropertyViewItem HoveredItem { get { return _HoveredItem; } private set { SetAndRaise(HoveredItemPropertyKey, ref _HoveredItem, value); } }

        /// <summary>
        /// Gets the <see cref="PropertyViewItem"/> that currently owns the control who have the keyboard focus.
        /// </summary>
        private PropertyViewItem _KeyboardActiveItem;
		public PropertyViewItem KeyboardActiveItem { get { return _KeyboardActiveItem; } private set { SetAndRaise(KeyboardActiveItemPropertyKey, ref _KeyboardActiveItem, value); } }

        /// <summary>
        /// Gets or sets the shared size of the 'Name' column.
        /// </summary>
        public GridLength NameColumnSize { get { return (GridLength)GetValue(NameColumnSizeProperty); } set { SetValue(NameColumnSizeProperty, value); } }

        /// <summary>
        /// This event is raised when a property item is about to be displayed in the PropertyGrid.
        /// This allow the user to customize the property item just before it is displayed.
        /// </summary>
        public event EventHandler<PropertyViewItemEventArgs> PrepareItem { add { AddHandler(PrepareItemEvent, value); } remove { RemoveHandler(PrepareItemEvent, value); } }

        /// <summary>
        /// This event is raised when an property item is about to be remove from the display in the PropertyGrid
        /// This allow the user to remove any attached handler in the PreparePropertyItem event.
        /// </summary>
        public event EventHandler<PropertyViewItemEventArgs> ClearItem { add { AddHandler(ClearItemEvent, value); } remove { RemoveHandler(ClearItemEvent, value); } }

        internal void ItemMouseMove(object sender, PointerEventArgs e)
        {
            var item = sender as PropertyViewItem;
            if (item != null)
            {
                if (item.Highlightable)
                    HighlightItem(item);

                HoverItem(item);
            }
        }

        internal void OnIsKeyboardFocusWithinChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (Equals(sender, this) && !(bool)e.NewValue)
            {
                KeyboardActivateItem(null);
                return;
            }

            // We want to find the closest PropertyViewItem to the element who got the keyboard focus.
            var focusedControl = TopLevel.GetTopLevel(this).FocusManager.GetFocusedElement () as AvaloniaObject;
            if (focusedControl != null)
            {
                var propertyItem = focusedControl as PropertyViewItem ?? Avalonia.VisualTree.VisualExtensions.FindAncestorOfType<PropertyViewItem>((Visual) focusedControl);
                if (propertyItem != null)
                {
                    KeyboardActivateItem(propertyItem);
                }
            }
        }

        protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
        {
            base.OnPointerCaptureLost(e);
            HoverItem(null);
            HighlightItem(null);
        }

        protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
        {
            return new PropertyViewItem(this);
        }

        protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
		{
			return NeedsContainer<PropertyViewItem>(item, out recycleKey);
		}

        protected override void PrepareContainerForItemOverride(Control element, object? item, int index)
		{
			base.PrepareContainerForItemOverride(element, item, index);
            var container = (PropertyViewItem)element;
            properties.Add(container);
            RaiseEvent(new PropertyViewItemEventArgs(PrepareItemEvent, this, container, item));
        }

        protected override void ClearContainerForItemOverride(Control element)
        {
            var container = (PropertyViewItem)element;
            RaiseEvent(new PropertyViewItemEventArgs(ClearItemEvent, this, (PropertyViewItem)element, null));
            properties.Remove(container);
            base.ClearContainerForItemOverride(element);
        }

        private void KeyboardActivateItem(PropertyViewItem item)
        {
            KeyboardActiveItem?.SetValue(PropertyViewItem.IsKeyboardActivePropertyKey, false);
            KeyboardActiveItem = item;
            KeyboardActiveItem?.SetValue(PropertyViewItem.IsKeyboardActivePropertyKey, true);
        }

        private void HoverItem(PropertyViewItem item)
        {
            HoveredItem?.SetValue(PropertyViewItem.IsHoveredPropertyKey, false);
            HoveredItem = item;
            HoveredItem?.SetValue(PropertyViewItem.IsHoveredPropertyKey, true);
        }

        private void HighlightItem(PropertyViewItem item)
        {
            HighlightedItem?.SetValue(PropertyViewItem.IsHighlightedPropertyKey, false);
            HighlightedItem = item;
            HighlightedItem?.SetValue(PropertyViewItem.IsHighlightedPropertyKey, true);
        }


        //protected override AutomationPeer OnCreateAutomationPeer()
        //{
        //    return (AutomationPeer)new TreeViewAutomationPeer(this);
        //}
    }
}
