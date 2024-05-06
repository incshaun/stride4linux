// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Stride.Core.Annotations;
using Stride.Core.Presentation.Services;

namespace Stride.Core.Presentation.Dialogs
{
    public abstract class ModalDialogBase : IModalDialogInternal
    {
        private readonly IDispatcherService dispatcher;

        protected ModalDialogBase([NotNull] IDispatcherService dispatcher)
        {
            if (dispatcher == null) throw new ArgumentNullException(nameof(dispatcher));
            this.dispatcher = dispatcher;
        }

        /// <param name="result"></param>
        /// <inheritdoc/>
        public void RequestClose(DialogResult result)
        {
            throw new NotSupportedException("RequestClose is not supported for this dialog.");
        }

        /// <inheritdoc/>
        public object DataContext { get; set; }

        /// <inheritdoc/>
        public DialogResult Result { get; set; }

        [NotNull]
        public abstract Task<DialogResult> ShowModal();
    }
}
