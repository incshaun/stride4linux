// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Globalization;


namespace Stride.Core.Presentation.ValueConverters
{
    /// <summary>
    /// This converter compares the given string with the string passed as parameter, and returns <c>true</c> if they are equal, <c>false</c> otherwise.
    /// </summary>
    public class StringEquals : OneWayValueConverter<StringEquals>
    {
        /// <inheritdoc/>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string svalue = value as string;
            if (svalue == null)
            {
                svalue = value.ToString();
            }
 
            var result = string.Equals(svalue, (string)parameter);
            return result;
        }
    }
}
