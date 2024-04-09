// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;


using Stride.Core.Presentation.Behaviors;
using Stride.Core.Presentation.Quantum.ViewModels;

namespace Stride.Core.Assets.Editor.View.Behaviors
{
    public class PropertyViewFilteringBehavior : ItemsControlCollectionViewBehavior
    {
		static PropertyViewFilteringBehavior()
		{
			FilterTokenProperty.Changed.AddClassHandler<PropertyViewFilteringBehavior>(FilterTokenChanged);
		}

        public static readonly StyledProperty<string> FilterTokenProperty = StyledProperty<string>.Register<PropertyViewFilteringBehavior, string>("FilterToken", null); // T5

        public string FilterToken { get { return (string)GetValue(FilterTokenProperty); } set { SetValue(FilterTokenProperty, value); } }

        private static void FilterTokenChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var behavior = (PropertyViewFilteringBehavior)d;
            var token = behavior.FilterToken;
            if (!string.IsNullOrWhiteSpace(token))
                behavior.FilterPredicate = x => Match((NodeViewModel)x, token);
            else
                behavior.FilterPredicate = null;
        }

        private static bool Match(NodeViewModel node, string token)
        {
            return node.DisplayName.IndexOf(token, StringComparison.CurrentCultureIgnoreCase) >= 0 || node.Children.Any(x => Match(x, token));
        }
    }
}
