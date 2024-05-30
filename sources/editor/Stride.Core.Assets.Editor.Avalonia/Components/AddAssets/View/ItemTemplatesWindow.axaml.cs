// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using Avalonia;
using Avalonia.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;
using Avalonia.Threading;
using Stride.Core.Assets.Editor.Components.TemplateDescriptions.ViewModels;
using Stride.Core.Assets.Editor.Services;
using Stride.Core.Presentation.Controls;
using Avalonia.Interactivity;

namespace Stride.Core.Assets.Editor.Components.AddAssets.View
{
    /// <summary>
    /// Interaction logic for AssetTemplatesWindow.xaml
    /// </summary>
    public partial class ItemTemplatesWindow : PopupModalWindow, IItemTemplateDialog
    {

        public ItemTemplatesWindow(AssetTemplatesViewModel viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));
            InitializeComponent();
            ViewModel = viewModel;
            Loaded += OnLoaded;
        }

        public AssetTemplatesViewModel ViewModel { get { return (AssetTemplatesViewModel)DataContext; } set { DataContext = value; } }

        public ITemplateDescriptionViewModel SelectedTemplate { get; private set; }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            if (IsMouseOverWindow(e))
            {
                // Defer the validation so the list box has time to update the selected template (we are fired before it because it is the preview event).
                Dispatcher.UIThread.InvokeAsync(Validate);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Enter)
                Validate();
        }

        private void Validate()
        {
            SelectedTemplate = ViewModel.SelectedTemplate.Key;
            Result = SelectedTemplate != null ? Presentation.Services.DialogResult.Ok : Presentation.Services.DialogResult.Cancel;
            Close();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Activate();
            if (TemplateListBox.Items.Count > 0)
            {
                TemplateListBox.SelectedIndex = 0;
                var item = TemplateListBox.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem;
                item?.Focus();
            }
        }
    }
}
