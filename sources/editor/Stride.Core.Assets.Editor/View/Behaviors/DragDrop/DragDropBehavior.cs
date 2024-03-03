// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Stride.Core.Assets.Editor.ViewModel;
using Stride.Core.Annotations;
using Stride.Core.Translation;

namespace Stride.Core.Assets.Editor.View.Behaviors
{
    public static class DragDropBehavior
    {
        /// <summary>
        /// Can't drop here
        /// </summary>
        public static readonly string InvalidDropAreaMessage = Tr._p("Message", "Can't drop here");
    }
    
    public partial class DragDropBehavior<TControl, TContainer>
    {
    }
}
