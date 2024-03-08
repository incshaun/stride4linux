// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Avalonia;
using Avalonia.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Stride.Core.Presentation.Commands;

namespace Stride.Core.Presentation.Controls
{
    public class TagControl : ContentControl
    {
        /// <summary>
        /// Identifies the <see cref="CloseTagCommand"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<ICommandBase> CloseTagCommandProperty = StyledProperty<ICommandBase>.Register<TagControl, ICommandBase>("CloseTagCommand"); // T1

        public ICommandBase CloseTagCommand
        {
            get { return (ICommandBase)GetValue(CloseTagCommandProperty); }
            set { SetValue(CloseTagCommandProperty, value); }
        }

        static TagControl()
        {
            
        }
    }
}
