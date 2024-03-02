// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Avalonia;
using Avalonia.Controls;

using Avalonia.Xaml.Interactivity;

using Avalonia.Interactivity;

namespace Stride.Core.Presentation.Behaviors
{
    /// <summary>
    /// A <see cref="Behavior{T}"/> that support deferred attachement for a FrameworkElement derived class.
    /// In such a case, the attachement is delayed until the <see cref="FrameworkElement.Loaded"/> event is raised.
    /// </summary>
    /// <typeparam name="T">The type of instance to attach to.</typeparam>
    public abstract class DeferredBehaviorBase<T> : Behavior<T> where T : AvaloniaObject
    {
        /// <summary>
        /// Represents the <see cref="AttachOnEveryLoadedEvent"/> property.
        /// </summary>
        public static readonly StyledProperty<bool> AttachOnEveryLoadedEventProperty = StyledProperty<bool>.Register<DeferredBehaviorBase<T>, bool>(nameof(AttachOnEveryLoadedEvent), false);

        private bool currentlyLoaded;

        /// <summary>
        /// Gets or sets whether <see cref="OnAttachedAndLoaded"/> should be called each time the <see cref="FrameworkElement.Loaded"/> event is raised.
        /// </summary>
        public bool AttachOnEveryLoadedEvent { get { return (bool)GetValue(AttachOnEveryLoadedEventProperty); } set { SetValue(AttachOnEveryLoadedEventProperty, value); } }

        protected override void OnAttached()
        {
            base.OnAttached();

            var element = AssociatedObject as Control;

            if (element != null)
            {
                element.Loaded += AssociatedObjectLoaded;
                element.Unloaded += AssociatedObjectUnloaded;
            }

            if (element == null || element.IsLoaded)
            {
                currentlyLoaded = true;
                OnAttachedAndLoaded();
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (currentlyLoaded)
            {
                currentlyLoaded = false;
                OnDetachingAndUnloaded();
            }

            var element = AssociatedObject as Control;
            if (element != null)
            {
                element.Loaded -= AssociatedObjectLoaded;
                element.Unloaded -= AssociatedObjectUnloaded;
            }
        }

        protected virtual void OnAttachedAndLoaded()
        {
            // Intentionally does nothing
        }

        protected virtual void OnDetachingAndUnloaded()
        {
            // Intentionally does nothing
        }

        private void AssociatedObjectLoaded(object sender, RoutedEventArgs e)
        {
            if (!currentlyLoaded)
            {
                currentlyLoaded = true;
                OnAttachedAndLoaded();
            }
        }

        private void AssociatedObjectUnloaded(object sender, RoutedEventArgs e)
        {
            if (currentlyLoaded)
            {
                currentlyLoaded = false;
                OnDetachingAndUnloaded();
            }
        }
    }
}
