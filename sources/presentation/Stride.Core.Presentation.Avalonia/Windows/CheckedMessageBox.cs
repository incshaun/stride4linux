// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;

using Stride.Core.Annotations;
using Stride.Core.Presentation.Services;

namespace Stride.Core.Presentation.Windows
{
    using MessageBoxImage = Services.MessageBoxImage;

    public class CheckedMessageBox : MessageBox
    {
        /// <summary>
        /// Identifies the <see cref="CheckedMessage"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<string> CheckedMessageProperty = StyledProperty<string>.Register<CheckedMessageBox, string>(nameof(CheckedMessage)); // T1

        /// <summary>
        /// Identifies the <see cref="IsCheckedProperty"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool?> IsCheckedProperty = StyledProperty<bool?>.Register<CheckedMessageBox, bool?>(nameof(IsChecked)); // T1

        public string CheckedMessage
        {
            get { return (string)GetValue(CheckedMessageProperty); }
            set { SetValue(CheckedMessageProperty, value); }
        }

        public bool? IsChecked
        {
            get { return (bool?)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        [NotNull]
        public static async Task<CheckedMessageBoxResult> Show(string message, string caption, [NotNull] IEnumerable<DialogButtonInfo> buttons, MessageBoxImage image, string checkedMessage, bool? isChecked)
        {
            var buttonList = buttons.ToList();
            var messageBox = new CheckedMessageBox
            {
                Title = caption,
                Content = message,
                ButtonsSource = buttonList,
                CheckedMessage = checkedMessage,
                IsChecked = isChecked,
            };
            SetImage(messageBox, image);
            SetKeyBindings(messageBox, buttonList);

            await messageBox.ShowModal();
            var result = messageBox.ButtonResult;
            return new CheckedMessageBoxResult(result, messageBox.IsChecked);
        }
    }
}
