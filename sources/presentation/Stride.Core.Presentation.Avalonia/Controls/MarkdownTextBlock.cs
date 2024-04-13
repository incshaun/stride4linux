// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using Avalonia;
using Avalonia.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Documents;
using Avalonia.Input;
using Stride.Core.Annotations;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Metadata;
using System.Windows.Input;

namespace Stride.Core.Presentation.Controls
{
    // FIXME: placeholder class.
    public class XamlMarkdown
    {
        public string BaseUrl;
        public ICommand HyperlinkCommand;
        public XamlMarkdown (MarkdownTextBlock tb)
        {
        }
        public string Transform (string text)
        {
            return text;
        }
    }
    
    [TemplatePart(Name = MessageContainerPartName, Type = typeof(ScrollViewer))]
    public class MarkdownTextBlock : TemplatedControl
    {
        /// <summary>
        /// The name of the part for the <see cref="ScrollViewer"/> container.
        /// </summary>
        private const string MessageContainerPartName = "PART_MessageContainer";

        /// <summary>
        /// Identifies the <see cref="BaseUrl"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<string> BaseUrlProperty = StyledProperty<string>.Register<MarkdownTextBlock, string>(nameof(BaseUrl)); // T4A
        /// <summary>
        /// Identifies the <see cref="HyperlinkCommand"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<ICommand> HyperlinkCommandProperty = StyledProperty<ICommand>.Register<MarkdownTextBlock, ICommand>(nameof(HyperlinkCommand)); // T4A
        /// <summary>
        /// Identifies the <see cref="Markdown"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<XamlMarkdown> MarkdownProperty = StyledProperty<XamlMarkdown>.Register<MarkdownTextBlock, XamlMarkdown>(nameof(Markdown)); // T4A
        /// <summary>
        /// Identifies the <see cref="Text"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<string> TextProperty = StyledProperty<string>.Register<MarkdownTextBlock, string>(nameof(Text)); // T4A

        public string BaseUrl
        {
            get { return (string)GetValue(BaseUrlProperty); }
            set { SetValue(BaseUrlProperty, value); }
        }

        public ICommand HyperlinkCommand
        {
            get { return (ICommand)GetValue(HyperlinkCommandProperty); }
            set { SetValue(HyperlinkCommandProperty, value); }
        }

        public XamlMarkdown Markdown
        {
            get { return (XamlMarkdown)GetValue(MarkdownProperty); }
            set { SetValue(MarkdownProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// The container in which the message is displayed.
        /// </summary>
        private ScrollViewer messageContainer;

        /// <summary>
        /// Default markdown used if none is supplied.
        /// </summary>
        private readonly Lazy<XamlMarkdown> defaultMarkdown;

        static MarkdownTextBlock()
		{
			BaseUrlProperty.Changed.AddClassHandler<MarkdownTextBlock>(BaseUrlChanged);
			HyperlinkCommandProperty.Changed.AddClassHandler<MarkdownTextBlock>(HyperlinkCommandChanged);
			MarkdownProperty.Changed.AddClassHandler<MarkdownTextBlock>(MarkdownChanged);
			TextProperty.Changed.AddClassHandler<MarkdownTextBlock>(TextChanged);

            // FIXME  T31
        }

        public MarkdownTextBlock()
        {
            defaultMarkdown = new Lazy<XamlMarkdown>(() => new XamlMarkdown(this));
        }

        /// <inheritdoc/>
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
		{
			base.OnApplyTemplate(e);

            messageContainer = e.NameScope.Find<ScrollViewer>(MessageContainerPartName);
            if (messageContainer == null)
                throw new InvalidOperationException($"A part named '{MessageContainerPartName}' must be present in the ControlTemplate, and must be of type '{typeof(ScrollViewer)}'.");

            ResetMessage();
        }

        private static void BaseUrlChanged([NotNull] AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var control = d as MarkdownTextBlock;
            if (control == null) throw new ArgumentNullException(nameof(control));

            if (e.NewValue != null)
            {
                control.GetMarkdown().BaseUrl = (string)e.NewValue;
            }
            control.ResetMessage();
        }

        private static void HyperlinkCommandChanged([NotNull] AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var control = d as MarkdownTextBlock;
            if (control == null) throw new ArgumentNullException(nameof(control));

            if (e.NewValue != null)
            {
                control.GetMarkdown().HyperlinkCommand = (ICommand)e.NewValue;
            }
            control.ResetMessage();
        }

        private static void MarkdownChanged([NotNull] AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var control = d as MarkdownTextBlock;
            if (control == null) throw new ArgumentNullException(nameof(control));

            if (e.NewValue != null)
            {
                ((XamlMarkdown)e.NewValue).BaseUrl = control.BaseUrl;
                ((XamlMarkdown)e.NewValue).HyperlinkCommand = control.HyperlinkCommand;
            }
            control.ResetMessage();
        }

        private static void TextChanged([NotNull] AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var control = d as MarkdownTextBlock;
            if (control == null) throw new ArgumentNullException(nameof(control));

            control.ResetMessage();
        }

        [NotNull]
        private XamlMarkdown GetMarkdown()
        {
            return Markdown ?? defaultMarkdown.Value;
        }

        private void ResetMessage()
        {
            if (messageContainer != null)
            {
//                 messageContainer.Document = ProcessText();
            }
        }

        [CanBeNull]
        private string ProcessText()
        {
            try
            {
                return GetMarkdown().Transform(Text ?? "*Nothing to display*");
            }
            catch (ArgumentException) { }
            catch (FormatException) { }
            catch (InvalidOperationException) { }

            return null;
        }
    }
}
