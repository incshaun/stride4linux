// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using RoslynPad.Editor;
using RoslynPad.Roslyn.Diagnostics;
using Stride.Core.Assets.Editor.Services;
using Stride.Core.Assets.Editor.ViewModel;
using Stride.Core.Annotations;
using Stride.Assets.Presentation.ViewModel;
using Stride.Core.Assets.Editor.Annotations;
using Stride.Core.Presentation.Commands;
using Stride.Core.Presentation.ViewModels;
using System.IO;

namespace Stride.Assets.Presentation.AssetEditors.ScriptEditor
{
    /// <summary>
    /// View model for the script editor (using Roslyn & AvalonEdit and RoslynPad).
    /// </summary>
    [AssetEditorViewModel<ScriptSourceFileAssetViewModel>]
    public class ScriptEditorViewModel : AssetEditorViewModel
    {
        public ScriptEditorViewModel([NotNull] ScriptSourceFileAssetViewModel script)
            : base(script)
        {
            Code = StrideAssetsViewModel.Instance.Code;
            SourceTextContainer = script.TextContainer;
            
            SaveDocumentCommand = new AnonymousCommand(ServiceProvider, SaveDocument);
        }

        /// <summary>
        /// The asset being edited.
        /// </summary>
        public override ScriptSourceFileAssetViewModel Asset => (ScriptSourceFileAssetViewModel)base.Asset;

        /// <summary>
        /// The code view model that manages roslyn states.
        /// </summary>
        public CodeViewModel Code { get; }

        /// <summary>
        /// The source text container used for editing.
        /// </summary>
        public AvalonEditTextContainer SourceTextContainer { get; }

        /// <summary>
        /// The roslyn host.
        /// </summary>
        public RoslynHost RoslynHost { get; private set; }

        /// <summary>
        /// The roslyn workspace.
        /// </summary>
        public RoslynWorkspace Workspace { get; private set; }

        /// <summary>
        /// The roslyn document id.
        /// </summary>
        public DocumentId DocumentId { get; private set; }

        public event EventHandler DocumentOpened;
        public event EventHandler DocumentClosed;
        public event EventHandler<DiagnosticsUpdatedArgs> ProcessDiagnostics;

        public ICommandBase SaveDocumentCommand { get; }
        
        /// <inheritdoc/>
        public sealed override async Task<bool> Initialize()
        {
            var projectWatcher = await StrideAssetsViewModel.Instance.Code.ProjectWatcher;
            RoslynHost = await projectWatcher.RoslynHost;
            Workspace = await StrideAssetsViewModel.Instance.Code.Workspace;

            Workspace.HostDocumentClosed += WorkspaceHostDocumentClosed;

            if (Asset.DocumentId == null)
                return false;

            // Open document
            DocumentId = Workspace.OpenDocument(SourceTextContainer, await Asset.DocumentId,
                a => Dispatcher.Invoke(() => ProcessDiagnostics?.Invoke(this, a)));
            //Workspace.TrackDocument(await Asset.DocumentId, OnTextUpdated);

            // Failed? let's close editor right away
            if (DocumentId == null)
                return false;

            DocumentOpened?.Invoke(this, EventArgs.Empty);

            return true;
        }

        private void WorkspaceHostDocumentClosed(DocumentId documentId)
        {
            if (documentId == DocumentId)
            {
                Dispatcher.Invoke(() =>
                {
                    DocumentClosed?.Invoke(this, EventArgs.Empty);

                    // Document is closing, we assume we have been asked before to save
                    ServiceProvider.Get<IAssetEditorsManager>().CloseAssetEditorWindow(Asset, false);
                });
            }
        }

        /// <inheritdoc/>
        public override void Destroy()
        {
            if (DocumentId != null)
                Workspace.CloseDocument(DocumentId);

            Workspace.HostDocumentClosed -= WorkspaceHostDocumentClosed;

            base.Destroy();
        }
        
        private void SaveDocument ()
        {
            var fn = Asset.AssetItem.FullPath;
            Console.WriteLine ("Saving document " + fn);
            using (var stream = new FileStream(fn, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                SourceTextContainer.Editor.Save (stream);
            }
        }
    }
}
