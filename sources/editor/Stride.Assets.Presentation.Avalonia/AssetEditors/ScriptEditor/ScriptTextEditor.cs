// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;

using Avalonia.Input;
using Microsoft.CodeAnalysis;
using RoslynPad.Editor;
using Stride.Core.Presentation.Controls;
using Stride.Core.Presentation.Core;
using Stride.Core.Presentation.Extensions;
using TextDocument = AvaloniaEdit.Document.TextDocument;
using Avalonia.Interactivity;
using Avalonia.Controls.Primitives;
using System;
using Avalonia.Controls.Metadata;

namespace Stride.Assets.Presentation.AssetEditors.ScriptEditor
{
    [TemplatePart(Name = "PART_CodeEditor", Type = typeof(SimpleCodeTextEditor))]
    public class ScriptTextEditor : TextBoxBase
    {
        // Template
        private SimpleCodeTextEditor codeEditor;

        // Code editor elements
        private AvalonEditTextContainer sourceTextContainer;

        // Local copies at creation time
        private RoslynWorkspace workspace;

        /// <summary>
        /// Identifies the <see cref="Workspace"/> dependency property.
        /// </summary>
        private static readonly DirectProperty<ScriptTextEditor, RoslynWorkspace> WorkspacePropertyKey = AvaloniaProperty.RegisterDirect<ScriptTextEditor, RoslynWorkspace>(nameof (Workspace), o => o.Workspace); // T10H

        /// <summary>
        /// Identifies the <see cref="ProjectId"/> dependency property.
        /// </summary>
        private static readonly DirectProperty<ScriptTextEditor, ProjectId> ProjectIdPropertyKey = AvaloniaProperty.RegisterDirect<ScriptTextEditor, ProjectId>(nameof (ProjectId), o => o.ProjectId); // T10H

        /// <summary>
        /// The workspace this script will belong to.
        /// </summary>
        private RoslynWorkspace _Workspace;
		public RoslynWorkspace Workspace { get { return _Workspace; } set { SetAndRaise(WorkspacePropertyKey, ref _Workspace, value); } }

        /// <summary>
        ///  The project in which the script should be created.
        /// </summary>
        private ProjectId _ProjectId;
		public ProjectId ProjectId { get { return _ProjectId; } set { SetAndRaise(ProjectIdPropertyKey, ref _ProjectId, value); } }

        /// <summary>
        /// The generated <see cref="Microsoft.CodeAnalysis.DocumentId"/> inside the <see cref="Workspace"/>.
        /// </summary>
        public DocumentId DocumentId { get; private set; }

        static ScriptTextEditor()
		{
			WorkspacePropertyKey.Changed.AddClassHandler<ScriptTextEditor>(OnRecreateScript);
			ProjectIdPropertyKey.Changed.AddClassHandler<ScriptTextEditor>(OnRecreateScript);

//             ValidateWithEnterProperty.OverrideMetadata(typeof(ScriptTextEditor), new FrameworkPropertyMetadata(false));
        }

        public ScriptTextEditor()
        {
            // Something in the TextBox is eating our Space events (probably IME)
            // Details: https://social.msdn.microsoft.com/Forums/vstudio/en-US/446ec083-04c8-43f2-89dc-1e2521a31f6b/textboxs-previewtextinput-event-does-not-trigger-on-spaces-input?forum=wpf
            // and http://stackoverflow.com/a/1463391
            AddHandler(KeyDownEvent, new EventHandler<KeyEventArgs>(HandleHandledKeyDown), handledEventsToo: true);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
		{
			base.OnApplyTemplate(e);

            codeEditor = DependencyObjectExtensions.CheckTemplatePart<SimpleCodeTextEditor>(e.NameScope.Find<SimpleCodeTextEditor>("PART_CodeEditor"));

            // Setup again the code editor with new parameters
            SetupCodeEditor();
        }

        protected override void OnValidating(CancelRoutedEventArgs e)
        {
            base.OnValidating(e);

            // Copy text back
            SetCurrentValue(TextProperty, sourceTextContainer.CurrentText.ToString());
        }

        private static void OnRecreateScript(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var scriptTextEditor = (ScriptTextEditor)d;

            // Setup again the code editor with new parameters
            scriptTextEditor.SetupCodeEditor();
        }

        private void HandleHandledKeyDown(object sender, RoutedEventArgs e)
        {
            // See remarks in .ctor
            if (((KeyEventArgs)e).Key == Key.Space && ((KeyEventArgs)e).KeyModifiers == KeyModifiers.None)
            {
                e.Handled = false;
            }
        }

        private void SetupCodeEditor()
        {
            // Already created?
            if (workspace != null)
            {
                // Anything changed?
                if (workspace == Workspace/* && projectId == ProjectId*/)
                {
                    return;
                }

                CleanupCodeEditor();
            }

            // Check we have everything we need
            if (Workspace == null || codeEditor == null || ProjectId == null)
                return;

            this.workspace = Workspace;

            // Start with initial text
            var textDocument = new TextDocument(Text);
            sourceTextContainer = new AvalonEditTextContainer(textDocument);
            sourceTextContainer.TextChanged += SourceTextContainer_TextChanged;

            var documentId = workspace.AddDocument(ProjectId, $"script-{Guid.NewGuid()}.cs", SourceCodeKind.Script, TextLoader.From(sourceTextContainer, VersionStamp.Create()));
            DocumentId = documentId;
//             workspace.OpenDocument(sourceTextContainer, documentId, a => Dispatcher.Invoke(() => codeEditor.ProcessDiagnostics(a)));

            // Bind SourceTextContainer to UI
            codeEditor.BindSourceTextContainer(workspace, sourceTextContainer, documentId);
        }

        private void CleanupCodeEditor()
        {
            workspace.CloseDocument(DocumentId);

            codeEditor.Unbind();

            sourceTextContainer.TextChanged -= SourceTextContainer_TextChanged;
            sourceTextContainer = null;
            
            // Remove working document
            workspace.RemoveDocument(DocumentId);
            DocumentId = null;
        }

        private void SourceTextContainer_TextChanged(object sender, Microsoft.CodeAnalysis.Text.TextChangeEventArgs e)
        {
            HasChangesToValidate = true;
        }
    }
}
