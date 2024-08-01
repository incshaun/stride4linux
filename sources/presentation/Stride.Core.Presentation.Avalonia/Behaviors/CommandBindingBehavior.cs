// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Avalonia;
using Avalonia.Controls;

using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
using Stride.Core.Annotations;
using Stride.Core.Presentation.Commands;

using Avalonia.Interactivity;
using System.Windows.Input;

namespace Stride.Core.Presentation.Behaviors
{
    /// <summary>
    /// This command will bind a <see cref="ICommandBase"/> to a <see cref="RoutedCommand"/>. It works just as a <see cref="CommandBinding"/> except that the bound
    /// command is executed when the routed command is executed. The <b>CanExecute</b> handler also invoke the <b>CanExecute</b> method of the <see cref="ICommandBase"/>.
    /// </summary>
    public class CommandBindingBehavior : Behavior<Control>
    {
		static CommandBindingBehavior()
		{
			CommandProperty.Changed.AddClassHandler<CommandBindingBehavior>(CommandChanged);
		}

//         private CommandBinding commandBinding;

        /// <summary>
        /// Identifies the <see cref="Command"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<ICommandBase> CommandProperty = StyledProperty<ICommandBase>.Register<CommandBindingBehavior, ICommandBase>(nameof(Command), null); // T5
        /// <summary>
        /// Identifies the <see cref="ContinueRouting"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> ContinueRoutingProperty = StyledProperty<bool>.Register<CommandBindingBehavior, bool>(nameof(ContinueRouting), true); // T2
        /// <summary>
        /// Identifies the <see cref="IsEnabled"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> IsEnabledProperty = StyledProperty<bool>.Register<CommandBindingBehavior, bool>(nameof(IsEnabled), true); // T2
        /// <summary>
        /// Identifies the <see cref="RoutedCommand"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<ICommand> RoutedCommandProperty = StyledProperty<ICommand>.Register<CommandBindingBehavior, ICommand>(nameof(RoutedCommand)); // T1

        /// <summary>
        /// Gets or sets the <see cref="ICommandBase"/> to bind.
        /// </summary>
        public ICommandBase Command { get { return (ICommandBase)GetValue(CommandProperty); } set { SetValue(CommandProperty, value); } }

        /// <summary>
        /// Gets or sets whether the input routed event that invoked the command should continue to route through the element tree.
        /// </summary>
        /// <seealso cref="RoutedEventArgs.ContinueRouting"/>
        public bool ContinueRouting { get { return (bool)GetValue(ContinueRoutingProperty); } set { SetValue(ContinueRoutingProperty, value); } }

        /// <summary>
        /// Gets or sets whether this command binding is enabled. When disabled, the <see cref="Command"/> won't be executed.
        /// </summary>
        public bool IsEnabled { get { return (bool)GetValue(IsEnabledProperty); } set { SetValue(IsEnabledProperty, value); } }

        /// <summary>
        /// Gets or sets the <see cref="ICommand"/> to bind.
        /// </summary>
        public ICommand RoutedCommand { get { return (ICommand)GetValue(RoutedCommandProperty); } set { SetValue(RoutedCommandProperty, value); } }

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            ApplicationCommands.AddCommandBinding (RoutedCommand, (e) => OnExecuted(e), (e) => OnCanExecute(e));
//             commandBinding = new CommandBinding(RountedCommand, (s, e) => OnExecuted(e), (s, e) => OnCanExecute(e));
//             AssociatedObject.CommandBindings.Add(commandBinding);
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
//             AssociatedObject.CommandBindings.Remove(commandBinding);
        }

        private static void CommandChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
        }

        private void OnCanExecute([NotNull] object? canExecuteRoutedEventArgs)
        {
            if (Command != null)
            {
//                 canExecuteRoutedEventArgs.CanExecute = IsEnabled && Command.Command.CanExecute(canExecuteRoutedEventArgs.Parameter);
            }
            else
            {
//                 canExecuteRoutedEventArgs.CanExecute = false;
            }

//             if (canExecuteRoutedEventArgs.CanExecute || !ContinueRouting)
//             {
//                 canExecuteRoutedEventArgs.Handled = true;
//             }
//             else
//             {
//                 canExecuteRoutedEventArgs.ContinueRouting = true;
//             }
        }

        private void OnExecuted([NotNull] object? executedRoutedEventArgs)
        {
            if (Command != null && IsEnabled)
            {
                Command.Execute(executedRoutedEventArgs);
                if (executedRoutedEventArgs != null)
                {
                   ((RoutedEventArgs) executedRoutedEventArgs).Handled = true;
                }
            }
        }
    }
}
