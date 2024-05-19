// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Globalization;
using Stride.Core.Presentation.Quantum.ViewModels;
using Stride.Core.Presentation.ValueConverters;
using System.Collections.Generic;

namespace Stride.Core.Assets.Editor.View.ValueConverters
{
    public class DifferentValuesToString : ValueConverterBase2<DifferentValuesToString>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != NodeViewModel.DifferentValues ? value?.ToString() ?? string.Empty : null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}