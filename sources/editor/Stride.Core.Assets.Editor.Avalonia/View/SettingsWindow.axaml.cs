// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using Avalonia;
using Avalonia.Controls;

using Stride.Core.Assets.Editor.Components.Properties;
using Stride.Core.Assets.Editor.Settings;
using Stride.Core.Assets.Editor.Settings.ViewModels;
using Stride.Core.Presentation.Quantum;
using Stride.Core.Presentation.Quantum.ViewModels;
using Stride.Core.Presentation.ViewModels;
using Stride.Core.Presentation.Controls;

namespace Stride.Core.Assets.Editor.View
{
    public class SettingsTemplateProvider : NodeViewModelTemplateProvider
    {
        public override string Name => "SettingsEntry";

        public override bool MatchNode(NodeViewModel node)
        {
            return node?.NodeValue is PackageSettingsWrapper.SettingsKeyWrapper;
        }
    }

    public class SettingsStringFromAcceptableValuesTemplateProvider : NodeViewModelTemplateProvider
    {
        public override string Name => "StringFromAcceptableValues";

        public override bool MatchNode(NodeViewModel node)
        {
            object hasAcceptableValues;
            return node.Parent != null && (node.Parent.AssociatedData.TryGetValue("HasAcceptableValues", out hasAcceptableValues) && (bool)hasAcceptableValues);
        }
    }

    public class SettingsCommandTemplateProvider : NodeViewModelTemplateProvider
    {
        public override string Name => "SettingsCommand";

        public override bool MatchNode(NodeViewModel node)
        {
            return node?.NodeValue is SettingsCommand;
        }
    }

    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : ModalWindow
    {
        public SettingsWindow(IViewModelServiceProvider serviceProvider)
        {
            DataContext = new SettingsViewModel(serviceProvider, EditorSettings.SettingsContainer.CurrentProfile);
            InitializeComponent();
//             Width = Math.Min(Width, SystemParameters.WorkArea.Width);
//             Height = Math.Min(Height, SystemParameters.WorkArea.Height);
        }
    }
}
