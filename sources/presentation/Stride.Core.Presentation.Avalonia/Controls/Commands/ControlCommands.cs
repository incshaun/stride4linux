// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Stride.Core.Annotations;
using Stride.Core.Extensions;
using Avalonia.Interactivity;
using System.Collections.Generic;
using Stride.Core.Presentation.Commands;
using Avalonia.Styling;

namespace Stride.Core.Presentation.Controls.Commands
{
    /// <summary>
    /// This class provides an instance of all commands in the namespace <see cref="Commands"/>.
    /// These instances can be used in XAML with the <see cref="System.Windows.Markup.StaticExtension"/> markup extension.
    /// </summary>
    public static class ControlCommands
    {
        /// <summary>
        /// Initialize the static properties of the <see cref="ControlCommands"/> class.
        /// </summary>
        static ControlCommands()
        {
            ClearSelectionCommand = new RoutedCommand<Selector>(OnClearSelectionCommand);
            SetAllVectorComponentsCommand = new RoutedCommand<VectorEditorBase>(OnSetAllVectorComponents);
            ResetValueCommand = new RoutedCommand<VectorEditorBase>(OnResetValue);
        }

        /// <summary>
        /// Clears the current selection of a text box.
        /// </summary>
        [NotNull]
        public static RoutedCommand ClearSelectionCommand { get; }

        /// <summary>
        /// Sets all the components of a <see cref="VectorEditorBase"/> to the value given as parameter.
        /// </summary>
        [NotNull]
        public static RoutedCommand SetAllVectorComponentsCommand { get; }

        /// <summary>
        /// Resets the current value of a vector editor to the value set in the <see cref="VectorEditorBase{T}.DefaultValue"/> property.
        /// </summary>
        [NotNull]
        public static RoutedCommand ResetValueCommand { get; }
        
        private static void OnClearSelectionCommand(Selector sender)
        {
            var selector = sender as Selector;
            if (selector != null)
            {
//                 selector.SelectedItem = null;
            }
        }

        private static void OnSetAllVectorComponents(VectorEditorBase sender)
        {
            var vectorEditor = sender as VectorEditorBase;
            if (vectorEditor != null)
            {
                try
                {
//                     var value = Convert.ToSingle(e.Parameter);
//                     vectorEditor.SetVectorFromValue(value);
                }
                catch (Exception ex)
                {
                    ex.Ignore();
                }
            }
        }

        private static void OnResetValue(VectorEditorBase sender)
        {
            var vectorEditor = sender as VectorEditorBase;
            vectorEditor?.ResetValue();
        }
    }
}