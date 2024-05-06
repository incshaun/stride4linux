// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Avalonia.Platform.Storage.FileIO;

using System;
using Stride.Core.Annotations;
using Stride.Core.Presentation.Services;

using FileDialogFilter = Stride.Core.Presentation.Services.FileDialogFilter;

namespace Stride.Core.Presentation.Dialogs
{
    public class FileSaveModalDialog : ModalDialogBase, IFileSaveModalDialog
    {
        internal FileSaveModalDialog([NotNull] IDispatcherService dispatcher)
            : base(dispatcher)
        {
            Filters = new List<FileDialogFilter>();
        }

        /// <inheritdoc/>
        public IList<FileDialogFilter> Filters { get; set; }

        /// <inheritdoc/>
        public string FilePath { get; private set; }

        /// <inheritdoc/>
        public string InitialDirectory { get; set; }

        /// <inheritdoc/>
        public string DefaultFileName { get; set; }

        /// <inheritdoc/>
        public string DefaultExtension { get; set; }

        /// <inheritdoc/>
        public override async Task<DialogResult> ShowModal()
        {
            List<FilePickerFileType> OFilters = new List<FilePickerFileType> ();
            foreach (var filter in Filters.Where(x => !string.IsNullOrEmpty(x.ExtensionList)))
            {
                OFilters.Add(new FilePickerFileType (filter.Description) { Patterns = new List<string> (filter.ExtensionList.Split(';'))});
            }
//             SaveDlg.AlwaysAppendDefaultExtension = true;
            Window window = null;
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
            {
                window = lifetime.MainWindow;
            }
            IStorageProvider sp = TopLevel.GetTopLevel(window).StorageProvider; 
            var file = await sp.SaveFilePickerAsync(new FilePickerSaveOptions()
            {
                Title = "Save file",
                FileTypeChoices = OFilters,
                    SuggestedStartLocation = await sp.TryGetFolderFromPathAsync(new Uri (InitialDirectory)),
//                 SuggestedFileName = "FileName",
                ShowOverwritePrompt = true
            });
            if (file == null)
            {
                Result = DialogResult.Cancel;
                FilePath = null;
            }
            else
            {
                Result = DialogResult.Ok;
                FilePath = file.Path.LocalPath;
            }

            return Result;
            return DialogResult.None;
        }
    }
}
