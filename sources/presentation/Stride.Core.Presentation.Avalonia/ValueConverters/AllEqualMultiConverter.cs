// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls;

using Stride.Core.Annotations;

using System.Collections.Generic;

namespace Stride.Core.Presentation.ValueConverters
{
    public class AllEqualMultiConverter : OneWayMultiValueConverter<AllEqualMultiConverter>
    {
        [NotNull]
        public override object? Convert([NotNull] IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values.Count < 2)
                throw new InvalidOperationException("This multi converter must be invoked with at least two elements");

            var fallbackValue = parameter is bool && (bool)parameter;
            var first = values[0];
            var result = values.All(x => x == AvaloniaProperty.UnsetValue ? fallbackValue : Equals(x, first));
            return result;
        }
    }
}
