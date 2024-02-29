// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.Templates;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Markup.Xaml;

using Avalonia.Controls.Templates;

namespace Stride.Core.Assets.Editor.View.TemplateProviders
{
//     [ContentProperty("TemplateDefinitions")]
    public class DataTypeTemplateSelector : IDataTemplate
    {
        public TemplateDefinitionCollection TemplateDefinitions { get; } = new TemplateDefinitionCollection();

       public Control? Build(object? param)
        {
           var templates = TemplateDefinitions;
           var template = templates.FirstOrDefault(t => t.DataType.IsInstanceOfType(param));
           return template.DataTemplate.Build(param);
        }

        public bool Match(object? data)
        {
            var templates = TemplateDefinitions;
            var template = templates.FirstOrDefault(t => t.DataType.IsInstanceOfType(data));
            return template != null;
        }        
//         public override DataTemplate SelectTemplate(object item, AvaloniaObject container)
//         {
//             var uiElement = container as UIElement;
//             if (uiElement == null)
//             {
//                 return base.SelectTemplate(item, container);
//             }
// 
//             var templates = TemplateDefinitions;
//             if (templates == null || templates.Count == 0)
//             {
//                 return base.SelectTemplate(item, container);
//             }
// 
//             var template = templates.FirstOrDefault(t => t.DataType.IsInstanceOfType(item));
//             return template?.DataTemplate ?? base.SelectTemplate(item, container);
//         }
    }

    public class TemplateDefinitionCollection : Collection<TemplateDefinition> { }

    public class TemplateDefinition : AvaloniaObject
    {
        public static readonly StyledProperty<Type> DataTypeProperty = StyledProperty<Type>.Register<TemplateDefinition, Type>("DataType");
        
        public static readonly StyledProperty<DataTemplate> DataTemplateProperty = StyledProperty<DataTemplate>.Register<TemplateDefinition, DataTemplate>("DataTemplate");

        public Type DataType
        {
            get { return (Type)GetValue(DataTypeProperty); }
            set { SetValue(DataTypeProperty, value); }
        }

        public DataTemplate DataTemplate
        {
            get { return (DataTemplate)GetValue(DataTemplateProperty); }
            set { SetValue(DataTemplateProperty, value); }
        }
    }
}
