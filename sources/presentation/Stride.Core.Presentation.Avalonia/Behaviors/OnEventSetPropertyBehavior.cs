// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Avalonia;
using Avalonia.Controls;


namespace Stride.Core.Presentation.Behaviors
{
    /// <summary>
    /// An implementation of the <see cref="OnEventBehavior"/> class that allows to set the value of a dependency property
    /// on the control hosting this behavior when a specific event is raised.
    /// </summary>
    public class OnEventSetPropertyBehavior : OnEventBehavior
    {
        /// <summary>
        /// Identifies the <see cref="Property"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<AvaloniaProperty> PropertyProperty = StyledProperty<AvaloniaProperty>.Register<OnEventSetPropertyBehavior, AvaloniaProperty>("Property"); // T1

        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<object> ValueProperty = StyledProperty<object>.Register<OnEventSetPropertyBehavior, object>("Value"); // T1

        /// <summary>
        /// Identifies the <see cref="Target"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<AvaloniaObject> TargetProperty = StyledProperty<AvaloniaObject>.Register<OnEventSetPropertyBehavior, AvaloniaObject>("Target"); // T1

        /// <summary>
        /// Gets or sets the <see cref="DependencyProperty"/> to set when the event is raised.
        /// </summary>
        public AvaloniaProperty Property { get { return (AvaloniaProperty)GetValue(PropertyProperty); } set { SetValue(PropertyProperty, value); } }

        /// <summary>
        /// Gets or sets the value to set when the event is raised.
        /// </summary>
        public object Value { get { return GetValue(ValueProperty); } set { SetValue(ValueProperty, value); } }

        /// <summary>
        /// Gets or sets the target control to set the dependency property.
        /// If null, it will be set on the control hosting this behavior.
        /// </summary>
        public AvaloniaObject Target { get { return (AvaloniaObject)GetValue(TargetProperty); } set { SetValue(TargetProperty, value); } }

        /// <inheritdoc/>
        protected override void OnEvent()
        {
            var target = Target ?? AssociatedObject;
            target.SetCurrentValue(Property, Value);
        }
    }
}
