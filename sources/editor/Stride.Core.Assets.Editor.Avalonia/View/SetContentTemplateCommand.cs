// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using Avalonia;
using Avalonia.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Markup.Xaml.Templates;
using System.Windows.Input;

namespace Stride.Core.Assets.Editor.View
{
    public class SetContentTemplateCommand : ICommand
    {
        public ContentPresenter Target { get; set; }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Target.ContentTemplate = (DataTemplate)parameter;
        }

        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }
    }
}
