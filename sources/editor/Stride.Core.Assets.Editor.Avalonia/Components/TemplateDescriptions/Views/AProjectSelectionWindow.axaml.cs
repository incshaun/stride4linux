using System;
using System.ComponentModel;
using Stride.Core.Assets.Editor.Components.TemplateDescriptions.ViewModels;
using Stride.Core.Assets.Editor.Settings;
using Stride.Core.Assets.Editor.ViewModel;
using Stride.Core.IO;
using Stride.Core.Presentation.View;
using Stride.Core.Presentation.Windows;
using Stride.Core.Translation;
using MessageBoxButton = Stride.Core.Presentation.Services.MessageBoxButton;
using MessageBoxImage = Stride.Core.Presentation.Services.MessageBoxImage;

using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Stride.Core.Presentation.Controls;
using Avalonia;
using System.Linq;
using System.Collections.ObjectModel;

namespace Stride.Core.Assets.Editor.Components.TemplateDescriptions.Views
{
    public partial class AProjectSelectionWindow : ModalWindow
    {
        public AProjectSelectionWindow(NewOrOpenSessionTemplateCollectionViewModel ctx)
        {
            InitializeComponent(ctx);


            //            Width = Math.Min(Width, SystemParameters.WorkArea.Width);
            //            Height = Math.Min(Height, SystemParameters.WorkArea.Height);
            Title = string.Format(Tr._p("Title", "Project selection - {0}"), EditorPath.EditorTitle);
        }

        private void InitializeComponent(NewOrOpenSessionTemplateCollectionViewModel ctx, bool loadXaml = true, bool attachDevTools = true)
        {
            AvaloniaXamlLoader.Load(this);
            //DataContext = ctx;

#if DEBUG
            if (attachDevTools)
            {
//                this.AttachDevTools();
            }
#endif
        }

        public NewSessionParameters NewSessionParameters { get; private set; }

        public UFile ExistingSessionPath { get; private set; }

        public NewOrOpenSessionTemplateCollectionViewModel Templates { get { return (NewOrOpenSessionTemplateCollectionViewModel)DataContext; } set { DataContext = value; } }

        protected override void OnClosing(WindowClosingEventArgs e)
        {
            if (Result == Presentation.Services.DialogResult.Ok)
            {
                if (!ValidateProperties())
                {
                    e.Cancel = true;
                    return;
                }
            }

            if (Result == Presentation.Services.DialogResult.Ok)
            {
                var recentProject = Templates.SelectedTemplate as ExistingProjectViewModel;
                var newPackageTemplate = Templates.SelectedTemplate as TemplateDescriptionViewModel;
                if (recentProject != null)
                {
                    ExistingSessionPath = recentProject.Path;
                }
                else if (newPackageTemplate != null)
                {
                    NewSessionParameters = new NewSessionParameters
                    {
                        TemplateDescription = Templates.SelectedTemplate != null ? newPackageTemplate.GetTemplate() : null,
                        OutputName = Templates.Name,
                        OutputDirectory = Templates.Location,
                        SolutionName = Templates.SolutionName,
                        SolutionLocation = Templates.SolutionLocation,
                    };
                }
            }
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (Result == Presentation.Services.DialogResult.Ok)
            {
                EditorSettings.Save();
                InternalSettings.TemplatesWindowDialogLastNewSessionTemplateDirectory.SetValue(Templates.Location.FullPath);
                InternalSettings.Save();
            }
        }
        private void OnTextBoxValidated(object sender, EventArgs e)
        {
            ValidateProperties();
        }

        private bool ValidateProperties()
        {
            string error;
            if (!Templates.ValidateProperties(out error))
            {
//                 DialogHelper.BlockingMessageBox(DispatcherService.Create(), error, EditorPath.EditorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
//                 return false;
            }
            return true;
        }
    }
}

