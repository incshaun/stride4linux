// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using Avalonia.Markup.Xaml;
using Stride.Core.Annotations;
using System.Collections.Generic;

namespace Stride.Core.Presentation.MarkupExtensions
{
    
    public class IntExtension : MarkupExtension
    {
        public int Value { get; set; }

        public IntExtension(object value)
        {
            Value = Convert.ToInt32(value);
        }

        [NotNull]
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Value;
        }
    }
}
