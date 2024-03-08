// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;

using Stride.Core.Presentation.Extensions;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Interactivity;
using Avalonia.Data;
using Avalonia.Controls.Templates;

namespace Stride.Core.Assets.Editor.View.Controls
{
    public class EditableContentListBox : ListBox
    {
        private ScrollViewer scrollViewer;

        public static readonly StyledProperty<bool> CanEditProperty = StyledProperty<bool>.Register<EditableContentListBox, bool>("CanEdit", true); // T2

        public static readonly StyledProperty<DataTemplate> EditItemTemplateProperty = StyledProperty<DataTemplate>.Register<EditableContentListBox, DataTemplate>("EditItemTemplate"); // T1

        public static readonly StyledProperty<IDataTemplate> EditItemTemplateSelectorProperty = StyledProperty<IDataTemplate>.Register<EditableContentListBox, IDataTemplate>("EditItemTemplateSelector"); // T1

        public static ICommandSource BeginEditCommand { get; private set; }

        static EditableContentListBox()
        {
            //
//             BeginEditCommand = new ICommandSource("BeginEditCommand", typeof(EditableContentListBox), new InputGestureCollection(new[] { new KeyGesture(Key.F2) }));
//             CommandManager.RegisterClassCommandBinding(typeof(EditableContentListBox), new CommandBinding(BeginEditCommand, OnBeginEditCommand));
        }

        public EditableContentListBox()
        {
            Loaded += OnEditableListBoxLoaded;
        }

        public bool CanEdit { get { return (bool)GetValue(CanEditProperty); } set { SetValue(CanEditProperty, value); } }

        public DataTemplate EditItemTemplate { get { return (DataTemplate)GetValue(EditItemTemplateProperty); } set { SetValue(EditItemTemplateProperty, value); } }

        public IDataTemplate EditItemTemplateSelector { get { return (IDataTemplate)GetValue(EditItemTemplateSelectorProperty); } set { SetValue(EditItemTemplateSelectorProperty, value); } }


        public void BeginEdit()
        {
            if (!CanEdit)
                return;

            var selectedItem = SelectedItems.Cast<object>().LastOrDefault();

            if (selectedItem == null)
                return;

            ScrollIntoView(selectedItem);

//             var selectedContainer = (EditableContentListBoxItem)ItemContainerGenerator.ContainerFromItem(selectedItem);
            var selectedContainer = (EditableContentListBoxItem)ItemContainerGenerator.ContainerFromIndex(SelectedIndex);
            if (selectedContainer != null && selectedContainer.CanEdit)
            {
                selectedContainer.IsEditing = true;
            }
        }

        private void OnEditableListBoxLoaded(object sender, RoutedEventArgs e)
        {
            scrollViewer = Avalonia.VisualTree.VisualExtensions.FindDescendantOfType<ScrollViewer>(this);

            if (scrollViewer != null)
                scrollViewer.ScrollChanged += ScrollViewerScrollChanged;
        }

        protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
		{
			return NeedsContainer<EditableContentListBoxItem>(item, out recycleKey);
		}

        protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
        {
//             return new EditableContentListBoxItem(ItemTemplate, ItemTemplateSelector, EditItemTemplate, EditItemTemplateSelector);
            return new EditableContentListBoxItem(null, ItemTemplate, EditItemTemplate, EditItemTemplateSelector); // FIXME: resolve datatemplate and idatatemplate.
        }

        private void ScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (Math.Abs(e.ExtentDelta.Y) < 1e-3 && Math.Abs(e.ExtentDelta.X) < 1e-3)
                return;

//             if (!ReferenceEquals(e.OriginalSource, scrollViewer))
//             {
//                 return;
//             }

            var selectedItem = SelectedItems
                .Cast<object>()
                .LastOrDefault();

            if (selectedItem == null)
                return;

//             var selectedContainer = (EditableContentListBoxItem)ItemContainerGenerator.ContainerFromItem(selectedItem);
            var selectedContainer = (EditableContentListBoxItem)ItemContainerGenerator.ContainerFromIndex(SelectedIndex);

            if (selectedContainer == null)
                return;

            selectedContainer.IsEditing = false;
        }

        private static void OnBeginEditCommand(object sender, RoutedEventArgs e)
        {
            var listBox = (EditableContentListBox)sender;
            listBox.BeginEdit();
        }
    }

    public class EditableContentListBoxItem : ListBoxItem
    {
//         private static readonly MethodInfo NotifyListItemClickedMethod;

        private readonly DataTemplate regularContentTemplate;
        private readonly IDataTemplate regularContentTemplateSelector;

        private readonly DataTemplate editContentTemplate;
        private readonly IDataTemplate editContentTemplateSelector;

        private bool mouseDown;

        public static readonly StyledProperty<bool> CanEditProperty = StyledProperty<bool>.Register<EditableContentListBoxItem, bool>("CanEdit", true); // T2

        public static readonly StyledProperty<bool> IsEditingProperty = StyledProperty<bool>.Register<EditableContentListBoxItem, bool>("IsEditing", false, defaultBindingMode : BindingMode.TwoWay); // T9E

        static EditableContentListBoxItem()
		{
			IsEditingProperty.Changed.AddClassHandler<EditableContentListBoxItem>(IsEditingPropertyChanged);

//             NotifyListItemClickedMethod = typeof(ListBox).GetMethod("NotifyListItemClicked", BindingFlags.Instance | BindingFlags.NonPublic);
//             if (NotifyListItemClickedMethod == null)
//                 throw new InvalidOperationException("Unable to reach the NotifyListItemClicked internal method from ListBox class.");
        }

        internal EditableContentListBoxItem(
            DataTemplate regularContentTemplate,
            IDataTemplate regularContentTemplateSelector,
            DataTemplate editContentTemplate,
            IDataTemplate editContentTemplateSelector)
        {
            this.regularContentTemplate = regularContentTemplate;
            this.regularContentTemplateSelector = regularContentTemplateSelector;
            this.editContentTemplate = editContentTemplate;
            this.editContentTemplateSelector = editContentTemplateSelector;
        }

        public bool IsEditing { get { return (bool)GetValue(IsEditingProperty); } set { SetValue(IsEditingProperty, value); } }

        public bool CanEdit { get { return (bool)GetValue(CanEditProperty); } set { SetValue(CanEditProperty, value); } }

        private static void IsEditingPropertyChanged(AvaloniaObject sender, AvaloniaPropertyChangedEventArgs e)
        {
            var container = (EditableContentListBoxItem)sender;

//             bool check;

            if (container.IsEditing)
//                 check = ApplyTemplate(container, container.editContentTemplate, container.editContentTemplateSelector);
                ApplyTemplate(container, container.editContentTemplate, container.editContentTemplateSelector);
            else
            {
//                 check = ApplyTemplate(container, container.regularContentTemplate, container.regularContentTemplateSelector);
                ApplyTemplate(container, container.regularContentTemplate, container.regularContentTemplateSelector);
                if ((bool)e.OldValue)
                {
                    container.Focus();
                }
            }
//             Console.WriteLine(check);
        }

        private static void ApplyTemplate(ContentControl container, DataTemplate dt, IDataTemplate dts)
        {
            container.ContentTemplate = dt;
//             container.ContentTemplateSelector = dts;
            container.ApplyTemplate();
        }

        protected virtual void OnPointerPressed(PointerPressedEventArgs e)
        {
            // Intentionally does nothing to prevent item selection to happen on MouseDown. It will be managed on MouseUp.
            // NOTE: This is ok since ListBoxItem.OnMouseLeftButtonDown only manages selection, and Control.OnMouseLeftButtonDown does nothing.
            // NOTE: We still have to keep track of the mouse down event for mouse up to prevent weird behavior
            mouseDown = true;
        }

        protected virtual void OnPointerReleased(PointerReleasedEventArgs e)
        {
            if (mouseDown)
            {
                var listBox = ItemsControl.ItemsControlFromItemContainer(this) as ListBox;
                if (listBox != null && Focus())
                {
                    // Hackish way to reproduce what ListBoxItem does on MouseDown by invoking an internal method.
//                     NotifyListItemClickedMethod.Invoke(listBox, new object[] { this, MouseButton.Left });
                }
            }
            mouseDown = false;
        }
    }
}
