// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
//using System.Windows;
using Avalonia.Controls;
using Stride.Core.Assets.Editor.ViewModel;
using Stride.Core.Assets;
using Stride.Core.Assets.Editor.Components.Properties;
using Stride.Core.Assets.Editor.Services;
using Stride.Core.Diagnostics;
using Stride.Core.Extensions;
using Stride.Core.Reflection;
using Stride.Core;
using Stride.Core.Annotations;
using Stride.Assets.Presentation.AssetEditors.AssetHighlighters;
using Stride.Assets.Presentation.AssetEditors.EntityHierarchyEditor.EntityFactories;
using Stride.Assets.Presentation.AssetEditors.Gizmos;
using Stride.Assets.Presentation.NodePresenters.Commands;
using Stride.Assets.Presentation.NodePresenters.Updaters;
using Stride.Assets.Presentation.SceneEditor.Services;
using Stride.Assets.Presentation.ViewModel;
using Stride.Assets.Presentation.ViewModel.CopyPasteProcessors;
using Stride.Editor;
using Stride.Engine;
using Stride.Core.Assets.Templates;
using Stride.Core.Packages;
using Stride.Editor.Annotations;
using Stride.Editor.Preview.View;

namespace Stride.Assets.Presentation
{
    public sealed partial class StrideDefaultAssetsPlugin
    {
//placeholder
public static IReadOnlyList<(Type type, int order)> ComponentOrders { get; private set; } = new List<(Type, int)>();
public static IReadOnlyDictionary<Type, Type> AssetHighlighterTypesDictionary => AssetHighlighterTypes;
private static readonly Dictionary<Type, Type> AssetHighlighterTypes = new Dictionary<Type, Type>();   
private static readonly Dictionary<Type, Type> GizmoTypes = new Dictionary<Type, Type>();
public static IReadOnlyDictionary<Type, Type> GizmoTypeDictionary => GizmoTypes;
    }
}
