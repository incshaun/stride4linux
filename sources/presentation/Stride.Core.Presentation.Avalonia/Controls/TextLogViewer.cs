// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Avalonia;
using Avalonia.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using Avalonia.Threading;
using Stride.Core.Annotations;
using Stride.Core.Diagnostics;
using Stride.Core.Extensions;

using Avalonia.Interactivity;
using Avalonia.Controls.Primitives;
using Avalonia.Media.TextFormatting;

namespace Stride.Core.Presentation.Controls
{
    /// <summary>
    /// This control displays a collection of <see cref="ILogMessage"/>.
    /// </summary>
    [TemplatePart(Name = "PART_LogTextBox", Type = typeof(Avalonia.Controls.TextBox))]
    [TemplatePart(Name = "PART_ClearLog", Type = typeof(Button))]
    [TemplatePart(Name = "PART_PreviousResult", Type = typeof(Button))]
    [TemplatePart(Name = "PART_NextResult", Type = typeof(Button))]
    public class TextLogViewer : TemplatedControl
    {
        private readonly List<TextRange> searchMatches = new List<TextRange>();
        private int currentResult;

        /// <summary>
        /// The <see cref="Avalonia.Controls.TextBox"/> in which the log messages are actually displayed.
        /// </summary>
        private Avalonia.Controls.TextBox logTextBox;

        /// <summary>
        /// Identifies the <see cref="LogMessages"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<ICollection<ILogMessage>> LogMessagesProperty = StyledProperty<ICollection<ILogMessage>>.Register<TextLogViewer, ICollection<ILogMessage>>("LogMessages", null);

        /// <summary>
        /// Identifies the <see cref="AutoScroll"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> AutoScrollProperty = StyledProperty<bool>.Register<TextLogViewer, bool>("AutoScroll", true);

        /// <summary>
        /// Identifies the <see cref="IsToolBarVisible"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> IsToolBarVisibleProperty = StyledProperty<bool>.Register<TextLogViewer, bool>("IsToolBarVisible", true);

        /// <summary>
        /// Identifies the <see cref="CanClearLog"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> CanClearLogProperty = StyledProperty<bool>.Register<TextLogViewer, bool>("CanClearLog", true);

        /// <summary>
        /// Identifies the <see cref="CanFilterLog"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> CanFilterLogProperty = StyledProperty<bool>.Register<TextLogViewer, bool>("CanFilterLog", true);

        /// <summary>
        /// Identifies the <see cref="CanSearchLog"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> CanSearchLogProperty = StyledProperty<bool>.Register<TextLogViewer, bool>("CanSearchLog", true);

        /// <summary>
        /// Identifies the <see cref="SearchToken"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<string> SearchTokenProperty = StyledProperty<string>.Register<TextLogViewer, string>("SearchToken", "");

        /// <summary>
        /// Identifies the <see cref="SearchMatchCase"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> SearchMatchCaseProperty = StyledProperty<bool>.Register<TextLogViewer, bool>("SearchMatchCase", false);

        /// <summary>
        /// Identifies the <see cref="SearchMatchWord"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> SearchMatchWordProperty = StyledProperty<bool>.Register<TextLogViewer, bool>("SearchMatchWord", false);

        /// <summary>
        /// Identifies the <see cref="SearchMatchBrush"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<IBrush> SearchMatchBrushProperty = StyledProperty<IBrush>.Register<TextLogViewer, IBrush>("SearchMatchBrush", Brushes.LightSteelBlue);

        /// <summary>
        /// Identifies the <see cref="DebugBrush"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<IBrush> DebugBrushProperty = StyledProperty<IBrush>.Register<TextLogViewer, IBrush>("DebugBrush", Brushes.White);

        /// <summary>
        /// Identifies the <see cref="VerboseBrush"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<IBrush> VerboseBrushProperty = StyledProperty<IBrush>.Register<TextLogViewer, IBrush>("VerboseBrush", Brushes.White);

        /// <summary>
        /// Identifies the <see cref="InfoBrush"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<IBrush> InfoBrushProperty = StyledProperty<IBrush>.Register<TextLogViewer, IBrush>("InfoBrush", Brushes.White);

        /// <summary>
        /// Identifies the <see cref="WarningBrush"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<IBrush> WarningBrushProperty = StyledProperty<IBrush>.Register<TextLogViewer, IBrush>("WarningBrush", Brushes.White);

        /// <summary>
        /// Identifies the <see cref="ErrorBrush"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<IBrush> ErrorBrushProperty = StyledProperty<IBrush>.Register<TextLogViewer, IBrush>("ErrorBrush", Brushes.White);

        /// <summary>
        /// Identifies the <see cref="FatalBrush"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<IBrush> FatalBrushProperty = StyledProperty<IBrush>.Register<TextLogViewer, IBrush>("FatalBrush", Brushes.White);

        /// <summary>
        /// Identifies the <see cref="ShowDebugMessages"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> ShowDebugMessagesProperty = StyledProperty<bool>.Register<TextLogViewer, bool>("ShowDebugMessages", true);

        /// <summary>
        /// Identifies the <see cref="ShowVerboseMessages"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> ShowVerboseMessagesProperty = StyledProperty<bool>.Register<TextLogViewer, bool>("ShowVerboseMessages", true);

        /// <summary>
        /// Identifies the <see cref="ShowInfoMessages"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> ShowInfoMessagesProperty = StyledProperty<bool>.Register<TextLogViewer, bool>("ShowInfoMessages", true);

        /// <summary>
        /// Identifies the <see cref="ShowWarningMessages"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> ShowWarningMessagesProperty = StyledProperty<bool>.Register<TextLogViewer, bool>("ShowWarningMessages", true);

        /// <summary>
        /// Identifies the <see cref="ShowErrorMessages"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> ShowErrorMessagesProperty = StyledProperty<bool>.Register<TextLogViewer, bool>("ShowErrorMessages", true);

        /// <summary>
        /// Identifies the <see cref="ShowFatalMessages"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> ShowFatalMessagesProperty = StyledProperty<bool>.Register<TextLogViewer, bool>("ShowFatalMessages", true);

        /// <summary>
        /// Identifies the <see cref="ShowStacktrace"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> ShowStacktraceProperty = StyledProperty<bool>.Register<TextLogViewer, bool>("ShowStacktrace", true);

        /// <summary>
        /// Initializes a new instance of the <see cref="TextLogViewer"/> class.
        /// </summary>
        static TextLogViewer()
		{
			LogMessagesProperty.Changed.AddClassHandler<TextLogViewer>(LogMessagesPropertyChanged);
			SearchTokenProperty.Changed.AddClassHandler<TextLogViewer>(SearchTokenChanged);
			SearchMatchCaseProperty.Changed.AddClassHandler<TextLogViewer>(SearchTokenChanged);
			SearchMatchWordProperty.Changed.AddClassHandler<TextLogViewer>(SearchTokenChanged);
			SearchMatchBrushProperty.Changed.AddClassHandler<TextLogViewer>(TextPropertyChanged);
			DebugBrushProperty.Changed.AddClassHandler<TextLogViewer>(TextPropertyChanged);
			VerboseBrushProperty.Changed.AddClassHandler<TextLogViewer>(TextPropertyChanged);
			InfoBrushProperty.Changed.AddClassHandler<TextLogViewer>(TextPropertyChanged);
			WarningBrushProperty.Changed.AddClassHandler<TextLogViewer>(TextPropertyChanged);
			ErrorBrushProperty.Changed.AddClassHandler<TextLogViewer>(TextPropertyChanged);
			FatalBrushProperty.Changed.AddClassHandler<TextLogViewer>(TextPropertyChanged);
			ShowDebugMessagesProperty.Changed.AddClassHandler<TextLogViewer>(TextPropertyChanged);
			ShowVerboseMessagesProperty.Changed.AddClassHandler<TextLogViewer>(TextPropertyChanged);
			ShowInfoMessagesProperty.Changed.AddClassHandler<TextLogViewer>(TextPropertyChanged);
			ShowWarningMessagesProperty.Changed.AddClassHandler<TextLogViewer>(TextPropertyChanged);
			ShowErrorMessagesProperty.Changed.AddClassHandler<TextLogViewer>(TextPropertyChanged);
			ShowFatalMessagesProperty.Changed.AddClassHandler<TextLogViewer>(TextPropertyChanged);
			ShowStacktraceProperty.Changed.AddClassHandler<TextLogViewer>(TextPropertyChanged);
		}

		public TextLogViewer()
		{
            Loaded += (s, e) =>
            {
                try
                {
//                     if (AutoScroll)
//                         logTextBox?.ScrollToEnd();
                }
                catch (Exception ex)
                {
                    // It happened a few times that ScrollToEnd throws an exception that crashes the whole application.
                    // Let's ignore it if this happens again.
                    ex.Ignore();
                }
            };
        }

        /// <summary>
        /// Gets or sets the collection of <see cref="ILogMessage"/> to display.
        /// </summary>
        public ICollection<ILogMessage> LogMessages { get { return (ICollection<ILogMessage>)GetValue(LogMessagesProperty); } set { SetValue(LogMessagesProperty, value); } }

        /// <summary>
        /// Gets or sets whether the control should automatically scroll when new lines are added when the scrollbar is already at the bottom.
        /// </summary>
        public bool AutoScroll { get { return (bool)GetValue(AutoScrollProperty); } set { SetValue(AutoScrollProperty, value); } }

        /// <summary>
        /// Gets or sets whether the tool bar should be visible.
        /// </summary>
        public bool IsToolBarVisible { get { return (bool)GetValue(IsToolBarVisibleProperty); } set { SetValue(IsToolBarVisibleProperty, value); } }

        /// <summary>
        /// Gets or sets whether it is possible to clear the log text.
        /// </summary>
        public bool CanClearLog { get { return (bool)GetValue(CanClearLogProperty); } set { SetValue(CanClearLogProperty, value); } }

        /// <summary>
        /// Gets or sets whether it is possible to filter the log text.
        /// </summary>
        public bool CanFilterLog { get { return (bool)GetValue(CanFilterLogProperty); } set { SetValue(CanFilterLogProperty, value); } }

        /// <summary>
        /// Gets or sets whether it is possible to search the log text.
        /// </summary>
        public bool CanSearchLog { get { return (bool)GetValue(CanSearchLogProperty); } set { SetValue(CanSearchLogProperty, value); } }

        /// <summary>
        /// Gets or sets the current search token.
        /// </summary>
        public string SearchToken { get { return (string)GetValue(SearchTokenProperty); } set { SetValue(SearchTokenProperty, value); } }

        /// <summary>
        /// Gets or sets whether the search result should match the case.
        /// </summary>
        public bool SearchMatchCase { get { return (bool)GetValue(SearchMatchCaseProperty); } set { SetValue(SearchMatchCaseProperty, value); } }

        /// <summary>
        /// Gets or sets whether the search result should match whole words only.
        /// </summary>
        public bool SearchMatchWord { get { return (bool)GetValue(SearchMatchWordProperty); } set { SetValue(SearchMatchWordProperty, value); } }

        /// <summary>
        /// Gets or sets the brush used to emphasize search results.
        /// </summary>
        public IBrush SearchMatchBrush { get { return (IBrush)GetValue(SearchMatchBrushProperty); } set { SetValue(SearchMatchBrushProperty, value); } }

        /// <summary>
        /// Gets or sets the brush used to emphasize debug messages.
        /// </summary>
        public IBrush DebugBrush { get { return (IBrush)GetValue(DebugBrushProperty); } set { SetValue(DebugBrushProperty, value); } }

        /// <summary>
        /// Gets or sets the brush used to emphasize verbose messages.
        /// </summary>
        public IBrush VerboseBrush { get { return (IBrush)GetValue(VerboseBrushProperty); } set { SetValue(VerboseBrushProperty, value); } }

        /// <summary>
        /// Gets or sets the brush used to emphasize info messages.
        /// </summary>
        public IBrush InfoBrush { get { return (IBrush)GetValue(InfoBrushProperty); } set { SetValue(InfoBrushProperty, value); } }

        /// <summary>
        /// Gets or sets the brush used to emphasize warning messages.
        /// </summary>
        public IBrush WarningBrush { get { return (IBrush)GetValue(WarningBrushProperty); } set { SetValue(WarningBrushProperty, value); } }

        /// <summary>
        /// Gets or sets the brush used to emphasize error messages.
        /// </summary>
        public IBrush ErrorBrush { get { return (IBrush)GetValue(ErrorBrushProperty); } set { SetValue(ErrorBrushProperty, value); } }

        /// <summary>
        /// Gets or sets the brush used to emphasize fatal messages.
        /// </summary>
        public IBrush FatalBrush { get { return (IBrush)GetValue(FatalBrushProperty); } set { SetValue(FatalBrushProperty, value); } }

        /// <summary>
        /// Gets or sets whether the log viewer should display debug messages.
        /// </summary>
        public bool ShowDebugMessages { get { return (bool)GetValue(ShowDebugMessagesProperty); } set { SetValue(ShowDebugMessagesProperty, value); } }

        /// <summary>
        /// Gets or sets whether the log viewer should display verbose messages.
        /// </summary>
        public bool ShowVerboseMessages { get { return (bool)GetValue(ShowVerboseMessagesProperty); } set { SetValue(ShowVerboseMessagesProperty, value); } }

        /// <summary>
        /// Gets or sets whether the log viewer should display info messages.
        /// </summary>
        public bool ShowInfoMessages { get { return (bool)GetValue(ShowInfoMessagesProperty); } set { SetValue(ShowInfoMessagesProperty, value); } }

        /// <summary>
        /// Gets or sets whether the log viewer should display warning messages.
        /// </summary>
        public bool ShowWarningMessages { get { return (bool)GetValue(ShowWarningMessagesProperty); } set { SetValue(ShowWarningMessagesProperty, value); } }

        /// <summary>
        /// Gets or sets whether the log viewer should display error messages.
        /// </summary>
        public bool ShowErrorMessages { get { return (bool)GetValue(ShowErrorMessagesProperty); } set { SetValue(ShowErrorMessagesProperty, value); } }

        /// <summary>
        /// Gets or sets whether the log viewer should display fatal messages.
        /// </summary>
        public bool ShowFatalMessages { get { return (bool)GetValue(ShowFatalMessagesProperty); } set { SetValue(ShowFatalMessagesProperty, value); } }

        /// <summary>
        /// Gets or sets whether the log viewer should display fatal messages.
        /// </summary>
        public bool ShowStacktrace { get { return (bool)GetValue(ShowStacktraceProperty); } set { SetValue(ShowStacktraceProperty, value); } }

        /// <inheritdoc/>
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
		{
			base.OnApplyTemplate(e);

            logTextBox = e.NameScope.Find<Avalonia.Controls.TextBox>("PART_LogTextBox");
            if (logTextBox == null)
                throw new InvalidOperationException("A part named 'PART_LogTextBox' must be present in the ControlTemplate, and must be of type 'Avalonia.Controls.TextBox'.");

            var clearLogButton = e.NameScope.Find<Button>("PART_ClearLog");
            if (clearLogButton != null)
            {
                clearLogButton.Click += ClearLog;
            }

            var previousResultButton = e.NameScope.Find<Button>("PART_PreviousResult");
            if (previousResultButton != null)
            {
                previousResultButton.Click += PreviousResultClicked;
            }
            var nextResultButton = e.NameScope.Find<Button>("PART_NextResult");
            if (nextResultButton != null)
            {
                nextResultButton.Click += NextResultClicked;
            }

            ResetText();
        }

        private void ClearLog(object sender, RoutedEventArgs e)
        {
            LogMessages.Clear();
        }

        private void ResetText()
        {
            if (logTextBox != null)
            {
                ClearSearchResults();
                var document = new string("");
                if (LogMessages != null)
                {
                    logTextBox.Text = "";
                    var logMessages = LogMessages.ToList();
                    AppendText(document, logMessages);
                }
                else
                    logTextBox.Text = document;
            }
        }

        private void AppendText([NotNull] string document, [NotNull] IEnumerable<ILogMessage> logMessages)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (logMessages == null) throw new ArgumentNullException(nameof(logMessages));
            if (logTextBox != null)
            {
//                 logTextBox.Text += System.Environment.NewLine + string.Join(System.Environment.NewLine, logMessages);
//                 var paragraph = (Paragraph)document.Blocks.AsEnumerable().First();
                var stringComparison = SearchMatchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
                var searchToken = SearchToken;
                var sb = new StringBuilder();
                foreach (var message in logMessages.Where(x => ShouldDisplayMessage(x.Type)))
                {
                    sb.Clear();

                    if (message.Module != null)
                    {
                        sb.AppendFormat("[{0}]: ", message.Module);
                    }

                    sb.AppendFormat("{0}: {1}", message.Type, message.Text);
                    
                    var ex = message.ExceptionInfo;
                    if (ex != null)
                    {
                        if (ShowStacktrace)
                        {
                            sb.AppendFormat("{0}{1}{0}", Environment.NewLine, ex);
                        }
                        else
                        {
                            sb.Append(" (...)");
                        }
                    }

                    sb.AppendLine();

                    var lineText = sb.ToString();

                    var logColor = GetLogColor(message.Type);
                    if (string.IsNullOrEmpty(searchToken))
                    {
                        //paragraph.Inlines.Add(new Run(lineText) { Foreground = logColor });
                        logTextBox.Text += lineText;
                    }
                    else
                    {
                        do
                        {
                            var tokenIndex = lineText.IndexOf(searchToken, stringComparison);
                            if (tokenIndex == -1)
                            {
                                //paragraph.Inlines.Add(new Run(lineText) { Foreground = logColor });
                                logTextBox.Text += lineText;
                                break;
                            }
                            var acceptResult = true;
                            if (SearchMatchWord && lineText.Length > 1)
                            {
                                if (tokenIndex > 0)
                                {
                                    var c = lineText[tokenIndex - 1];
                                    if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                                        acceptResult = false;
                                }
                                if (tokenIndex + searchToken.Length < lineText.Length)
                                {
                                    var c = lineText[tokenIndex + searchToken.Length];
                                    if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                                        acceptResult = false;
                                }
                            }

                            if (acceptResult)
                            {
                                if (tokenIndex > 0)
                                    //paragraph.Inlines.Add(new Run(lineText.Substring(0, tokenIndex)) { Foreground = logColor });
                                    logTextBox.Text += lineText.Substring(0, tokenIndex);

//                                 var tokenRun = new Run(lineText.Substring(tokenIndex, searchToken.Length)) { Background = SearchMatchBrush, Foreground = logColor };
//                                 //paragraph.Inlines.Add(tokenRun);
//                                 logTextBox.Text += tokenRun;
//                                 var tokenRange = new TextRange(tokenRun.ContentStart, tokenRun.ContentEnd);
//                                 searchMatches.Add(tokenRange);
                                logTextBox.Text += "[" + lineText.Substring(tokenIndex, searchToken.Length) + "]";
                                lineText = lineText.Substring(tokenIndex + searchToken.Length);
                            }
                        } while (lineText.Length > 0);
                    }
                }
            }
        }


        private void ClearSearchResults()
        {
            searchMatches.Clear();
        }

        private void SelectFirstOccurrence()
        {
            if (searchMatches.Count > 0)
            {
                SelectSearchResult(0);
            }
        }

        private void SelectPreviousOccurrence()
        {
            if (searchMatches.Count > 0)
            {
                var previousResult = (searchMatches.Count + currentResult - 1) % searchMatches.Count;
                SelectSearchResult(previousResult);
            }
        }

        private void SelectNextOccurrence()
        {
            if (searchMatches.Count > 0)
            {
                var nextResult = (currentResult + 1) % searchMatches.Count;
                SelectSearchResult(nextResult);
            }
        }

        private void SelectSearchResult(int resultIndex)
        {
            var result = searchMatches[resultIndex];
//             logTextBox.Selection.Select(result.Start, result.End);
//             var selectionRect = logTextBox.Selection.Start.GetCharacterRect(LogicalDirection.Forward);
//             var offset = selectionRect.Top + logTextBox.VerticalOffset;
//             logTextBox.ScrollToVerticalOffset(offset - logTextBox.ActualHeight / 2);
//             logTextBox.BringIntoView();
            currentResult = resultIndex;
        }

        private bool ShouldDisplayMessage(LogMessageType type)
        {
            switch (type)
            {
                case LogMessageType.Debug:
                    return ShowDebugMessages;
                case LogMessageType.Verbose:
                    return ShowVerboseMessages;
                case LogMessageType.Info:
                    return ShowInfoMessages;
                case LogMessageType.Warning:
                    return ShowWarningMessages;
                case LogMessageType.Error:
                    return ShowErrorMessages;
                case LogMessageType.Fatal:
                    return ShowFatalMessages;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }

        private IBrush GetLogColor(LogMessageType type)
        {
            switch (type)
            {
                case LogMessageType.Debug:
                    return DebugBrush;
                case LogMessageType.Verbose:
                    return VerboseBrush;
                case LogMessageType.Info:
                    return InfoBrush;
                case LogMessageType.Warning:
                    return WarningBrush;
                case LogMessageType.Error:
                    return ErrorBrush;
                case LogMessageType.Fatal:
                    return FatalBrush;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }

        private static void TextPropertyChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var logViewer = (TextLogViewer)d;
            logViewer.ResetText();
//             logViewer.logTextBox?.ScrollToEnd();
        }

        /// <summary>
        /// Raised when the <see cref="LogMessages"/> dependency property is changed.
        /// </summary>
        private static void LogMessagesPropertyChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var logViewer = (TextLogViewer)d;
            var oldValue = e.OldValue as ICollection<ILogMessage>;
            var newValue = e.NewValue as ICollection<ILogMessage>;
            if (oldValue != null)
            {
                // ReSharper disable SuspiciousTypeConversion.Global - go home resharper, you're drunk
                var notifyCollectionChanged = oldValue as INotifyCollectionChanged;
                // ReSharper restore SuspiciousTypeConversion.Global
                if (notifyCollectionChanged != null)
                {
                    notifyCollectionChanged.CollectionChanged -= logViewer.LogMessagesCollectionChanged;
                }
            }
            if (e.NewValue != null)
            {
                // ReSharper disable SuspiciousTypeConversion.Global - go home resharper, you're drunk
                var notifyCollectionChanged = newValue as INotifyCollectionChanged;
                // ReSharper restore SuspiciousTypeConversion.Global
                if (notifyCollectionChanged != null)
                {
                    notifyCollectionChanged.CollectionChanged += logViewer.LogMessagesCollectionChanged;
                }
            }
            logViewer.ResetText();
        }

        /// <summary>
        /// Raised when the <see cref="SearchToken"/> property is changed.
        /// </summary>
        private static void SearchTokenChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var logViewer = (TextLogViewer)d;
            logViewer.ResetText();
            logViewer.SelectFirstOccurrence();
        }

        /// <summary>
        /// Raised when the collection of log messages is observable and changes.
        /// </summary>
        private void LogMessagesCollectionChanged(object sender, [NotNull] NotifyCollectionChangedEventArgs e)
        {
//             var shouldScroll = AutoScroll && logTextBox != null && logTextBox.ExtentHeight - logTextBox.ViewportHeight - logTextBox.VerticalOffset < 1.0;
            var shouldScroll = AutoScroll;

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems != null)
                {
                    if (logTextBox != null)
                    {
                        if (logTextBox.Text == null)
                        {
                            logTextBox.Text = new string("");
                        }
                        AppendText(logTextBox.Text, e.NewItems.Cast<ILogMessage>());
                    }
                }
            }
            else
            {
                ResetText();
            }

            if (shouldScroll)
            {
                // Sometimes crashing with ExecutionEngineException in Window.GetWindowMinMax() if not ran with a dispatcher low priority.
                // Note: priority should still be higher than DispatcherPriority.Input so that user input have a chance to scroll.
//                 Dispatcher.InvokeAsync(() => logTextBox.ScrollToEnd(), DispatcherPriority.DataBind);
            }
        }

        private void PreviousResultClicked(object sender, RoutedEventArgs e)
        {
            SelectPreviousOccurrence();
        }

        private void NextResultClicked(object sender, RoutedEventArgs e)
        {
            SelectNextOccurrence();
        }
    }
}
