// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
using Avalonia.Media;
using Stride.Core.Assets.Editor.ViewModel;
using Stride.Core.Annotations;
// using Stride.Core.Presentation.Adorners;
using Stride.Core.Presentation.Extensions;
using Stride.Core.Translation;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace Stride.Core.Assets.Editor.View.Behaviors
{
    /// <summary>
    /// Base class for drag and drop behaviors.
    /// </summary>
    /// <typeparam name="TControl">The type the <see cref="Behavior{TControl}"/> can be attached to.</typeparam>
    /// <typeparam name="TContainer"></typeparam>
    public abstract class DragDropBehavior<TControl, TContainer> : Behavior<TControl>, IDragDropBehavior
        where TControl : Control
        where TContainer : Control
    {
		static DragDropBehavior ()
		{
			CanDragProperty.Changed.AddClassHandler<DragDropBehavior<TControl, TContainer>>(CanDragChanged);
			CanDropProperty.Changed.AddClassHandler<DragDropBehavior<TControl, TContainer>>(CanDropChanged);
			CanInsertProperty.Changed.AddClassHandler<DragDropBehavior<TControl, TContainer>>(CanInsertChanged);
		}

        public const double InsertThreshold = 4;

        /// <summary>
        /// Identifies the <see cref="CanDrag"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> CanDragProperty = StyledProperty<bool>.Register<DragDropBehavior<TControl, TContainer>, bool>(nameof(CanDrag), true);

        /// <summary>
        /// Identifies the <see cref="CanDrop"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> CanDropProperty = StyledProperty<bool>.Register<DragDropBehavior<TControl, TContainer>, bool>(nameof(CanDrop), true);

        /// <summary>
        /// Identifies the <see cref="CanInsert"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> CanInsertProperty = StyledProperty<bool>.Register<DragDropBehavior<TControl, TContainer>, bool>(nameof(CanInsert), false);

        /// <summary>
        /// Identifies the <see cref="DisplayDropAdorner"/> dependency property.
        /// </summary>
//         public static readonly StyledProperty<DisplayDropAdorner> DisplayDropAdornerProperty = StyledProperty<DisplayDropAdorner>.Register<DragDropBehavior<TControl, TContainer>, DisplayDropAdorner>(nameof(DisplayDropAdorner), DisplayDropAdorner.Never);

        /// <summary>
        /// Identifies the <see cref="DisplayInsertAdorner"/> dependency property.
        /// </summary>
//         public static readonly StyledProperty<bool> DisplayInsertAdornerProperty = StyledProperty<bool>.Register<DragDropBehavior<TControl, TContainer>, bool>(nameof(DisplayInsertAdorner), false);

        public bool CanDrag { get => (bool)GetValue(CanDragProperty); set => SetValue(CanDragProperty, value); }

        public bool CanDrop { get => (bool)GetValue(CanDropProperty); set => SetValue(CanDropProperty, value); }

        public bool CanInsert { get => (bool)GetValue(CanInsertProperty); set => SetValue(CanInsertProperty, value); }

//         public DisplayDropAdorner DisplayDropAdorner { get => (DisplayDropAdorner)GetValue(DisplayDropAdornerProperty); set => SetValue(DisplayDropAdornerProperty, value); }

//         public bool DisplayInsertAdorner { get => (bool)GetValue(DisplayInsertAdornerProperty); set => SetValue(DisplayInsertAdornerProperty, value); }

        public DataTemplate DragVisualTemplate { get; set; }

        public bool UsePreviewEvents { get; set; }

        protected Point DragStartPoint { get; private set; }

        protected object DragStartOriginalSource { get; set; }

        protected Window DragWindow { get; private set; }

        /// <summary>
        /// Indicates whether a dragging operation is in progress.
        /// </summary>
        protected bool IsDragging { get; private set; }

        /// <summary>
        /// Called when the <see cref="CanDrag"/> property has changed.
        /// </summary>
        private static void CanDragChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            ((DragDropBehavior<TControl, TContainer>)d).CanDragChanged();
        }

        /// <summary>
        /// Called when the <see cref="CanDrop"/> property has changed.
        /// </summary>
        private static void CanDropChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            ((DragDropBehavior<TControl, TContainer>)d).CanDropChanged();
        }

        /// <summary>
        /// Called when the <see cref="CanInsert"/> property has changed.
        /// </summary>
        private static void CanInsertChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            ((DragDropBehavior<TControl, TContainer>)d).CanInsertChanged();
        }

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            if (CanDrag)
                SubscribeToDragEvents();
            if (CanDrop || CanInsert)
                SubscribeToDropEvents();
//             DragDropAdornerManager.RegisterElement(AssociatedObject);
            base.OnAttached();
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            base.OnDetaching();
//             DragDropAdornerManager.UnregisterElement(AssociatedObject);
            UnsubscribeFromDragEvents();
            UnsubscribeFromDropEvents();
        }

        /// <summary>
        /// Occurs when the <see cref="CanDrag"/> property changed.
        /// </summary>
        protected virtual void CanDragChanged()
        {
            if (AssociatedObject == null)
                return;

            if (CanDrag)
                SubscribeToDragEvents();
            else
                UnsubscribeFromDragEvents();
        }

        /// <summary>
        /// Occurs when the <see cref="CanDrop"/> property changed.
        /// </summary>
        protected virtual void CanDropChanged()
        {
            if (AssociatedObject == null || CanInsert)
                return;

            if (CanDrop)
                SubscribeToDropEvents();
            else
                UnsubscribeFromDropEvents();
        }

        /// <summary>
        /// Occurs when the <see cref="CanInsert"/> property changed.
        /// </summary>
        protected virtual void CanInsertChanged()
        {
            if (AssociatedObject == null || CanDrop)
                return;

            if (CanInsert)
                SubscribeToDropEvents();
            else
                UnsubscribeFromDropEvents();
        }

        protected virtual bool CanInitializeDrag(object originalSource)
        {
            return true;
        }

        private void SubscribeToDragEvents()
        {
//             AssociatedObject.PreviewMouseLeftButtonDown += PreviewMouseLeftButtonDown;
//             AssociatedObject.PreviewMouseMove += PreviewMouseMove;
//             AssociatedObject.PreviewMouseUp += PreviewMouseUp;
//             if (UsePreviewEvents)
//             {
//                 AssociatedObject.PreviewDragLeave += OnDragLeave;
//             }
//             else
//             {
//                 AssociatedObject.DragLeave += OnDragLeave;
//             }
//             AssociatedObject.GiveFeedback += OnGiveFeedback;
        }

        private void SubscribeToDropEvents()
        {
//             AssociatedObject.AllowDrop = true;
//             if (UsePreviewEvents)
//             {
//                 AssociatedObject.PreviewDrop += OnDrop;
//                 AssociatedObject.PreviewDragOver += OnDragOver;
//             }
//             else
//             {
//                 AssociatedObject.Drop += OnDrop;
//                 AssociatedObject.DragOver += OnDragOver;
//             }
        }

        private void UnsubscribeFromDragEvents()
        {
//             AssociatedObject.GiveFeedback -= OnGiveFeedback;
//             if (UsePreviewEvents)
//             {
//                 AssociatedObject.PreviewDragLeave -= OnDragLeave;
//             }
//             else
//             {
//                 AssociatedObject.DragLeave -= OnDragLeave;
//             }
//             AssociatedObject.PreviewMouseMove -= PreviewMouseMove;
//             AssociatedObject.PreviewMouseLeftButtonDown -= PreviewMouseLeftButtonDown;
        }

        private void UnsubscribeFromDropEvents()
        {
//             if (UsePreviewEvents)
//             {
//                 AssociatedObject.PreviewDragOver -= OnDragOver;
//                 AssociatedObject.PreviewDrop -= OnDrop;
//             }
//             else
//             {
//                 AssociatedObject.DragOver -= OnDragOver;
//                 AssociatedObject.Drop -= OnDrop;
//             }
        }

//         protected DragDropEffects DoDragDrop(AvaloniaObject dragSource)
//         {
//             var data = InitializeDrag(DragStartOriginalSource);
//             if (data == null)
//                 return DragDropEffects.None;
// 
//             DragWindow = new DragWindow
//             {
//                 Content = new ContentControl { Content = data, ContentTemplate = DragVisualTemplate }
//             };
//             DragWindow.Show();
// 
//             try
//             {
//                 IsDragging = true;
//                 return DragDrop.DoDragDrop(dragSource, new DataObject(DragContainer.Format, data), DragDropEffects.All);
//             }
//             catch (COMException)
//             {
//                 return DragDropEffects.None;
//             }
//             finally
//             {
//                 IsDragging = false;
//                 if (DragWindow != null)
//                 {
//                     DragWindow.Close();
//                     DragWindow = null;
//                 }
//             }
//         }

//         [CanBeNull]
//         protected virtual TContainer GetContainer(object source)
//         {
//             var frameworkElement = source as Control;
//             var contentElement = source as FrameworkContentElement;
//             if (contentElement != null)
//             {
//                 frameworkElement = contentElement.Parent as Control;
//             }
//             return frameworkElement as TContainer ?? Avalonia.VisualTree.VisualExtensions.FindAncestorOfType<TContainer>((Visual) frameworkElement);
//         }

        [CanBeNull]
        protected virtual IAddChildViewModel GetDropTargetItem(TContainer container)
        {
            return container?.DataContext as IAddChildViewModel;
        }

        [CanBeNull]
        protected virtual IInsertChildViewModel GetInsertTargetItem(TContainer container, Point position, out InsertPosition insertPosition)
        {
            insertPosition = InsertPosition.Before;
            return null;
        }

        [NotNull]
        protected abstract IEnumerable<object> GetItemsToDrag(TContainer container);

//         [CanBeNull]
//         protected object InitializeDrag(object originalSource)
//         {
//             if (!CanDrag)
//                 return null;
// 
//             if (!CanInitializeDrag(originalSource))
//             {
//                 return null;
//             }
//             object data = null;
//             var container = GetContainer(originalSource);
//             var itemsToDrag = GetItemsToDrag(container).ToList();
//             if (itemsToDrag.Count > 0)
//             {
//                 var dragContainer = new DragContainer(itemsToDrag);
//                 data = dragContainer;
//             }
//             return data;
//         }

        protected bool OnDragLeave(IDataObject data)
        {
//             // Invalidate current drag status
//             var dragContainer = DragDropHelper.GetDragContainer(data);
//             if (dragContainer != null)
//             {
//                 dragContainer.IsAccepted = false;
//                 dragContainer.Message = DragDropBehavior.InvalidDropAreaMessage;
//             }
// 
//             var itemsToDrop = DragDropHelper.GetItemsToDrop(dragContainer, data as DataObject);
// 
//             if (itemsToDrop == null)
//                 return false;
// 
//             if (DragDropHelper.ShouldDisplayDropAdorner(DisplayDropAdorner, itemsToDrop))
//             {
//                 var localPos = AssociatedObject.GetCursorRelativePosition();
//                 if (localPos.X <= 0 || localPos.Y <= 0 || localPos.X >= AssociatedObject.ActualWidth || localPos.Y >= AssociatedObject.ActualHeight)
//                 {
//                     DragDropAdornerManager.SetAdornerState(AssociatedObject, HighlightAdornerState.Visible);
//                 }
//             }
// 
            return true;
        }

//         protected bool OnDragOver(TContainer container, Point position, IDataObject data, [NotNull] RoutedEventArgs e)
//         {
//             var dragContainer = DragDropHelper.GetDragContainer(data);
//             DragDropAdornerManager.ClearInsertAdorner();
// 
//             // Invalidate current drag status
//             if (dragContainer != null)
//             {
//                 dragContainer.IsAccepted = false;
//                 dragContainer.Message = DragDropBehavior.InvalidDropAreaMessage;
//             }
// 
//             // Check if we can drop and if we have a valid target.
//             if (container == null)
//                 return false;
// 
//             var itemsToDrop = DragDropHelper.GetItemsToDrop(dragContainer, data as DataObject);
// 
//             if (itemsToDrop == null)
//                 return false;
// 
//             string message;
// 
//             // Insertion "override" add, so let's check this first
//             if (CanInsert)
//             {
//                 InsertPosition insertPosition;
//                 var target = GetInsertTargetItem(container, position, out insertPosition);
//                 if (target != null && target.CanInsertChildren(itemsToDrop, insertPosition, ComputeModifiers(), out message))
//                 {
//                     if (dragContainer != null)
//                     {
//                         dragContainer.IsAccepted = true;
//                         dragContainer.Message = message;
//                     }
//                     // The event must be handled otherwise OnDrop won't be invoked
//                     e.Handled = true;
//                     DragDropAdornerManager.UpdateInsertAdorner(container, insertPosition);
//                     return true;
//                 }
//             }
// 
//             if (CanDrop)
//             {
//                 var target = GetDropTargetItem(container);
//                 if (target == null)
//                     return false;
// 
//                 var isAccepted = target.CanAddChildren(itemsToDrop, ComputeModifiers(), out message);
//                 if (dragContainer != null)
//                 {
//                     dragContainer.IsAccepted = isAccepted;
//                     dragContainer.Message = message;
//                 }
// 
//                 // The event must be handled otherwise OnDrop won't be invoked
//                 if (isAccepted)
//                     e.Handled = true;
// 
//                 if (DragDropHelper.ShouldDisplayDropAdorner(DisplayDropAdorner, itemsToDrop))
//                 {
//                     var adornerState = isAccepted ? HighlightAdornerState.HighlightAccept : HighlightAdornerState.HighlightRefuse;
//                     DragDropAdornerManager.SetAdornerState(AssociatedObject, adornerState);
//                 }
//                 return isAccepted;
//             }
// 
//             return false;
//         }

        protected bool OnDrop(TContainer container, Point position, IDataObject data, RoutedEventArgs e)
        {
//             DragDropAdornerManager.ClearInsertAdorner();
// 
//             if (DragWindow != null)
//             {
//                 DragWindow.Close();
//                 DragWindow = null;
//             }
// 
//             if (container == null)
//                 return false;
// 
//             if (CanInsert)
//             {
//                 // Check if we can drop and if we have a valid target.
//                 InsertPosition insertPosition;
//                 var target = GetInsertTargetItem(container, position, out insertPosition);
//                 if (target != null)
//                 {
//                     var dragContainer = DragDropHelper.GetDragContainer(data);
//                     var itemsToDrop = DragDropHelper.GetItemsToDrop(dragContainer, data as DataObject);
//                     string message;
// 
//                     if (itemsToDrop != null && target.CanInsertChildren(itemsToDrop, insertPosition, ComputeModifiers(), out message))
//                     {
//                         target.InsertChildren(itemsToDrop, insertPosition, ComputeModifiers());
//                         if (e != null) e.Handled = true;
//                         return true;
//                     }
//                 }
//             }
// 
//             if (CanDrop)
//             {
//                 // Check if we can drop and if we have a valid target.
//                 var target = GetDropTargetItem(container);
//                 if (target == null)
//                     return false;
// 
//                 var dragContainer = DragDropHelper.GetDragContainer(data);
//                 var itemsToDrop = DragDropHelper.GetItemsToDrop(dragContainer, data as DataObject);
//                 string message;
// 
//                 if (itemsToDrop != null && target.CanAddChildren(itemsToDrop, ComputeModifiers(), out message))
//                 {
//                     target.AddChildren(itemsToDrop, ComputeModifiers());
//                     if (e != null) e.Handled = true;
//                 }
//             }
// 
            return true;
        }

//         private void PreviewMouseUp(object sender, [NotNull] MouseButtonEventArgs e)
//         {
//             DragStartPoint = e.GetPosition(AssociatedObject);
//             DragStartOriginalSource = null;
//         }
// 
//         private void PreviewMouseLeftButtonDown(object sender, [NotNull] MouseButtonEventArgs e)
//         {
//             DragStartPoint = e.GetPosition(AssociatedObject);
//             DragStartOriginalSource = e.OriginalSource;
//         }
// 
//         private void PreviewMouseMove(object sender, [NotNull] MouseEventArgs e)
//         {
//             // Note: it is possible that multiple controls could sent an event during the same frame (e.g. a ContentControl and its content).
//             // The drag drop operation could be triggered by the first event so we need to prevent any subsequent event from trigerring another operation.
//             if (IsDragging || e.LeftButton != MouseButtonState.Pressed || DragStartOriginalSource == null)
//                 return;
// 
//             var delta = e.GetPosition(AssociatedObject) - DragStartPoint;
//             if (Math.Abs(delta.X) >= SystemParameters.MinimumHorizontalDragDistance || Math.Abs(delta.Y) >= SystemParameters.MinimumVerticalDragDistance)
//             {
//                 DoDragDrop((DependencyObject)sender);
//             }
//         }

//         private void OnGiveFeedback(object sender, GiveFeedbackEventArgs e)
//         {
//             if (DragWindow == null)
//                 return;
// 
//             // Get root window
//             var rootWindow = TopLevel.GetTopLevel((DependencyObject as Control) as Windowsender);
//             if (rootWindow == null)
//                 return;
// 
//             // Get position in WPF screen coordinates
//             var point = rootWindow.GetCursorScreenPosition();
//             // Get the relative DPI between the two windows
//             var rootDpi = VisualTreeHelper.GetDpi(rootWindow);
//             var dragDpi = VisualTreeHelper.GetDpi(DragWindow);
//             // Calculate relative DPI scale
//             var dpiRatioX = rootDpi.DpiScaleX / dragDpi.DpiScaleX;
//             var dpiRatioY = rootDpi.DpiScaleY / dragDpi.DpiScaleY;
//             // Move drag window accordingly
//             DragWindow.Left = (point.X + 16) * dpiRatioX;
//             DragWindow.Top = point.Y * dpiRatioY;
//         }

        private void OnDragOver(object sender, [NotNull] DragEventArgs e)
        {
//             e.Effects = DragDropEffects.None;
// 
//             // Check if we can drop and if we have a valid target.
//             var container = GetContainer(e.OriginalSource);
//             if (container == null)
//                 return;
// 
//             if (OnDragOver(container, e.GetPosition(container), e.Data, e))
//             {
//                 e.Effects = DragDropEffects.Move;
//             }
        }

        private void OnDrop(object sender, [NotNull] DragEventArgs e)
        {
//             var container = GetContainer(e.OriginalSource);
//             if (container == null)
//                 return;
// 
//             OnDrop(container, e.GetPosition(container), e.Data, e);
        }

        private void OnDragLeave(object sender, [NotNull] DragEventArgs e)
        {
            OnDragLeave(e.Data);
        }

//         private static AddChildModifiers ComputeModifiers()
//         {
//             var modifiers = AddChildModifiers.None;
//             if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
//                 modifiers |= AddChildModifiers.Ctrl;
//             if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
//                 modifiers |= AddChildModifiers.Shift;
//             if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
//                 modifiers |= AddChildModifiers.Alt;
//             return modifiers;
//         }
    }
}
