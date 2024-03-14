// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;

using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
using Stride.Core.Annotations;

using NotNullAttribute = Stride.Core.Annotations.NotNullAttribute;

namespace Stride.Core.Presentation.Behaviors
{
    /// <summary>
    /// Base class for behaviors that capture the mouse.
    /// </summary>
    /// <typeparam name="TElement"></typeparam>
    public abstract class MouseMoveCaptureBehaviorBase<TElement> : Behavior<TElement>
        where TElement : Control
    {
		static MouseMoveCaptureBehaviorBase ()
		{
			IsEnabledProperty.Changed.AddClassHandler<MouseMoveCaptureBehaviorBase<TElement>>(IsEnabledChanged);
			UsePreviewEventsProperty.Changed.AddClassHandler<MouseMoveCaptureBehaviorBase<TElement>>(UsePreviewEventsChanged);
		}

        /// <summary>
        /// Identifies the <see cref="IsEnabled"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> IsEnabledProperty = StyledProperty<bool>.Register<MouseMoveCaptureBehaviorBase<TElement>, bool>(nameof(IsEnabled), true); // T5

        /// <summary>
        /// Identifies the <see cref="IsInProgress"/> dependency property key.
        /// </summary>
        protected static readonly DirectProperty<MouseMoveCaptureBehaviorBase<TElement>, bool> IsInProgressPropertyKey = AvaloniaProperty.RegisterDirect<MouseMoveCaptureBehaviorBase<TElement>, bool>(nameof (IsInProgress), o => o.IsInProgress); // T10A

        /// <summary>
        /// Identifies the <see cref="IsInProgress"/> dependency property.
        /// </summary>
        [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
        public static readonly AvaloniaProperty IsInProgressProperty = IsInProgressPropertyKey;

        /// <summary>
        /// Identifies the <see cref="Modifiers"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<KeyModifiers?> ModifiersProperty = StyledProperty<KeyModifiers?>.Register<MouseMoveCaptureBehaviorBase<TElement>, KeyModifiers?>(nameof(Modifiers), null); // T2

        /// <summary>
        /// Identifies the <see cref="UsePreviewEvents"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> UsePreviewEventsProperty = StyledProperty<bool>.Register<MouseMoveCaptureBehaviorBase<TElement>, bool>(nameof(UsePreviewEvents), false); // T5
        
        /// <summary>
        /// <c>true</c> if this behavior is enabled; otherwise, <c>false</c>.
        /// </summary>
        public bool IsEnabled { get { return (bool)GetValue(IsEnabledProperty); } set { SetValue(IsEnabledProperty, value); } }

        /// <summary>
        /// <c>true</c> if an operation is in progress; otherwise, <c>false</c>.
        /// </summary>
        public bool IsInProgress { get { return (bool)GetValue(IsInProgressProperty); } private set { SetValue(IsInProgressPropertyKey, value); } }

        public KeyModifiers? Modifiers { get { return (KeyModifiers?)GetValue(ModifiersProperty); } set { SetValue(ModifiersProperty, value); } }

        public bool UsePreviewEvents
        {
            get { return (bool)GetValue(UsePreviewEventsProperty); }
            set { SetValue(UsePreviewEventsProperty, value); }
        }

        private static void IsEnabledChanged([NotNull] AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var behavior = (MouseMoveCaptureBehaviorBase<TElement>)d;
            if ((bool)e.NewValue != true)
            {
                behavior.Cancel();
            }
        }

        private static void UsePreviewEventsChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var behavior = (MouseMoveCaptureBehaviorBase<TElement>)d;
            behavior.UnsubscribeFromMouseEvents((bool)e.OldValue);
            behavior.SubscribeToMouseEvents((bool)e.NewValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool AreModifiersValid()
        {
//             return Modifiers == null || (Modifiers == KeyModifiers.None ? Keyboard.Modifiers == KeyModifiers.None : Keyboard.Modifiers.HasFlag(Modifiers));
return true;            
        }

        protected void Cancel()
        {
            if (!IsInProgress)
                return;

            ReleaseMouseCapture();
            CancelOverride();
        }

        protected virtual void CancelOverride()
        {
            // Default implementation does nothing
        }

        /// <summary>
        /// Captures the mouse to the <see cref="Behavior{TElement}.AssociatedObject"/>.
        /// </summary>
        protected void CaptureMouse()
        {
            AssociatedObject.Focus();
//             AssociatedObject.CaptureMouse();
            IsInProgress = true;
        }

        ///  <inheritdoc/>
        protected override void OnAttached()
        {
            SubscribeToMouseEvents(UsePreviewEvents);
//             AssociatedObject.PreviewMouseUp += MouseUp;
//             AssociatedObject.LostMouseCapture += OnLostMouseCapture;
        }

        ///  <inheritdoc/>
        protected override void OnDetaching()
        {
            UnsubscribeFromMouseEvents(UsePreviewEvents);
//             AssociatedObject.PreviewMouseUp -= MouseUp;
//             AssociatedObject.LostMouseCapture -= OnLostMouseCapture;
        }

        protected abstract void OnPointerPressed([NotNull] PointerPressedEventArgs e);

        protected abstract void OnPointerMoved([NotNull] PointerEventArgs e);

        protected abstract void OnPointerReleased([NotNull] PointerReleasedEventArgs e);

        /// <summary>
        /// Releases the mouse capture, if the <see cref="Behavior{TElement}.AssociatedObject"/> held the capture. 
        /// </summary>
        protected void ReleaseMouseCapture()
        {
            IsInProgress = false;
            if (AssociatedObject.IsPointerOver)
            {
//                 AssociatedObject.ReleaseMouseCapture();
            }
        }

        private void MouseDown(object sender, [NotNull] PointerPressedEventArgs e)
        {
            if (!IsEnabled || IsInProgress)
                return;

            OnPointerPressed(e);
        }

        private void MouseMove(object sender, [NotNull] PointerEventArgs e)
        {
            if (!IsEnabled || !IsInProgress)
                return;

            OnPointerMoved(e);
        }

        private void MouseUp(object sender, [NotNull] PointerReleasedEventArgs e)
        {
            if (!IsEnabled || !IsInProgress || !AssociatedObject.IsPointerOver)
                return;

            OnPointerReleased(e);
        }

        private void OnLostMouseCapture(object sender, [NotNull] PointerEventArgs e)
        {
//             if (!ReferenceEquals(Mouse.Captured, sender))
//             {
//                 Cancel();
//             }
        }

        private void SubscribeToMouseEvents(bool usePreviewEvents)
        {
            if (AssociatedObject == null)
                return;

            if (usePreviewEvents)
            {
//                 AssociatedObject.PreviewMouseDown += MouseDown;
//                 AssociatedObject.PreviewMouseMove += MouseMove;
            }
            else
            {
//                 AssociatedObject.PointerPressed += MouseDown;
//                 AssociatedObject.PointerMoved += MouseMove;
            }
        }

        private void UnsubscribeFromMouseEvents(bool usePreviewEvents)
        {
            if (AssociatedObject == null)
                return;

            if (usePreviewEvents)
            {
//                 AssociatedObject.PreviewMouseDown -= MouseDown;
//                 AssociatedObject.PreviewMouseMove -= MouseMove;
            }
            else
            {
//                 AssociatedObject.PointerPressed -= MouseDown;
//                 AssociatedObject.PointerMoved -= MouseMove;
            }
        }
    }
}
