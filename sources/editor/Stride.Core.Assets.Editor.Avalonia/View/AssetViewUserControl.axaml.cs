// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System.Collections;
using System.Linq;
using Avalonia;
using Avalonia.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;
using Avalonia.Threading;
using Stride.Core.Assets.Editor.View.Controls;
using Stride.Core.Assets.Editor.ViewModel;
using Stride.Core.Presentation.Collections;
using Stride.Core.Presentation.Extensions;
using Stride.Core.Presentation.Commands;
using Stride.Core.Presentation.ViewModels;
using Stride.Core.Presentation.View;
using Stride.Core.Annotations;

using Avalonia.Interactivity;

using System.Windows.Input;

namespace Stride.Core.Assets.Editor.View
{
    /// <summary>
    /// Interaction logic for AssetViewUserControl.xaml
    /// </summary>
    public partial class AssetViewUserControl : UserControl
    {
        /// <summary>
        /// Identifies the <see cref="AssetCollection"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<AssetCollectionViewModel> AssetCollectionProperty = StyledProperty<AssetCollectionViewModel>.Register<AssetViewUserControl, AssetCollectionViewModel>(nameof(AssetCollection), null); // T5

        /// <summary>
        /// Identifies the <see cref="AssetContextMenu"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<Control> AssetContextMenuProperty = StyledProperty<Control>.Register<AssetViewUserControl, Control>(nameof(AssetContextMenu), null); // T5

        /// <summary>
        /// Identifies the <see cref="CanEditAssets"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> CanEditAssetsProperty = StyledProperty<bool>.Register<AssetViewUserControl, bool>(nameof(CanEditAssets), true); // T2

        /// <summary>
        /// Identifies the <see cref="CanAddAssets"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> CanAddAssetsProperty = StyledProperty<bool>.Register<AssetViewUserControl, bool>(nameof(CanAddAssets), true); // T2

        /// <summary>
        /// Identifies the <see cref="CanDeleteAssets"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> CanDeleteAssetsProperty = StyledProperty<bool>.Register<AssetViewUserControl, bool>(nameof(CanDeleteAssets), true); // T2

        /// <summary>
        /// Identifies the <see cref="CanRecursivelyDisplayAssets"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> CanRecursivelyDisplayAssetsProperty = StyledProperty<bool>.Register<AssetViewUserControl, bool>(nameof(CanRecursivelyDisplayAssets), true); // T2

        /// <summary>
        /// Identifies the <see cref="CanRecursivelyDisplayAssets"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> GiveFocusOnSelectionChangeProperty = StyledProperty<bool>.Register<AssetViewUserControl, bool>(nameof(GiveFocusOnSelectionChange), true); // T2

        /// <summary>
        /// Identifies the <see cref="ThumbnailZoomIncrement"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> ThumbnailZoomIncrementProperty = StyledProperty<double>.Register<AssetViewUserControl, double>(nameof(ThumbnailZoomIncrement), 16.0); // T2

        /// <summary>
        /// Identifies the <see cref="ThumbnailMinimumSize"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> ThumbnailMinimumSizeProperty = StyledProperty<double>.Register<AssetViewUserControl, double>(nameof(ThumbnailMinimumSize), 16.0); // T2

        /// <summary>
        /// Identifies the <see cref="ThumbnailMaximumSize"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> ThumbnailMaximumSizeProperty = StyledProperty<double>.Register<AssetViewUserControl, double>(nameof(ThumbnailMaximumSize), 128.0); // T2

        /// <summary>
        /// Identifies the <see cref="TileThumbnailSize"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> TileThumbnailSizeProperty = StyledProperty<double>.Register<AssetViewUserControl, double>(nameof(TileThumbnailSize), 96.0); // T2

        /// <summary>
        /// Identifies the <see cref="GridThumbnailSize"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> GridThumbnailSizeProperty = StyledProperty<double>.Register<AssetViewUserControl, double>(nameof(GridThumbnailSize), 16.0); // T2

        /// <summary>
        /// Identifies the <see cref="AssetDoubleClick"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<ICommand> AssetDoubleClickProperty = StyledProperty<ICommand>.Register<AssetViewUserControl, ICommand>(nameof(AssetDoubleClick)); // T1

        /// <summary>
        /// Gets the command that initiate the edition of the currently selected item.
        /// </summary>
        public ICommand BeginEditCommand { get; }

        /// <summary>
        /// Gets the command that will increase the size of thumbnails.
        /// </summary>
        public static ICommand ZoomInCommand { get; }

        /// <summary>
        /// Gets the command that will decrease the size of thumbnails.
        /// </summary>
        public static ICommand ZoomOutCommand { get; }

        static AssetViewUserControl()
		{
			AssetCollectionProperty.Changed.AddClassHandler<AssetViewUserControl>(AssetCollectionChanged);
			AssetContextMenuProperty.Changed.AddClassHandler<AssetViewUserControl>(OnAssetContextMenuChanged);

//             var serviceProvider = new ViewModelServiceProvider(new[] { new DispatcherService(Dispatcher.UIThread) });
//             BeginEditCommand = new AnonymousCommand<AssetViewUserControl>(serviceProvider, CanBeginEditCommand);
            
//             var serviceProvider = new ViewModelServiceProvider(new[] { new DispatcherService(Dispatcher.UIThread) });
//             BeginEditCommand = new AnonymousCommand<AssetViewUserControl>(serviceProvider, CanBeginEditCommand);
//             CommandManager.RegisterClassCommandBinding(typeof(AssetViewUserControl), new CommandBinding(BeginEditCommand, BeginEdit, CanBeginEditCommand));
//             CommandManager.RegisterClassInputBinding(typeof(AssetViewUserControl), new InputBinding(BeginEditCommand, new KeyGesture(Key.F2)));
// 
            ZoomInCommand = new RoutedCommand<AssetViewUserControl>(ZoomIn);

            //             ZoomInCommand = new RoutedCommand(nameof(ZoomInCommand), typeof(AssetViewUserControl));
//             var zoomInCommandBinding = new CommandBinding(ZoomInCommand, ZoomIn);
//             zoomInCommandBinding.PreviewCanExecute += (s, e) => e.CanExecute = true;
//             zoomInCommandBinding.PreviewExecuted += ZoomIn;
//             CommandManager.RegisterClassCommandBinding(typeof(AssetViewUserControl), zoomInCommandBinding);
// 
             ZoomOutCommand = new RoutedCommand<AssetViewUserControl>(ZoomOut);
//             ZoomOutCommand = new ICommand(nameof(ZoomOutCommand), typeof(AssetViewUserControl));
//             var zoomOutCommandBinding = new CommandBinding(ZoomOutCommand, ZoomOut);
//             zoomOutCommandBinding.PreviewCanExecute += (s, e) => e.CanExecute = true;
//             zoomOutCommandBinding.PreviewExecuted += ZoomOut;
//             CommandManager.RegisterClassCommandBinding(typeof(AssetViewUserControl), zoomOutCommandBinding);
        }

//         [ModuleInitializer]
//         public static void Initialize()
//         {
//             var instance = new AssetViewUserControl ();
//         }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AssetViewUserControl"/> class.
        /// </summary>
        public AssetViewUserControl()
        {
            BeginEditCommand = new RoutedCommand<AssetViewUserControl>((a) => CanBeginEditCommand(this));
            
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the <see cref="AssetCollectionViewModel"/> to display in this control.
        /// </summary>
        public AssetCollectionViewModel AssetCollection { get => (AssetCollectionViewModel)GetValue(AssetCollectionProperty); set => SetValue(AssetCollectionProperty, value); }

        /// <summary>
        /// Gets or sets the control to use as context menu for assets.
        /// </summary>
        public Control AssetContextMenu { get => (Control)GetValue(AssetContextMenuProperty); set => SetValue(AssetContextMenuProperty, value); }

        /// <summary>
        /// Gets the list of items to display in the primary tool bar. The primary tool bar won't be displayed if this list is empty.
        /// </summary>
        public IList PrimaryToolBarItems { get; } = new NonGenericObservableListWrapper<object>(new ObservableList<object>());

        /// <summary>
        /// Gets or sets whether it is possible to edit assets with this <see cref="AssetViewUserControl"/>.
        /// </summary>
        public bool CanEditAssets { get => (bool)GetValue(CanEditAssetsProperty); set => SetValue(CanEditAssetsProperty, value); }

        /// <summary>
        /// Gets or sets whether it is possible to add assets with this <see cref="AssetViewUserControl"/>.
        /// </summary>
        public bool CanAddAssets { get => (bool)GetValue(CanAddAssetsProperty); set => SetValue(CanAddAssetsProperty, value); }

        /// <summary>
        /// Gets or sets whether it is possible to delete assets with this <see cref="AssetViewUserControl"/>.
        /// </summary>
        public bool CanDeleteAssets { get => (bool)GetValue(CanDeleteAssetsProperty); set => SetValue(CanDeleteAssetsProperty, value); }

        /// <summary>
        /// Gets or sets whether it is possible to select to display asset recursively from selected locations.
        /// </summary>
        public bool CanRecursivelyDisplayAssets { get => (bool)GetValue(CanRecursivelyDisplayAssetsProperty); set => SetValue(CanRecursivelyDisplayAssetsProperty, value); }

        /// <summary>
        /// Gets or sets whether the control should get the focus when its selection changes. The focus is not given if the selection is cleared.
        /// </summary>
        public bool GiveFocusOnSelectionChange { get => (bool)GetValue(GiveFocusOnSelectionChangeProperty); set => SetValue(GiveFocusOnSelectionChangeProperty, value); }

        /// <summary>
        /// Gets or sets the zoom increment value.
        /// </summary>
        public double ThumbnailZoomIncrement { get => (double)GetValue(ThumbnailZoomIncrementProperty); set => SetValue(ThumbnailZoomIncrementProperty, value); }

        /// <summary>
        /// Gets or sets the minimum size of thumbnails.
        /// </summary>
        public double ThumbnailMinimumSize { get => (double)GetValue(ThumbnailMinimumSizeProperty); set => SetValue(ThumbnailMinimumSizeProperty, value); }

        /// <summary>
        /// Gets or sets the maximum size of thumbnails.
        /// </summary>
        public double ThumbnailMaximumSize { get => (double)GetValue(ThumbnailMaximumSizeProperty); set => SetValue(ThumbnailMaximumSizeProperty, value); }

        /// <summary>
        /// Gets or sets the size of thumbnails in tile view.
        /// </summary>
        public double TileThumbnailSize { get => (double)GetValue(TileThumbnailSizeProperty); set => SetValue(TileThumbnailSizeProperty, value); }

        /// <summary>
        /// Gets or sets the size of thumbnails in grid view.
        /// </summary>
        public double GridThumbnailSize { get => (double)GetValue(GridThumbnailSizeProperty); set => SetValue(GridThumbnailSizeProperty, value); }

        /// <summary>
        /// Gets or sets the command to execute when user double-clicks an asset.
        /// </summary>
        public ICommand AssetDoubleClick { get => (ICommand)GetValue(AssetDoubleClickProperty); set => SetValue(AssetDoubleClickProperty, value); }

        /// <summary>
        /// Begins edition of the currently selected content.
        /// </summary>
        public void BeginEdit()
        {
            if (!CanEditAssets || AssetCollection == null)
                return;

            var selectedAsset = AssetCollection.SelectedContent.LastOrDefault();
            if (selectedAsset == null)
                return;

            var listBox = global::Avalonia.VisualTree.VisualExtensions.FindDescendantOfType<EditableContentListBox>(AssetViewPresenter);
            listBox?.BeginEdit();

            var gridView = global::Avalonia.VisualTree.VisualExtensions.FindDescendantOfType<DataGrid>(AssetViewPresenter);
            gridView?.BeginEdit();
        }

        private static void ZoomIn(AssetViewUserControl d)
        {
            var listBox = global::Avalonia.VisualTree.VisualExtensions.FindDescendantOfType<EditableContentListBox>(d.AssetViewPresenter);
            if (listBox != null)
            {
                d.TileThumbnailSize += d.ThumbnailZoomIncrement;
                if (d.TileThumbnailSize >= d.ThumbnailMaximumSize)
                {
                    d.TileThumbnailSize = d.ThumbnailMaximumSize;
                }
            }

            var gridView = global::Avalonia.VisualTree.VisualExtensions.FindDescendantOfType<DataGrid>(d.AssetViewPresenter);
            if (gridView != null)
            {
                d.GridThumbnailSize += d.ThumbnailZoomIncrement;
                if (d.GridThumbnailSize >= d.ThumbnailMaximumSize)
                {
                    d.GridThumbnailSize = d.ThumbnailMaximumSize;
                }
            }
        }

        private static void ZoomOut(AssetViewUserControl d)
        {
            var listBox = global::Avalonia.VisualTree.VisualExtensions.FindDescendantOfType<EditableContentListBox>(d.AssetViewPresenter);
            if (listBox != null)
            {
                d.TileThumbnailSize -= d.ThumbnailZoomIncrement;
                if (d.TileThumbnailSize <= d.ThumbnailMinimumSize)
                {
                    d.TileThumbnailSize = d.ThumbnailMinimumSize;
                }
            }

            var gridView = global::Avalonia.VisualTree.VisualExtensions.FindDescendantOfType<DataGrid>(d.AssetViewPresenter);
            if (gridView != null)
            {
                d.GridThumbnailSize -= d.ThumbnailZoomIncrement;
                if (d.GridThumbnailSize <= d.ThumbnailMinimumSize)
                {
                    d.GridThumbnailSize = d.ThumbnailMinimumSize;
                }
            }
        }

        protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
        {
            base.OnPointerWheelChanged(e);
//             if (Keyboard.Modifiers == ModifierKeys.Control)
//             {
//                 if (e.Delta > 0)
//                 {
//                     ZoomIn();
//                     e.Handled = true;
//                 }
//                 if (e.Delta < 0)
//                 {
//                     ZoomOut();
//                     e.Handled = true;
//                 }
//             }
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            if (!IsFocused && !IsKeyboardFocusWithin)
                Focus();
        }

        private bool CanBeginEdit()
        {
            if (!CanEditAssets || AssetCollection == null)
                return false;

            // Special case to under edition state restoration in the DataGrid
            if (AssetCollection.SelectedContent.Count == 0)
                return true;

            if (AssetCollection.SelectedContent.Count != 1)
                return false;

            // HACK: might be a better way to check that
            var asset = AssetCollection.SelectedContent.Last() as AssetViewModel;
            return !asset?.IsLocked ?? true;
        }

        private static void CanBeginEditCommand([NotNull] AssetViewUserControl sender)
        {
            var control = (AssetViewUserControl)sender;
//             e.CanExecute = control.CanBeginEdit();
            if (control.CanBeginEdit())
            {
              control.BeginEdit ();
            }
        }

        private static void BeginEdit(object sender, RoutedEventArgs e)
        {
            var assetView = (AssetViewUserControl)sender;
            assetView.BeginEdit();
        }

        private static void ZoomIn(object sender, RoutedEventArgs e)
        {
            var assetView = (AssetViewUserControl)sender;
            AssetViewUserControl.ZoomIn(assetView);
        }

        private static void ZoomOut(object sender, RoutedEventArgs e)
        {
            var assetView = (AssetViewUserControl)sender;
            AssetViewUserControl.ZoomOut(assetView);
        }

        /// <summary>
        /// Raised when the <see cref="AssetCollection"/> dependency property changes.
        /// </summary>
        /// <param name="d">The event sender.</param>
        /// <param name="e">The event argument.</param>
        private static void AssetCollectionChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var assetViewControl = (AssetViewUserControl)d;
            assetViewControl.RootContainer.DataContext = e.NewValue;
        }

        /// <summary>
        /// Raised when the <see cref="AssetContextMenu"/> dependency property changes.
        /// </summary>
        /// <param name="d">The event sender.</param>
        /// <param name="e">The event argument.</param>
        private static void OnAssetContextMenuChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                NameScope.SetNameScope((StyledElement)e.NewValue, NameScope.GetNameScope((StyledElement)d));
            }
        }
    }
}
