// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Documents;
using Stride.Core.Presentation.Extensions;

namespace Stride.Core.Assets.Editor.Components.Status.Views
{
    public class ToolTipHelper : Control
    {
        public static readonly StyledProperty<StatusViewModel> StatusProperty =
            StyledProperty<StatusViewModel>.Register<ToolTipHelper,StatusViewModel>("Status", null);

        private static readonly StyledProperty<int> StatusTokenProperty =
            StyledProperty<int>.Register<ToolTipHelper,int>("StatusToken", -1);
        
//         public static readonly AttachedProperty<StatusViewModel> StatusProperty = AvaloniaProperty<StatusViewModel>.RegisterAttached<ToolTipHelper, Control, StatusViewModel>("Status", null, OnStatusChanged);
// 
//         private static readonly DependencyPropertyKey StatusTokenPropertyKey =
//             DependencyProperty.RegisterAttachedReadOnly("StatusToken", typeof(int), typeof(ToolTipHelper), new PropertyMetadata(-1));
// 
//         private static readonly DependencyProperty StatusTokenProperty = StatusTokenPropertyKey.DependencyProperty;
        
        public static StatusViewModel GetStatus(Control element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));

            return (StatusViewModel)element.GetValue(StatusProperty);
        }

        public static void SetStatus(Control element, StatusViewModel value)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));

            element.SetValue(StatusProperty, value);
        }

        private static int GetStatusToken(Control element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));

            return (int)element.GetValue(StatusTokenProperty);
        }

        private static void SetStatusToken(Control element, int value)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));

            element.SetValue(StatusTokenProperty, value);
        }

        private static void OnStatusChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var element = d as Control;
            if (element == null)
                return;

//             ToolTipService.RemoveToolTipClosingHandler(element, ToolTipClosed);
//             ToolTipService.RemoveToolTipOpeningHandler(element, ToolTipOpened);
// 
//             ToolTipService.AddToolTipOpeningHandler(element, ToolTipOpened);
//             ToolTipService.AddToolTipClosingHandler(element, ToolTipClosed);
        }

        private static void ToolTipOpened(object sender, EventArgs e)
        {
            var element = sender as Control;
            if (element == null)
                return;

//             var status = GetStatus(element);
//             var text = GetTooltipText(element.ToolTip);
//             if (status == null || string.IsNullOrEmpty(text))
//                 return;
//             var token = GetStatusToken(element);
//             if (token >= 0)
//                 status.PopStatus(token);
// 
//             token = status.PushStatus(text);
//             SetStatusToken(element, token);
        }

        private static void ToolTipClosed(object sender, EventArgs e)
        {
            var element = sender as Control;
            if (element == null)
                return;

            var status = GetStatus(element);
            var token = GetStatusToken(element);
            if (status != null && token >= 0)
            {
                status.PopStatus(token);
            }
        }

        // TODO: should we make this method public?
        /// <summary>
        /// Gets the text of the tooltip.
        /// <list type="bullet">
        /// <item>If <paramref name="obj"/> is a <see cref="string"/> returns it.</item>
        /// <item>If <paramref name="obj"/> is a <see cref="TextBlock"/> returns its <see cref="TextBlock.Text"/> or the first inline of type <see cref="Run"/>.</item>
        /// <item>If <paramref name="obj"/> is a <see cref="FrameworkElement"/> gets the first child of type <see cref="TextBlock"/> and returns its <see cref="TextBlock.Text"/> or the first inline of type <see cref="Run"/>.</item>
        /// </list> 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string GetTooltipText(object obj)
        {
            var str = obj as string;
            if (str != null)
                return str;
            
            var txt = obj as TextBlock;
            if (txt == null)
            {
                var element = obj as Control;
                if (element == null)
                    return null;

                // TODO: should we aggregate all textblocks?
                //txt = element.FindVisualChildOfType<TextBlock>(); 
                txt = Avalonia.VisualTree.VisualExtensions.FindDescendantOfType<TextBlock>(element); 
            }

            if (txt == null)
                return null;

            if (txt.Inlines.Count == 0)
                return txt.Text;

            // TODO: should we aggregate all inline elements?
            var run = txt.Inlines.OfType<Run>().FirstOrDefault();
            return run?.Text;
        }
    }
}
