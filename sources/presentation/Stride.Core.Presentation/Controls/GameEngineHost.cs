// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Stride.Core.Mathematics;

namespace Stride.Core.Presentation.Controls
{
    public interface GameEngineHostInterface : IDisposable
    {
        
    }
    
    /// <summary>
    /// A <see cref="FrameworkElement"/> that can host a game engine window. This control is faster than <see cref="HwndHost"/> but might behave
    /// a bit less nicely on certain cases (such as resize, etc.).
    /// </summary>
    public class GameEngineHostBase : IDisposable
    {
        public delegate GameEngineHostInterface Builder (IntPtr childHandle, object window);
        public static Builder builder;
        private GameEngineHostInterface impl = null;
        
        public GameEngineHostBase(IntPtr childHandle, object window)
        {
            if (builder != null)
            {
              impl = builder (childHandle, window);
            }
        }
        
        public GameEngineHostInterface GetHost ()
        {
            return impl;
        }

        public void Dispose()
        {
            impl?.Dispose ();
        }
    }
}
