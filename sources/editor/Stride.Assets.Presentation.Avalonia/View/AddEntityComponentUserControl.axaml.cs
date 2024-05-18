// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Controls.Documents;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
//using System.Windows.Navigation;
using Avalonia.Controls.Shapes;
using Stride.Core.Assets.Editor.Quantum.NodePresenters.Commands;
using Stride.Core.Presentation.Controls;
using Stride.Core.Presentation.Extensions;
using System.Linq;
using System.Windows.Input;

namespace Stride.Assets.Presentation.View
{
    /// <summary>
    /// Interaction logic for AddEntityComponentUserControl.xaml
    /// </summary>
    public partial class AddEntityComponentUserControl : UserControl
    {
        public const double ComponentListWidth = 250.0;


        public IEnumerable<AbstractNodeType> AvailableComponentTypes
        {
            get { return (IEnumerable<AbstractNodeType>)GetValue(AvailableComponentTypesProperty); }
            set { SetValue(AvailableComponentTypesProperty, value); }
        }

        // Using a AvaloniaProperty as the backing store for ComponentTypes.  This enables animation, styling, binding, etc...
        public static readonly StyledProperty<IEnumerable<AbstractNodeType>> AvailableComponentTypesProperty = StyledProperty<IEnumerable<AbstractNodeType>>.Register<AddEntityComponentUserControl, IEnumerable<AbstractNodeType>>("AvailableComponentTypes", null); // T2

        public IEnumerable<AbstractNodeTypeGroup> AvailableComponentTypeGroups
        {
            get { return (IEnumerable<AbstractNodeTypeGroup>)GetValue(AvailableComponentTypeGroupsProperty); }
            set { SetValue(AvailableComponentTypeGroupsProperty, value); }
        }

        // Using a AvaloniaProperty as the backing store for AvailableComponentTypeGroups.  This enables animation, styling, binding, etc...
        public static readonly StyledProperty<IEnumerable<AbstractNodeTypeGroup>> AvailableComponentTypeGroupsProperty = StyledProperty<IEnumerable<AbstractNodeTypeGroup>>.Register<AddEntityComponentUserControl, IEnumerable<AbstractNodeTypeGroup>>("AvailableComponentTypeGroups", null); // T2

        public AbstractNodeTypeGroup SelectedGroup
        {
            get { return (AbstractNodeTypeGroup)GetValue(SelectedGroupProperty); }
            set { SetValue(SelectedGroupProperty, value); }
        }

        // Using a AvaloniaProperty as the backing store for SelectedGroup.  This enables animation, styling, binding, etc...
        public static readonly StyledProperty<AbstractNodeTypeGroup> SelectedGroupProperty = StyledProperty<AbstractNodeTypeGroup>.Register<AddEntityComponentUserControl, AbstractNodeTypeGroup>("SelectedGroup", null); // T5

        public string SearchToken
        {
            get { return (string)GetValue(SearchTokenProperty); }
            set { SetValue(SearchTokenProperty, value); }
        }

        // Using a AvaloniaProperty as the backing store for SearchToken.  This enables animation, styling, binding, etc...
        public static readonly StyledProperty<string> SearchTokenProperty =
            StyledProperty<string>.Register<AddEntityComponentUserControl, string>("SearchToken", null);        

        public IEnumerable<AbstractNodeType> ComponentTypes
        {
            get { return (IEnumerable<AbstractNodeType>)GetValue(ComponentTypesProperty); }
            set { SetValue(ComponentTypesProperty, value); }
        }

        public static readonly StyledProperty<IEnumerable<AbstractNodeType>> ComponentTypesProperty = StyledProperty<IEnumerable<AbstractNodeType>>.Register<AddEntityComponentUserControl, IEnumerable<AbstractNodeType>>("ComponentTypes", null); // T2

        public ICommand AddNewItemCommand
        {
            get { return (ICommand)GetValue(AddNewItemCommandProperty); }
            set { SetValue(AddNewItemCommandProperty, value); }
        }

        // Using a AvaloniaProperty as the backing store for AddNewItemCommand.  This enables animation, styling, binding, etc...
        public static readonly StyledProperty<ICommand> AddNewItemCommandProperty = StyledProperty<ICommand>.Register<AddEntityComponentUserControl, ICommand>("AddNewItemCommand", null); // T2

        public ICommand AddNewScriptComponentCommand
        {
            get { return (ICommand)GetValue(AddNewScriptComponentCommandProperty); }
            set { SetValue(AddNewScriptComponentCommandProperty, value); }
        }

        // Using a AvaloniaProperty as the backing store for AddNewScriptComponentCommand.  This enables animation, styling, binding, etc...
        public static readonly StyledProperty<ICommand> AddNewScriptComponentCommandProperty = StyledProperty<ICommand>.Register<AddEntityComponentUserControl, ICommand>("AddNewScriptComponentCommand", null); // T2

        private static void OnSelectedGroupChanged(AvaloniaObject d,AvaloniaPropertyChangedEventArgs e)
        {
            if (d is AddEntityComponentUserControl control)
            {
                d.SetValue(ComponentTypesProperty, e.NewValue is AbstractNodeTypeGroup group ? group.Types : null);
            }
        }

        private static void OnSearchTokenChanged(AvaloniaObject d,AvaloniaPropertyChangedEventArgs e)
        {
            if (d is AddEntityComponentUserControl control)
            {
                if(string.IsNullOrEmpty(e.NewValue as string))
                {
                    control.ComponentTypes = null;
                }
                else
                {
                    control.ComponentTypes = control.AvailableComponentTypes;
                }
            }
        }

        static AddEntityComponentUserControl()
		{
			SelectedGroupProperty.Changed.AddClassHandler<AddEntityComponentUserControl>(OnSelectedGroupChanged);
			SearchTokenProperty.Changed.AddClassHandler<AddEntityComponentUserControl>(OnSearchTokenChanged);        
		}

		public AddEntityComponentUserControl()
		{
            InitializeComponent();
            FilteringComboBox.DataContext = this;

            PART_Popup.Closed += Popup_Closed;
        }

        private void Popup_Closed(object sender, EventArgs e)
        {
            FilteringComboBox.SelectedIndex = -1;
            SearchToken = null;
            SelectedGroup = null;
            var groupList = global::Avalonia.VisualTree.VisualExtensions.GetVisualChildren(FilteringComboBox).Where (x => x is ListBox).Select (x => (ListBox) x).FirstOrDefault(x => x.Name == "GroupList");
            if (groupList != null)
                groupList.SelectedIndex = -1;
        }
    }
}
