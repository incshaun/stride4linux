// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
using Stride.Core.Presentation.Extensions;
using Avalonia.Interactivity;

namespace Stride.Core.Assets.Editor.View.Behaviors
{
    public class ListBoxHighlightedItemBehavior : Behavior<ListBox>
    {
		static ListBoxHighlightedItemBehavior()
		{
			SelectHighlightedWhenEnteringControlProperty.Changed.AddClassHandler<ListBoxHighlightedItemBehavior>(SelectHighlightedWhenEnteringControlChanged);
		}

        /// <summary>
        /// Identifies the <see cref="IsEnabled"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> IsEnabledProperty = StyledProperty<bool>.Register<ListBoxHighlightedItemBehavior, bool>(nameof(IsEnabled), true); // T2

        /// <summary>
        /// Identifies the <see cref="HighlightedItem"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<object> HighlightedItemProperty = StyledProperty<object>.Register<ListBoxHighlightedItemBehavior, object>(nameof(HighlightedItem)); // T1

        /// <summary>
        /// Identifies the <see cref="UseSelectedItemIfAvailable"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> UseSelectedItemIfAvailableProperty = StyledProperty<bool>.Register<ListBoxHighlightedItemBehavior, bool>(nameof(UseSelectedItemIfAvailable)); // T1

        /// <summary>
        /// Identifies the <see cref="SelectHighlightedWhenEnteringControl"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<Control> SelectHighlightedWhenEnteringControlProperty = StyledProperty<Control>.Register<ListBoxHighlightedItemBehavior, Control>(nameof(SelectHighlightedWhenEnteringControl)); // T4A

        /// <summary>
        /// Identifies the <see cref="DelayToUpdate"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> DelayToUpdateProperty = StyledProperty<double>.Register<ListBoxHighlightedItemBehavior, double>(nameof(DelayToUpdate)); // T1

        private ListBoxItem lastHoveredItem;
        private int lastHoveredItemId;

        public bool IsEnabled { get { return (bool)GetValue(IsEnabledProperty); } set { SetValue(IsEnabledProperty, value); } }

        public object HighlightedItem { get { return GetValue(HighlightedItemProperty); } set { SetValue(HighlightedItemProperty, value); } }

        public bool UseSelectedItemIfAvailable { get { return (bool)GetValue(UseSelectedItemIfAvailableProperty); } set { SetValue(UseSelectedItemIfAvailableProperty, value); } }

        public Control SelectHighlightedWhenEnteringControl { get { return (Control)GetValue(SelectHighlightedWhenEnteringControlProperty); } set { SetValue(SelectHighlightedWhenEnteringControlProperty, value); } }

        public double DelayToUpdate { get { return (double)GetValue(DelayToUpdateProperty); } set { SetValue(DelayToUpdateProperty, value); } }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PointerMoved += TimedUpdateHighlightedItem;
            AssociatedObject.SelectionChanged += UpdateHighlightedItem;
            if (SelectHighlightedWhenEnteringControl != null)
            {
                SelectHighlightedWhenEnteringControl.PointerEntered += MouseEnter;
                SelectHighlightedWhenEnteringControl.PointerExited += MouseLeave;

            }
        }

        protected override void OnDetaching()
        {
            if (SelectHighlightedWhenEnteringControl != null)
            {
                SelectHighlightedWhenEnteringControl.PointerEntered -= MouseEnter;
                SelectHighlightedWhenEnteringControl.PointerExited -= MouseLeave;

            }
            AssociatedObject.SelectionChanged -= UpdateHighlightedItem;
            AssociatedObject.PointerMoved -= TimedUpdateHighlightedItem;
            base.OnDetaching();
        }

        private void UpdateHighlightedItem(object sender, RoutedEventArgs e)
        {
            if (!IsEnabled)
                return;

            var item = GetHoveredItem(UseSelectedItemIfAvailable, e.Source);
            if (item != null)
            {
                UpdateLastHoveredItem(item);
                SetCurrentValue(HighlightedItemProperty, item.DataContext);
            }
        }

        private async void TimedUpdateHighlightedItem(object sender, PointerEventArgs e)
        {
            if (!IsEnabled)
                return;

            ListBoxItem hoveredItem = GetHoveredItem(UseSelectedItemIfAvailable, e.Source);
            // Keep track of the last hovered item in time, for when this handler goes async
            UpdateLastHoveredItem(hoveredItem);
            int hoveredId = lastHoveredItemId;

            if (hoveredItem == null)
                return;

            // Set the highlighted item immediately if we don't have a delay to wait
            if (DelayToUpdate <= 0)
            {
                SetCurrentValue(HighlightedItemProperty, hoveredItem.DataContext);
                return;
            }

            await Task.Delay(TimeSpan.FromSeconds(DelayToUpdate));

            // Check if the same item is still hovered after the delay
            if (hoveredId == lastHoveredItemId)
            {
                SetCurrentValue(HighlightedItemProperty, hoveredItem.DataContext);
            }
        }

        private void UpdateLastHoveredItem(ListBoxItem hoveredItem)
        {
            if (!ReferenceEquals(hoveredItem, lastHoveredItem))
            {
                lastHoveredItem = hoveredItem;
                ++lastHoveredItemId;
            }
        }

        private ListBoxItem GetHoveredItem(bool useSelectedItemIfAvailable, object originalSource)
        {
            ListBoxItem item = null;
            // First get the selected item, if available
            if (useSelectedItemIfAvailable && AssociatedObject.SelectedItem != null)
            {
                item = (ListBoxItem)AssociatedObject.ContainerFromIndex(AssociatedObject.SelectedIndex);
            }
            // If nothing is selected or if we don't want the selected item, get the hovered item
            if (item == null)
            {
                var frameworkElement = originalSource as Control;
                var contentElement = originalSource as Control; //FrameworkContentElement;
                if (contentElement != null)
                {
                    frameworkElement = contentElement.Parent as Control;
                }
                item = frameworkElement as ListBoxItem ?? global::Avalonia.VisualTree.VisualExtensions.FindAncestorOfType<ListBoxItem>((Visual) frameworkElement);
            }
            return item;
        }

        private static void SelectHighlightedWhenEnteringControlChanged(AvaloniaObject d,AvaloniaPropertyChangedEventArgs e)
        {
            var behavior = (ListBoxHighlightedItemBehavior)d;
            if (e.OldValue != null)
            {
                ((Control)e.OldValue).PointerEntered -= behavior.MouseEnter;
                ((Control)e.OldValue).PointerExited -= behavior.MouseLeave;
            }
            if (e.NewValue != null)
            {
                ((Control)e.NewValue).PointerEntered += behavior.MouseEnter;
                ((Control)e.NewValue).PointerExited += behavior.MouseLeave;
            }
        }

        private void MouseEnter(object sender, PointerEventArgs e)
        {
            if (!IsEnabled)
                return;

            AssociatedObject.SetCurrentValue(SelectingItemsControl.SelectedItemProperty, HighlightedItem);
            UpdateLastHoveredItem(null);
        }

        private void MouseLeave(object sender, PointerEventArgs e)
        {
            if (!IsEnabled)
                return;

            AssociatedObject.SetCurrentValue(SelectingItemsControl.SelectedItemProperty, null);
            UpdateLastHoveredItem(null);
        }
    }
}
