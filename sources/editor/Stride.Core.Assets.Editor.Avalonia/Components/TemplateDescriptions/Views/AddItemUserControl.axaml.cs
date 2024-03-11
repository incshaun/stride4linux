// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System.Linq;
using Avalonia;
using Avalonia.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;
using Stride.Core.Assets.Editor.Components.TemplateDescriptions.ViewModels;
using Stride.Core.Presentation.Extensions;
using Avalonia.Interactivity;
using System.Linq;
using System.Windows.Input;

namespace Stride.Core.Assets.Editor.Components.TemplateDescriptions.Views
{
    /// <summary>
    /// Interaction logic for AddAssetUserControl.xaml
    /// </summary>
    public partial class AddItemUserControl : UserControl
    {
        public const double TemplateListWidth = 500.0;

        public static readonly StyledProperty<AddItemTemplateCollectionViewModel> TemplateCollectionProperty = StyledProperty<AddItemTemplateCollectionViewModel>.Register<AddItemUserControl, AddItemTemplateCollectionViewModel>(nameof(TemplateCollection)); // T1

        public static readonly StyledProperty<ICommand> AddItemCommandProperty = StyledProperty<ICommand>.Register<AddItemUserControl, ICommand>(nameof(AddItemCommand)); // T1

        public static readonly StyledProperty<ICommand> SelectFilesToCreateItemCommandProperty = StyledProperty<ICommand>.Register<AddItemUserControl, ICommand>(nameof(SelectFilesToCreateItemCommand)); // T1

        public AddItemUserControl()
        {
            InitializeComponent();
            Loaded += ControlLoaded;
        }

        public AddItemTemplateCollectionViewModel TemplateCollection { get { return (AddItemTemplateCollectionViewModel)GetValue(TemplateCollectionProperty); } set { SetValue(TemplateCollectionProperty, value); } }

        public ICommand AddItemCommand { get { return (ICommand)GetValue(AddItemCommandProperty); } set { SetValue(AddItemCommandProperty, value); } }

        public ICommand SelectFilesToCreateItemCommand { get { return (ICommand)GetValue(SelectFilesToCreateItemCommandProperty); } set { SetValue(SelectFilesToCreateItemCommandProperty, value); } }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            FilteringComboBox.SelectedIndex = -1;
            FilteringComboBox.Text = "";
            var groupList = global::Avalonia.VisualTree.VisualExtensions.GetVisualChildren(this).Where (x => x is ListBox).Select (x => (ListBox) x).FirstOrDefault(x => x.Name == "GroupList");
            if (groupList != null)
                groupList.SelectedIndex = -1;
        }
    }
}
