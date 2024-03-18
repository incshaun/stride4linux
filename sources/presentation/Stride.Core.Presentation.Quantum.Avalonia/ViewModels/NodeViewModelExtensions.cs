// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Stride.Core.Annotations;
using Stride.Core.Extensions;
using Stride.Core.Reflection;
using Stride.Core.Presentation.Collections;
using Stride.Core.Presentation.Commands;
using Stride.Core.Presentation.Core;
using Stride.Core.Presentation.Quantum.Presenters;
using Stride.Core.Presentation.ViewModels;
using Stride.Core.Quantum;
using Stride.Core.TypeConverters;
using Avalonia.Data.Core.Plugins;
using Avalonia.Data;
using Expression = System.Linq.Expressions.Expression;

namespace Stride.Core.Presentation.Quantum.ViewModels
{
    public class NodeViewModelExtensions
    {
        [ModuleInitializer]
        public static void Initialize ()
        {
            NodeViewModel.InitializerExtension = FinishInitialization;
        }
        
        public class AssociatedDataPropertyAccessorPlugin : IPropertyAccessorPlugin
        {
            private NodeViewModel datasource;
            public AssociatedDataPropertyAccessorPlugin (NodeViewModel source)
            {
                datasource = source;
            }
            public bool Match(object obj, string propertyName)
            {
                var name = NodeViewModel.EscapeName(propertyName);
                var value = datasource.GetChild(name) ?? datasource.GetCommand(name) ?? datasource.GetAssociatedData(name) ?? null;
                return value != null;
            }

            public IPropertyAccessor? Start(WeakReference<object?> reference, string propertyName)
            {
                return new AssociatedDataAccessor(reference, propertyName, datasource);
            }
            private class AssociatedDataAccessor : PropertyAccessorBase
            {
                public override Type? PropertyType => Value.GetType ();

                public override object? Value
                {
                    get
                    {
                        var name = NodeViewModel.EscapeName(propertyName);
                        var value = datasource.GetChild(name) ?? datasource.GetCommand(name) ?? datasource.GetAssociatedData(name) ?? null;
                        return value;
                    }
                }

                private readonly WeakReference<object?> _reference;
                private readonly string propertyName;
                private NodeViewModel datasource;

                public AssociatedDataAccessor(WeakReference<object?> reference, string property, NodeViewModel source)
                {
                    _reference = reference;
                    propertyName = property;
                    datasource = source;
                }

                public override bool SetValue(object? value, BindingPriority priority)
                {
                    throw new NotImplementedException();
                }

                protected override void SubscribeCore()
                {
                    PublishValue(Value);
                }

                protected override void UnsubscribeCore()
                {
                }
            }
        }
        
        public static void FinishInitialization(NodeViewModel nvm)
        {
            BindingPlugins.PropertyAccessors.Add(new AssociatedDataPropertyAccessorPlugin(nvm));
        }
    }
}