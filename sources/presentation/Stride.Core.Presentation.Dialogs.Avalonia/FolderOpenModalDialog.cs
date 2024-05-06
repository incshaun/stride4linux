// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Threading.Tasks;
// using Microsoft.WindowsAPICodePack.Dialogs;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Avalonia.Platform.Storage.FileIO;

using System;

using Stride.Core.Annotations;
using Stride.Core.Presentation.Services;

namespace Stride.Core.Presentation.Dialogs
{
    public class FolderOpenModalDialog : ModalDialogBase, IFolderOpenModalDialog
    {
        internal FolderOpenModalDialog([NotNull] IDispatcherService dispatcher)
            : base(dispatcher)
        {
        }

        /// <inheritdoc/>
        public string Directory { get; private set; }

        /// <inheritdoc/>
        public string InitialDirectory { get; set; }

        public override async Task<DialogResult> ShowModal()
        {
            Window window = null;
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
            {
                window = lifetime.MainWindow;
            }
            IStorageProvider sp = TopLevel.GetTopLevel(window).StorageProvider; 
            var folders = await sp.OpenFolderPickerAsync(new FolderPickerOpenOptions()
            {
                Title = "Folder",
                SuggestedStartLocation = await sp.TryGetFolderFromPathAsync(new Uri (InitialDirectory)),
//                 SuggestedFileName = "FileName",
                AllowMultiple = false
            });
            
            if (folders.Count <= 0)
            {
                Result = DialogResult.Cancel;
                Directory = null;
            }
            else
            {
                Result = DialogResult.Ok;
                Directory = folders[0].Path.LocalPath;
            }
            
            return Result;
        }
    }
}
