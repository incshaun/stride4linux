// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System.Collections;
using Avalonia;
using Avalonia.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Data;
using Avalonia.Controls.Templates;
using Avalonia.Styling;
using Avalonia.Interactivity;

namespace Stride.Core.Assets.Editor.Components.TemplateDescriptions.Views
{
    /// <summary>
    /// Interaction logic for ObjectBrowserUserControl.xaml
    /// </summary>
    public partial class ObjectBrowserUserControl : UserControl
    {
        public static readonly StyledProperty<IEnumerable> HierarchyItemsSourceProperty = StyledProperty<IEnumerable>.Register<ObjectBrowserUserControl, IEnumerable>("HierarchyItemsSource"); // T1

        public static readonly StyledProperty<object> SelectedHierarchyItemProperty = StyledProperty<object>.Register<ObjectBrowserUserControl, object>("SelectedHierarchyItem"); // T6E

        public static readonly StyledProperty<IDataTemplate> HierarchyItemTemplateProperty = StyledProperty<IDataTemplate>.Register<ObjectBrowserUserControl, IDataTemplate>("HierarchyItemTemplate"); // T1

        public static readonly StyledProperty<ControlTheme> HierarchyItemContainerStyleProperty = StyledProperty<ControlTheme>.Register<ObjectBrowserUserControl, ControlTheme>("HierarchyItemContainerStyle"); // T1

        public static readonly StyledProperty<IEnumerable> ObjectItemsSourceProperty = StyledProperty<IEnumerable>.Register<ObjectBrowserUserControl, IEnumerable>("ObjectItemsSource"); // T1

        public static readonly StyledProperty<object> SelectedObjectItemProperty = StyledProperty<object>.Register<ObjectBrowserUserControl, object>("SelectedObjectItem", defaultBindingMode: BindingMode.TwoWay); // T6E

        public static readonly StyledProperty<DataTemplate> ObjectItemTemplateProperty = StyledProperty<DataTemplate>.Register<ObjectBrowserUserControl, DataTemplate>("ObjectItemTemplate"); // T1

        public static readonly StyledProperty<IDataTemplate> ObjectItemTemplateSelectorProperty = StyledProperty<IDataTemplate>.Register<ObjectBrowserUserControl, IDataTemplate>("ObjectItemTemplateSelector"); // T1

        public static readonly StyledProperty<ControlTheme> ObjectItemContainerStyleProperty = StyledProperty<ControlTheme>.Register<ObjectBrowserUserControl, ControlTheme>("ObjectItemContainerStyle"); // T1

        public static readonly StyledProperty<DataTemplate> ObjectDescriptionTemplateProperty = StyledProperty<DataTemplate>.Register<ObjectBrowserUserControl, DataTemplate>("ObjectDescriptionTemplate"); // T1

        public static readonly StyledProperty<IDataTemplate> ObjectDescriptionTemplateSelectorProperty = StyledProperty<IDataTemplate>.Register<ObjectBrowserUserControl, IDataTemplate>("ObjectDescriptionTemplateSelector"); // T1

        public ObjectBrowserUserControl()
        {
            InitializeComponent();
        }

        public IEnumerable HierarchyItemsSource { get { return (IEnumerable)GetValue(HierarchyItemsSourceProperty); } set { SetValue(HierarchyItemsSourceProperty, value); } }

        public object SelectedHierarchyItem { get { return GetValue(SelectedHierarchyItemProperty); } set { SetValue(SelectedHierarchyItemProperty, value); } }

        public IDataTemplate HierarchyItemTemplate { get { return (IDataTemplate)GetValue(HierarchyItemTemplateProperty); } set { SetValue(HierarchyItemTemplateProperty, value); } }

        public ControlTheme HierarchyItemContainerStyle { get { return (ControlTheme)GetValue(HierarchyItemContainerStyleProperty); } set { SetValue(HierarchyItemContainerStyleProperty, value); } }

        public IEnumerable ObjectItemsSource { get { return (IEnumerable)GetValue(ObjectItemsSourceProperty); } set { SetValue(ObjectItemsSourceProperty, value); } }

        public object SelectedObjectItem { get { return GetValue(SelectedObjectItemProperty); } set { SetValue(SelectedObjectItemProperty, value); } }

        public DataTemplate ObjectItemTemplate { get { return (DataTemplate)GetValue(ObjectItemTemplateProperty); } set { SetValue(ObjectItemTemplateProperty, value); } }

        public IDataTemplate ObjectItemTemplateSelector { get { return (IDataTemplate)GetValue(ObjectItemTemplateSelectorProperty); } set { SetValue(ObjectItemTemplateSelectorProperty, value); } }

        public ControlTheme ObjectItemContainerStyle { get { return (ControlTheme)GetValue(ObjectItemContainerStyleProperty); } set { SetValue(ObjectItemContainerStyleProperty, value); } }

        public DataTemplate ObjectDescriptionTemplate { get { return (DataTemplate)GetValue(ObjectDescriptionTemplateProperty); } set { SetValue(ObjectDescriptionTemplateProperty, value); } }
        
        public IDataTemplate ObjectDescriptionTemplateSelector { get { return (IDataTemplate)GetValue(ObjectDescriptionTemplateSelectorProperty); } set { SetValue(ObjectDescriptionTemplateSelectorProperty, value); } }

        private void SelectedObjectUpdated(object sender, RoutedEventArgs e)
        {
            DescriptionScrollViewer.ScrollToHome();
        }
    }
}
