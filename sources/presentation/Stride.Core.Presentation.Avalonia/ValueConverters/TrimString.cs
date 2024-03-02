// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Stride.Core.Annotations;
using Stride.Core.Presentation.Controls;
using System.Collections.Generic;
using Avalonia.Media;

namespace Stride.Core.Presentation.ValueConverters
{
    public class TrimString : OneWayMultiValueConverter<TrimString>
    {
        public double MaxWidth { get; set; }

        public TextTrimming TextTrimming { get; set; }

        public TrimmingSource TrimmingSource { get; set; }

        public string WordSeparators { get; set; }

        /// <inheritdoc />
        public override object? Convert([NotNull] IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (values.Count != 2)
                throw new ArgumentException("The number of provided bindings to this converter must be 2.");

            var text = values[0]?.ToString() ?? string.Empty;
//             if (values[1] is Control control)
//                 return Trimming.ProcessTrimming(control, text, TextTrimming, TrimmingSource, WordSeparators, MaxWidth);
//             if (values[1] is TextBlock textBlock)
//                 return Trimming.ProcessTrimming(textBlock, text, TextTrimming, TrimmingSource, WordSeparators, MaxWidth);
            return text;
        }

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // Note: necessary because properties can be different for each instance of this converter.
            return this;
        }
    }
}
