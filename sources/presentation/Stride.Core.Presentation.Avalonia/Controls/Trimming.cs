// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Avalonia;
using Avalonia.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Media;
using Stride.Core.Annotations;
using Avalonia.Media;

namespace Stride.Core.Presentation.Controls
{
    public class Trimming
    {
        /// <summary>
        /// The string used as ellipsis for trimming.
        /// </summary>
        public const string Ellipsis = "…";

        /// <summary>
        /// Identifies the <see cref="TextTrimming"/> dependency property.
        /// </summary>
        public static readonly AttachedProperty<TextTrimming> TextTrimmingProperty = AvaloniaProperty<TextTrimming>.RegisterAttached<Trimming, Control, TextTrimming>("TextTrimming", TextTrimming.None);

        /// <summary>
        /// Identifies the <see cref="TrimmingSource"/> dependency property.
        /// </summary>
        public static readonly AttachedProperty<TrimmingSource> TrimmingSourceProperty = AvaloniaProperty<TrimmingSource>.RegisterAttached<Trimming, Control, TrimmingSource>("TrimmingSource", TrimmingSource.End);

        /// <summary>
        /// Identifies the <see cref="WordSeparators"/> dependency property.
        /// </summary>
        public static readonly AttachedProperty<string> WordSeparatorsProperty = AvaloniaProperty<string>.RegisterAttached<Trimming, Control, string>("WordSeparators", " \t");

        /// <summary>
        /// Gets the current value of the <see cref="TextTrimming"/> dependency property attached to the given <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="target">The target <see cref="DependencyObject"/>.</param>
        /// <returns>The value of the <see cref="TextTrimming"/> dependency property.</returns>
        public static TextTrimming GetTextTrimming([NotNull] AvaloniaObject target)
        {
            return (TextTrimming)target.GetValue(TextTrimmingProperty);
        }

        /// <summary>
        /// Sets the value of the <see cref="TextTrimming"/> dependency property attached to the given <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="target">The target <see cref="DependencyObject"/>.</param>
        /// <param name="value">The value to set.</param>
        public static void SetTextTrimming([NotNull] AvaloniaObject target, TextTrimming value)
        {
            target.SetValue(TextTrimmingProperty, value);
        }

        /// <summary>
        /// Gets the current value of the <see cref="TrimmingSource"/> dependency property attached to the given <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="target">The target <see cref="DependencyObject"/>.</param>
        /// <returns>The value of the <see cref="TrimmingSource"/> dependency property.</returns>
        public static TrimmingSource GetTrimmingSource([NotNull] AvaloniaObject target)
        {
            return (TrimmingSource)target.GetValue(TrimmingSourceProperty);
        }

        /// <summary>
        /// Sets the value of the <see cref="TrimmingSource"/> dependency property attached to the given <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="target">The target <see cref="DependencyObject"/>.</param>
        /// <param name="value">The value to set.</param>
        public static void SetTrimmingSource([NotNull] AvaloniaObject target, TrimmingSource value)
        {
            target.SetValue(TrimmingSourceProperty, value);
        }

        /// <summary>
        /// Gets the current value of the <see cref="WordSeparators"/> dependency property attached to the given <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="target">The target <see cref="DependencyObject"/>.</param>
        /// <returns>The value of the <see cref="WordSeparators"/> dependency property.</returns>
        public static string GetWordSeparators([NotNull] AvaloniaObject target)
        {
            return (string)target.GetValue(WordSeparatorsProperty);
        }

        /// <summary>
        /// Sets the value of the <see cref="WordSeparators"/> dependency property attached to the given <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="target">The target <see cref="DependencyObject"/>.</param>
        /// <param name="value">The value to set.</param>
        public static void SetWordSeparators([NotNull] AvaloniaObject target, string value)
        {
            target.SetValue(WordSeparatorsProperty, value);
        }

        public static string ProcessTrimming([NotNull] Control control, string text, double availableWidth)
        {
            var trimming = GetTextTrimming(control);
            var source = GetTrimmingSource(control);
            var wordSeparators = GetWordSeparators(control);
            return ProcessTrimming(control, text, trimming, source, wordSeparators, availableWidth);
        }

        public static string ProcessTrimming([NotNull] Control control, string text, TextTrimming trimming, TrimmingSource source, string wordSeparators, double availableWidth)
        {
            var typeface = Typeface.Default;
            double fontSize = 10.0;
            
            if (control.GetType ().GetProperty ("FontFamily") != null)
            {
                typeface = new Typeface((FontFamily) control.GetType ().GetProperty("FontFamily").GetValue (control), (FontStyle) control.GetType ().GetProperty("FontStyle").GetValue (control), (FontWeight) control.GetType ().GetProperty("FontWeight").GetValue (control), (FontStretch) control.GetType ().GetProperty("FontStretch").GetValue (control));
            }
            if (control.GetType ().GetProperty ("FontSize") != null)
            {
                fontSize = (double) control.GetType ().GetProperty ("FontSize").GetValue (control);
            }
            
//             var typeface = new Typeface(control.FontFamily, control.FontStyle, control.FontWeight, control.FontStretch);
            return ProcessTrimming(text, typeface, fontSize, trimming, source, wordSeparators, availableWidth);
        }

        public static string ProcessTrimming([NotNull] TextBlock textBlock, string text, double availableWidth)
        {
            var trimming = GetTextTrimming(textBlock);
            var source = GetTrimmingSource(textBlock);
            var wordSeparators = GetWordSeparators(textBlock);
            return ProcessTrimming(textBlock, text, trimming, source, wordSeparators, availableWidth);
        }

        public static string ProcessTrimming([NotNull] TextBlock textBlock, string text, TextTrimming trimming, TrimmingSource source, string wordSeparators, double availableWidth)
        {
            var typeface = new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch);
            return ProcessTrimming(text, typeface, textBlock.FontSize, trimming, source, wordSeparators, availableWidth);
        }

        private static string ProcessTrimming(string text, [NotNull] Typeface typeface, double fontSize, TextTrimming trimming, TrimmingSource source, string wordSeparators, double availableWidth)
        {
            if (trimming == TextTrimming.None)
            {
                return text;
            }

            var textWidth = GetTextWidth(text, trimming, typeface, fontSize, wordSeparators, out double[] sizes);
            if (availableWidth >= textWidth)
            {
                return text;
            }
            if (sizes.Length == 0)
            {
                return text;
            }

            List<string> words;

            if (trimming == TextTrimming.CharacterEllipsis)
            {
                    words = text.ToCharArray().Select(c => c.ToString(CultureInfo.InvariantCulture)).ToList();
            }
            else if (trimming == TextTrimming.WordEllipsis)
            {
                    words = SplitWords(text, wordSeparators);
            }
            else
            {
                    throw new ArgumentException("Invalid 'TextTrimming' argument.");
            }

            var firstWord = true;

            switch (source)
            {
                case TrimmingSource.Begin:
                {
                    var currentWidth = GetTextWidth(Ellipsis, trimming, typeface, fontSize, wordSeparators, out _);

                    var ending = new StringBuilder();
                    for (var i = words.Count - 1; i >= 0; --i)
                    {
                        var test = currentWidth + sizes[i];

                        if (test > availableWidth)
                        {
                            // If there's not enough room for a single word, fall back on character trimming from the beginning
                            if (trimming == TextTrimming.WordEllipsis && firstWord)
                            {
                                return ProcessTrimming(words[i], typeface, fontSize, TextTrimming.CharacterEllipsis, TrimmingSource.Begin, wordSeparators, availableWidth);
                            }
                            break;
                        }
                        ending.Insert(0, words[i]);
                        currentWidth = test;
                        firstWord = false;
                    }

                    return $"{Ellipsis}{ending}";
                }
                case TrimmingSource.Middle:
                {
                    var currentWidth = GetTextWidth(Ellipsis, trimming, typeface, fontSize, wordSeparators, out _);

                    var begin = new StringBuilder();
                    var ending = new StringBuilder();
                    for (int i = 0, j = words.Count - 1; i <= j; ++i, --j)
                    {
                        var test = currentWidth + sizes[i] + (i != j ? sizes[j] : 0);

                        if (test > availableWidth)
                        {
                            // If there's not enough room for a single word, fall back on character trimming from the end
                            if (trimming == TextTrimming.WordEllipsis && firstWord)
                            {
                                return ProcessTrimming(words[j], typeface, fontSize, TextTrimming.CharacterEllipsis, TrimmingSource.End, wordSeparators, availableWidth);
                            }
                            break;
                        }
                        begin.Append(words[i]);
                        if (i != j)
                            ending.Insert(0, words[j]);

                        currentWidth = test;
                        firstWord = false;
                    }

                    return string.Format("{0}{2}{1}", begin, ending, Ellipsis);
                }
                case TrimmingSource.End:
                {
                    var currentWidth = GetTextWidth(Ellipsis, trimming, typeface, fontSize, wordSeparators, out _);

                    var begin = new StringBuilder();
                    for (var i = 0; i < words.Count; ++i)
                    {
                        var test = currentWidth + sizes[i];

                        if (test > availableWidth)
                        {
                            // If there's not enough room for a single word, fall back on character trimming from the end
                            if (trimming == TextTrimming.WordEllipsis && firstWord)
                            {
                                return ProcessTrimming(words[i], typeface, fontSize, TextTrimming.CharacterEllipsis, TrimmingSource.Begin, wordSeparators, availableWidth);
                            }
                            break;
                        }

                        begin.Append(words[i]);
                        currentWidth = test;
                        firstWord = false;
                    }

                    return $"{begin}{Ellipsis}";
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static double GetTextWidth([NotNull] string text, TextTrimming trimming, [NotNull] Typeface typeface, double fontSize, string wordSeparators, [NotNull] out double[] sizes)
        {
            var totalWidth = 0.0;
            // We use a period to ensure space characters will have their actual size used.
            var period = new FormattedText(".", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, fontSize, Brushes.Black);
            var periodWidth = period.Width;

            if (trimming == TextTrimming.CharacterEllipsis)
            {
                    sizes = new double[text.Length];
                    for (var i = 0; i < text.Length; i++)
                    {
                        var token = text[i].ToString(CultureInfo.CurrentUICulture) + ".";
                        var formattedText = new FormattedText(token, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, fontSize, Brushes.Black);
                        var width = formattedText.Width - periodWidth;
                        sizes[i] = width;
                        totalWidth += width;
                    }
                    return totalWidth;
            }
            else if (trimming == TextTrimming.WordEllipsis)
            {
                    var words = SplitWords(text, wordSeparators);
                    sizes = new double[words.Count];
                    for (var i = 0; i < words.Count; i++)
                    {
                        var token = words[i] + ".";
                        var formattedText = new FormattedText(token, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, fontSize, Brushes.Black);
                        var width = formattedText.Width - periodWidth;
                        sizes[i] = width;
                        totalWidth += width;
                    }
                    return totalWidth;
            }
            else
            {
                    throw new ArgumentOutOfRangeException(nameof(trimming));
            }
        }

        [NotNull]
        private static List<string> SplitWords([NotNull] string text, string wordSeparators)
        {
            var words = new List<string>();

            var sb = new StringBuilder();
            foreach (var c in text)
            {
                if (wordSeparators.Contains(c))
                {
                    // Ignore empty entries (ie. double consecutive separators)
                    if (sb.Length > 0)
                    {
                        // Add the current word to the list
                        words.Add(sb.ToString());
                    }

                    // Reset the string builder for the next word
                    sb.Clear();

                    // Add the separator itself, it is still needed in the list
                    words.Add(c.ToString(CultureInfo.CurrentUICulture));
                }
                else
                {
                    // If the current character in not a separator, simply append it to the current word
                    sb.Append(c);
                }
            }

            // We reached the end of the text, add the current word to the list.
            if (sb.Length > 0)
                words.Add(sb.ToString());

            return words;
        }
    }
}
