// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using Avalonia;
using Avalonia.Controls;

using Avalonia.Markup.Xaml;
using Stride.Core.Annotations;

namespace Stride.Core.Presentation.MarkupExtensions
{
    
    public class ThicknessExtension : MarkupExtension
    {
        public ThicknessExtension(double uniformLength)
        {
            Value = new Thickness(uniformLength);
        }

        public ThicknessExtension(double horizontal, double vertical)
        {
            Value = new Thickness(horizontal, vertical, horizontal, vertical);
        }

        public ThicknessExtension(double left, double top, double right, double bottom)
        {
            Value = new Thickness(left, top, right, bottom);
        }

        public ThicknessExtension(Thickness value)
        {
            Value = value;
        }

        public Thickness Value { get; set; }

        [NotNull]
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Value;
        }
    }
}
