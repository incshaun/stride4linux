// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using Avalonia;
using Avalonia.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Interactivity;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;

namespace Stride.Editor.Preview.View
{
    [TemplatePart(Name = "PART_StrideView", Type = typeof(ContentPresenter))]
    public class StridePreviewView : TemplatedControl, IPreviewView
    {
        private IPreviewBuilder builder;

        private IAssetPreview previewer;

        static StridePreviewView()
        {
            // FIXME  T31
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
		{
			base.OnApplyTemplate(e);
            var strideViewPresenter = e.NameScope.Find<ContentPresenter>("PART_StrideView");
            if (strideViewPresenter != null && builder != null)
            {
                var strideView = builder.GetStrideView();
                Console.WriteLine ("App Temp " + strideView + " - " + strideViewPresenter.Content + " - " + ((Control) strideView).Parent );
//                strideViewPresenter.Content = null;
//                strideViewPresenter.UpdateLayout ();
                Console.WriteLine ("App Temp2 " + strideView + " - " + strideViewPresenter.Content + " - " + ((Control) strideView).Parent + " - " + (strideViewPresenter != ((Control) strideView).Parent) + " - " + strideViewPresenter);
//                ((Control) strideView).Parent = null;
//                if (((Control) strideView).Parent == null) // already made the content.
                {
                    strideViewPresenter.Content = strideView;
                }
                Console.WriteLine ("App Temp3 " + strideView + " - " + strideViewPresenter.Content + " - " + ((Control) strideView).Parent );
            }

 //           UpdateStrideView();
        }

        public void InitializeView(IPreviewBuilder previewBuilder, IAssetPreview assetPreview)
        {
            previewer = assetPreview;
            builder = previewBuilder;
            var viewModel = previewer.PreviewViewModel;
            if (viewModel != null)
            {
                viewModel.AttachPreview(previewer);
                DataContext = viewModel;
            }
            UpdateStrideView();

            Loaded += OnLoaded;
        }

        public void UpdateView(IAssetPreview assetPreview)
        {
            var viewModel = previewer.PreviewViewModel;
            if (viewModel != null)
            {
                viewModel.AttachPreview(previewer);
                DataContext = viewModel;
            }
            UpdateStrideView();
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            previewer?.OnViewAttached();
        }

        private void UpdateStrideView()
        {
 /*           try
            {
            var a = this;
            var b = this.FindNameScope();
            if (this.FindNameScope() != null)
            {
                var strideViewPresenter = this.FindNameScope().Find<ContentPresenter>("PART_StrideView");
                if (strideViewPresenter != null && builder != null)
                {
                    var strideView = builder.GetStrideView();
                    strideViewPresenter.Content = strideView;
                }
            }
            }
            catch (Exception ex)
            {
                Console.WriteLine ("UpdateStrideView ex: " + ex);
            }*/
        }
    }
}
