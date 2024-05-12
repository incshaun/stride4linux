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
    public class FileOpenModalDialog : ModalDialogBase, IFileOpenModalDialog
    {
        internal FileOpenModalDialog([NotNull] IDispatcherService dispatcher)
            : base(dispatcher)
        {
            Filters = new List<FileDialogFilter>();
            FilePaths = new List<string>();
        }

        /// <inheritdoc/>
        public bool AllowMultiSelection { get; set; }

        /// <inheritdoc/>
        public IList<FileDialogFilter> Filters { get; set; }

        /// <inheritdoc/>
        public IReadOnlyList<string> FilePaths { get; private set; }

        /// <inheritdoc/>
        public string InitialDirectory { get; set; }

        /// <inheritdoc/>
        public string DefaultFileName { get; set; }

        /// <inheritdoc/>
        public override async Task<DialogResult> ShowModal()
        {
            List<FilePickerFileType> OFilters = new List<FilePickerFileType> ();
            foreach (var filter in Filters.Where(x => !string.IsNullOrEmpty(x.ExtensionList)))
            {
                OFilters.Add(new FilePickerFileType (filter.Description) { Patterns = new List<string> (filter.ExtensionList.Split(';').Select (x => "*." + x))});
            }
            Window window = null;
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
            {
                window = lifetime.MainWindow;
            }
            IStorageProvider sp = TopLevel.GetTopLevel(window).StorageProvider; 
            IReadOnlyList<IStorageFile> results = await sp.OpenFilePickerAsync(new FilePickerOpenOptions()
                {
                    Title = "Open file",
                    FileTypeFilter = OFilters,
//                     SuggestedFileName = DefaultFileName,
                    SuggestedStartLocation = await sp.TryGetFolderFromPathAsync(new Uri (InitialDirectory)),
                    AllowMultiple = AllowMultiSelection
                });            
            if (results?.Count <= 0)
            {
                Result = DialogResult.Cancel;
            }
            else
            {
                Result = DialogResult.Ok;
            }

            FilePaths = Result != DialogResult.Cancel ? new List<string>(results.Select (e => e.Path.LocalPath)) : new List<string>();
            return Result;
        }
    }
}
