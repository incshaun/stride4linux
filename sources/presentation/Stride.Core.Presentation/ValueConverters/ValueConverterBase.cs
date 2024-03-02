// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Globalization;
using Stride.Core.Annotations;

namespace Stride.Core.Presentation.ValueConverters
{
    public abstract partial class ValueConverterBase<T>
    {
        public abstract object Convert(object value, [NotNull] Type targetType, object parameter, CultureInfo culture);

        public abstract object ConvertBack(object value, [NotNull] Type targetType, object parameter, CultureInfo culture);
    }
}
