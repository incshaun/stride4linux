// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Stride.Core.Presentation.Commands;
using Stride.Core.Presentation.Controls;
using Stride.Core.Presentation.View;
using Stride.Core.Presentation.ViewModels;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Controls.Templates;
using System.Windows.Input;

using System;

namespace Stride.Core.Presentation.Windows
{
    /// <summary>
    /// Base class for message-based dialog windows.
    /// </summary>
    public abstract class MessageDialogBase : ModalWindow
    {
        protected override Type StyleKeyOverride { get { return typeof(MessageDialogBase); } }

        /// <summary>
        /// Identifies the <see cref="ButtonsSource"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<IEnumerable<DialogButtonInfo>> ButtonsSourceProperty = StyledProperty<IEnumerable<DialogButtonInfo>>.Register<MessageDialogBase, IEnumerable<DialogButtonInfo>>(nameof(ButtonsSource)); // T1

        /// <summary>
        /// Identifies the <see cref="MessageTemplate"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<DataTemplate> MessageTemplateProperty = StyledProperty<DataTemplate>.Register<MessageDialogBase, DataTemplate>(nameof(MessageTemplate)); // T1

        /// <summary>
        /// Identifies the <see cref="MessageTemplateSelector"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<IDataTemplate> MessageTemplateSelectorProperty = StyledProperty<IDataTemplate>.Register<MessageDialogBase, IDataTemplate>(nameof(MessageTemplateSelector)); // T1

        /// <summary>
        /// Identifies the <see cref="ButtonCommand"/> dependency property key.
        /// </summary>
        private static readonly DirectProperty<MessageDialogBase, ICommandBase> ButtonCommandPropertyKey = AvaloniaProperty.RegisterDirect<MessageDialogBase, ICommandBase>(nameof (ButtonCommand), o => o.ButtonCommand); // T10H1
        /// <summary>
        /// Identifies the <see cref="ButtonCommand"/> dependency property.
        /// </summary>
        //protected static readonly AvaloniaProperty ButtonCommandProperty = ButtonCommandPropertyKey.DependencyProperty;
        
        protected MessageDialogBase()
        {
            var serviceProvider = new ViewModelServiceProvider(new[] { new DispatcherService(Dispatcher.UIThread) });
            ButtonCommand = new AnonymousCommand<int>(serviceProvider, ButtonClick);
        }

        public IEnumerable<DialogButtonInfo> ButtonsSource { get { return (IEnumerable<DialogButtonInfo>)GetValue(ButtonsSourceProperty); } set { SetValue(ButtonsSourceProperty, value); } }

        public DataTemplate MessageTemplate { get { return (DataTemplate)GetValue(MessageTemplateProperty); } set { SetValue(MessageTemplateProperty, value); } }

        public IDataTemplate MessageTemplateSelector { get { return (IDataTemplate)GetValue(MessageTemplateSelectorProperty); } set { SetValue(MessageTemplateSelectorProperty, value); }} 

        public int ButtonResult { get; private set; }

//         protected ICommandBase ButtonCommand { get { return (ICommandBase)GetValue(ButtonCommandProperty); } private set { SetValue(ButtonCommandPropertyKey, value); } }
        private ICommandBase _ButtonCommand;
        public ICommandBase ButtonCommand { get { return _ButtonCommand; } set { SetAndRaise(ButtonCommandPropertyKey, ref _ButtonCommand, value); } }

        private void ButtonClick(int parameter)
        {
            ButtonResult = parameter;
            Close();
        }
    }

}
