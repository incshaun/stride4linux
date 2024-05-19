// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;

// using System.Windows.Interop;

using Stride.Core.Assets.Editor.ViewModel.Progress;
using Stride.Core.Presentation.Controls;
using Avalonia.Interactivity;
using Avalonia.Controls.Primitives;

namespace Stride.Core.Assets.Editor.View
{
    /// <summary>
    /// Interaction logic for WorkProgressWindow.xaml
    /// </summary>
    public partial class WorkProgressWindow : ModalWindow
    {
        private readonly WorkProgressViewModel workProgress;
        private bool callbackRegistered;

        public static IntPtr CurrentHandle;

        public WorkProgressWindow(WorkProgressViewModel workProgress)
        {
            InitializeComponent();
            this.workProgress = workProgress;
            Loaded += WindowLoaded;
            Closing += WindowClosing;
//             Width = Math.Min(Width, SystemParameters.WorkArea.Width);
//             Height = Math.Min(Height, SystemParameters.WorkArea.Height);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
		{
			base.OnApplyTemplate(e);
            if (workProgress.WorkDone && !workProgress.ShouldStayOpen())
            {
                if (!IsLoaded)
                {
                    // We don't close in this case since a loaded event is guaranteed to happen after OnApplyTemplate.
                    // This should prevent fatal exception from occurring (PDX-2968).
                    return;
                }
                Close();
                return;
            }

            if (!callbackRegistered)
            {
                workProgress.WorkFinished += WorkFinished;
                callbackRegistered = true;
            }
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            if (workProgress.WorkDone && !workProgress.ShouldStayOpen())
            {
                Close();
                return;
            }
            DataContext = workProgress;
//             CurrentHandle = new WindowInteropHelper(this).Handle;
            ShowInTaskbar = Owner == null;
        }


        private void WindowClosing(object sender, CancelEventArgs e)
        {
            // Prevent closing the window while the work is still in progress, unless it has been user cancelled.
            if (!workProgress.WorkDone && Result != Presentation.Services.DialogResult.Cancel)
            {
                e.Cancel = true;
            }
            else
            {
                CurrentHandle = IntPtr.Zero;
            }
        }

        private void WorkFinished(object sender, WorkProgressNotificationEventArgs e)
        {
            if (!workProgress.ShouldStayOpen())
            {
                Close();
            }
        }
    }
}