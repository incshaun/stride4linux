// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;


namespace Stride.Core.Presentation.ValueConverters
{
    /// <summary>
    /// This converter will convert a specific value to a <see cref="AvaloniaProperty.UnsetValue"/>.
    /// </summary>
    public class ValueToUnset : OneWayValueConverter<ValueToUnset>
    {
        /// <inheritdoc/>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Equals(value, parameter) ? AvaloniaProperty.UnsetValue : value;
        }
    }
}
