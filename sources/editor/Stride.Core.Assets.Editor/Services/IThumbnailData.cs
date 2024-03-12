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
            Console.WriteLine ("Calling base");
            return null;
        }
// 
//         
//                 
    }

    public class ThumbnailDataBase
    {
        private static Dictionary <string, object> thumbSource = new Dictionary <string, object> ();
        
        public static void addSource (string name, object value)
        {
//             thumbSource.Add (name, value);
            thumbSource[name] = value;
        }
        
        public static IThumbnailData createThumb (string name, ObjectId thumbnailId, object param)
        {
            Console.WriteLine ("Creating thumb: " + name);
            Console.WriteLine ("Creating thumb: " + name + " " + thumbSource[name]);
            return ((IThumbnailData) (thumbSource[name])).Build (thumbnailId, param);
        }
    }
    
//     public partial class ThumbnailData : ViewModelBase, IThumbnailData
//     {
//         protected readonly ObjectId thumbnailId;
//         private static readonly Dictionary<ObjectId, Task<object>> ComputingThumbnails = new Dictionary<ObjectId, Task<object>>();
//         private object presenter;
// 
//         protected delegate object BuildImageSourceMethod ();
//         protected static BuildImageSourceMethod bisMethod;
// 
//         public object Presenter { get { return presenter; } set { SetValue(ref presenter, value); } }
//            
//         protected ThumbnailData(ObjectId thumbnailId)
//         {
//             this.thumbnailId = thumbnailId;
//         }
//         public async Task PrepareForPresentation(IDispatcherService dispatcher)
//         {
//             Console.WriteLine ("Right place222 " + bisMethod);
//             Task<object> task;
//             lock (ComputingThumbnails)
//             {
//                 if (!ComputingThumbnails.TryGetValue(thumbnailId, out task))
//                 {
//                     task = Task.Run(() => bisMethod());
//                     ComputingThumbnails.Add(thumbnailId, task);
//                 }
//             }
// 
//             var result = await task;
//             dispatcher.Invoke(() => Presenter = result);
// //             FreeBuildingResources();
// 
//             lock (ComputingThumbnails)
//             {
//                 ComputingThumbnails.Remove(thumbnailId);
//             }
//         }
//         
// //         public abstract partial Task PrepareForPresentation(IDispatcherService dispatcher);
// //         public Task PrepareForPresentation(IDispatcherService dispatcher) { Console.WriteLine ("Wrong place"); return null; }
//         /// <summary>
//         /// Clears the resources required to build the image source.
//         /// </summary>
// //         protected abstract void FreeBuildingResources();
//     }
//     
//     public partial class ResourceThumbnailData : ThumbnailData
//     {
//         object resourceKey;
//         public ResourceThumbnailData(ObjectId thumbnailId, object resourceKey) : base(thumbnailId)
//         {
//             this.resourceKey = resourceKey;
//         }
// 
//         
//         /// <inheritdoc />
// //         protected override void FreeBuildingResources()
// //         {
// //             resourceKey = null;
// //         }
//     }
// 
//     /// <summary>
//     /// Byte streams bitmap support for thumbnails.
//     /// </summary>
//     public sealed partial class BitmapThumbnailData : ThumbnailData
//     {
//         private Stream thumbnailBitmapStream;
//         public BitmapThumbnailData(ObjectId thumbnailId, Stream thumbnailBitmapStream) : base(thumbnailId)
//         {
//             this.thumbnailBitmapStream = thumbnailBitmapStream;
//         }
//         
//         /// <inheritdoc />
// //         protected override void FreeBuildingResources()
// //         {
// //             thumbnailBitmapStream = null;
// //         }
//     }
}
