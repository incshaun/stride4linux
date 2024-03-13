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
    public interface IThumbnailData
    {
        public abstract Task PrepareForPresentation(IDispatcherService dispatcher);
        
        public virtual IThumbnailData Build (ObjectId thumbnailId, object param) 
        {
            return null;
        }
    }

    public class ThumbnailDataBase
    {
        private static Dictionary <string, object> thumbSource = new Dictionary <string, object> ();
        
        public static void addSource (string name, object value)
        {
            thumbSource[name] = value;
        }
        
        public static IThumbnailData createThumb (string name, ObjectId thumbnailId, object param)
        {
            return ((IThumbnailData) (thumbSource[name])).Build (thumbnailId, param);
        }
    }
}
