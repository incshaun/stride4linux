// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using Avalonia;
using Avalonia.Controls;

using Avalonia.Markup.Xaml;
using Stride.Core.Annotations;

namespace Stride.Core.Presentation.MarkupExtensions
{
    
    public class SizeExtension : MarkupExtension
    {
        public SizeExtension(double uniformLength)
        {
            Value = new Size(uniformLength, uniformLength);
        }

        public SizeExtension(double width, double height)
        {
            Value = new Size(width, height);
        }

        public SizeExtension(Size value)
        {
            Value = value;
        }

        public Size Value { get; set; }

        [NotNull]
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Value;
        }
    }
}
