// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Stride.Core.Storage;
using Stride.Core.Presentation.Services;
using Stride.Core.Presentation.ViewModels;

namespace Stride.Core.Assets.Editor.Services
{
    public abstract partial class ThumbnailData : ViewModelBase
    {
        protected readonly ObjectId thumbnailId;
        protected ThumbnailData(ObjectId thumbnailId)
        {
            this.thumbnailId = thumbnailId;
        }

        
        public async Task PrepareForPresentation(IDispatcherService dispatcher) {}
                
    }

    public sealed partial class ResourceThumbnailData : ThumbnailData
    {
        object resourceKey;
        public ResourceThumbnailData(ObjectId thumbnailId, object resourceKey) : base(thumbnailId)
        {
            this.resourceKey = resourceKey;
        }
        
    }

    /// <summary>
    /// Byte streams bitmap support for thumbnails.
    /// </summary>
    public sealed partial class BitmapThumbnailData : ThumbnailData
    {
        private Stream thumbnailBitmapStream;
        public BitmapThumbnailData(ObjectId thumbnailId, Stream thumbnailBitmapStream) : base(thumbnailId)
        {
            this.thumbnailBitmapStream = thumbnailBitmapStream;
        }
    }
}
