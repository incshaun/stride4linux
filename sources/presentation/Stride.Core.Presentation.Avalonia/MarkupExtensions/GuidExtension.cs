// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using Avalonia.Markup.Xaml;
using Stride.Core.Annotations;

namespace Stride.Core.Presentation.MarkupExtensions
{
    
    public sealed class GuidExtension : MarkupExtension
    {
        public Guid Value { get; set; }

        public GuidExtension()
        {
            Value = Guid.Empty;
        }

        public GuidExtension(object value)
        {
            Guid guid;
            Guid.TryParse(value as string, out guid);
            Value = guid;
        }

        /// <inheritdoc/>
        [NotNull]
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Value;
        }
    }
}
