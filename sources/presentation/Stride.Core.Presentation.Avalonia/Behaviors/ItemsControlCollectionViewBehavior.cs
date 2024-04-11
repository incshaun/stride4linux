// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using Avalonia;
using Avalonia.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Xaml.Interactivity;
using Stride.Core.Annotations;
using Stride.Core.Presentation.Core;
using System.Collections.Generic;

using CollectionView = Avalonia.Collections.DataGridCollectionView;
using PropertyGroupDescription = Avalonia.Collections.DataGridPathGroupDescription;

namespace Stride.Core.Presentation.Behaviors
{
    public class ItemsControlCollectionViewBehavior : Behavior<ItemsControl>
    {
		static ItemsControlCollectionViewBehavior()
		{
			GroupingPropertyNameProperty.Changed.AddClassHandler<ItemsControlCollectionViewBehavior>(GroupingPropertyNameChanged);
			FilterPredicateProperty.Changed.AddClassHandler<ItemsControlCollectionViewBehavior>(FilterPredicateChanged);
		}

        private readonly DependencyPropertyWatcher propertyWatcher = new DependencyPropertyWatcher();

        public static readonly StyledProperty<string> GroupingPropertyNameProperty = StyledProperty<string>.Register<ItemsControlCollectionViewBehavior, string>("GroupingPropertyName", null); // T5

        public static readonly StyledProperty<Predicate<object>> FilterPredicateProperty = StyledProperty<Predicate<object>>.Register<ItemsControlCollectionViewBehavior, Predicate<object>>("FilterPredicate", null); // T5

        public string GroupingPropertyName { get { return (string)GetValue(GroupingPropertyNameProperty); } set { SetValue(GroupingPropertyNameProperty, value); } }
  
        public Predicate<object> FilterPredicate { get { return (Predicate<object>)GetValue(FilterPredicateProperty); } set { SetValue(FilterPredicateProperty, value); } }

        public IValueConverter GroupingPropertyConverter { get; set; }

        protected override void OnAttached()
        {
            base.OnAttached();
            propertyWatcher.Attach(AssociatedObject);
            propertyWatcher.RegisterValueChangedHandler(ItemsControl.ItemsSourceProperty, ItemsSourceChanged);
            UpdateCollectionView();
        }

        protected override void OnDetaching()
        {
            propertyWatcher.Detach();
            base.OnDetaching();
        }

        private void UpdateCollectionView()
        {
            if (AssociatedObject?.ItemsSource != null)
            {
//                 var collectionView = (CollectionView)CollectionViewSource.GetDefaultView(AssociatedObject.ItemsSource);
                var collectionView = new CollectionView(AssociatedObject.ItemsSource);
                if (collectionView == null) throw new InvalidOperationException("CollectionViewSource.GetDefaultView returned null for the items source of the associated object.");
                using (collectionView.DeferRefresh())
                {
                    bool removeGrouping = string.IsNullOrWhiteSpace(GroupingPropertyName);
                    if (collectionView.CanGroup)
                    {
                        if (collectionView.GroupDescriptions == null) throw new InvalidOperationException("CollectionView does not have a group description collection.");
                        collectionView.GroupDescriptions.Clear();

                        if (!removeGrouping)
                        {
//                             var groupDescription = new PropertyGroupDescription(GroupingPropertyName, GroupingPropertyConverter);
                            var groupDescription = new PropertyGroupDescription(GroupingPropertyName);
                            collectionView.GroupDescriptions.Add(groupDescription);
                        }
                    }
                    if (collectionView.CanFilter && (FilterPredicate != null))
                    {
                        collectionView.Filter = new Func<object, bool>(FilterPredicate);
                    }
                }
            }
        }

        private void ItemsSourceChanged(object sender, EventArgs e)
        {
            UpdateCollectionView();
        }

        private static void GroupingPropertyNameChanged([NotNull] AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var behavior = (ItemsControlCollectionViewBehavior)d;
            behavior.UpdateCollectionView();
        }

        private static void FilterPredicateChanged([NotNull] AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var behavior = (ItemsControlCollectionViewBehavior)d;
            behavior.UpdateCollectionView();
        }
    }
}

