// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
#if STRIDE_GRAPHICS_API_OPENGL
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Silk.NET.Core.Contexts;

namespace Stride.Graphics
{
    /// <summary>
    /// Used internally to provide a context for async resource creation
    /// (such as texture or buffer created on a thread where no context is active).
    /// </summary>
    public struct UseOpenGLCreationContext : IDisposable
    {
        public readonly CommandList CommandList;

       private readonly bool useDeviceCreationContext;
        private readonly bool needUnbindContext = false;

        private  bool asyncCreationLockTaken = false;
        private readonly object asyncCreationLockObject;

        private readonly IGLContext deviceCreationContext;
        private readonly GL GL;

       public bool UseDeviceCreationContext => useDeviceCreationContext;


private GraphicsDevice gd;
public UseOpenGLCreationContext(GraphicsDevice graphicsDevice)
            : this()
        {
#if STRIDE_GRAPHICS_API_OPENGLES            
            // Version for use where only a single graphics context is allowed (including where shared contexts cannot be used simultaneously).
            // Does enforce a lock on the context, so will cause deadlock if waiting for another thread to deliver some graphics related output
            // while holding the thread. 
            // Assume the lock will be held for practically an entire frame, since state may be lost if switching during the frame render.
            // Ultimately, need a solution that scales from 1 to n shared contexts.
//             Thread thread = Thread.CurrentThread;
// Console.WriteLine ("DOTNET UseOpenGLCreationContext A " + graphicsDevice.CurrentGraphicsContext + " - " + thread.ManagedThreadId + " - " + thread.Name + "--" + graphicsDevice.GetHashCode () + " " + Environment.StackTrace);            

            GL = graphicsDevice.GL;
            if (graphicsDevice.CurrentGraphicsContext != IntPtr.Zero)
            {
                // Console.WriteLine ("DOTNET UseOpenGLCreationContext Reentry!!!!! " + graphicsDevice.CurrentGraphicsContext + " - " + thread.ManagedThreadId + " - " + thread.Name + "--" + graphicsDevice.GetHashCode () + " " + Environment.StackTrace);            
            }
            //             if (graphicsDevice.CurrentGraphicsContext == IntPtr.Zero)
            {
                // Lock, since there is only one deviceCreationContext.
                // TODO: Support multiple deviceCreationContext (TLS creation of context was crashing, need to investigate why)
                asyncCreationLockObject = graphicsDevice.asyncCreationLockObject;
                // Console.WriteLine ("DOTNET UseOpenGLCreationContext Monitor enter " + graphicsDevice.CurrentGraphicsContext + " - " + thread.ManagedThreadId + " - " + thread.Name + "--" + graphicsDevice.GetHashCode () + " " + asyncCreationLockTaken);            
                Monitor.Enter(graphicsDevice.asyncCreationLockObject, ref asyncCreationLockTaken);
                // Console.WriteLine ("DOTNET UseOpenGLCreationContext Monitor postenter " + graphicsDevice.CurrentGraphicsContext + " - " + thread.ManagedThreadId + " - " + thread.Name + "--" + graphicsDevice.GetHashCode () + " " + asyncCreationLockTaken);            
                if (graphicsDevice.CurrentGraphicsContext == IntPtr.Zero)
                {
                    needUnbindContext = true;
                }
                //                useDeviceCreationContext = true;
                gd = graphicsDevice;
                CommandList = graphicsDevice.InternalMainCommandList;
                
                graphicsDevice.MainGraphicsContext.MakeCurrent();
                // Console.WriteLine ("DOTNET UseOpenGLCreationContext AAA " + graphicsDevice.CurrentGraphicsContext +" - " + thread.ManagedThreadId + " - " + thread.Name);            
            }
#else

            GL = graphicsDevice.GL;
            if (graphicsDevice.CurrentGraphicsContext == IntPtr.Zero)
            {
                needUnbindContext = true;
                useDeviceCreationContext = true;

                // Lock, since there is only one deviceCreationContext.
                // TODO: Support multiple deviceCreationContext (TLS creation of context was crashing, need to investigate why)
                asyncCreationLockObject = graphicsDevice.asyncCreationLockObject;
                Monitor.Enter(graphicsDevice.asyncCreationLockObject, ref asyncCreationLockTaken);

                // Bind the context
                deviceCreationContext = graphicsDevice.deviceCreationContext;
                deviceCreationContext.MakeCurrent();
            }
            else
            {
                // TODO Hardcoded to the fact it uses only one command list, this should be fixed
                CommandList = graphicsDevice.InternalMainCommandList;
            }


// // Console.WriteLine ("DOTNET UseOpenGLCreationContext A1 " + graphicsDevice.CurrentGraphicsContext + " " + Environment.StackTrace);            
//             GL = graphicsDevice.GL;
// //            Console.WriteLine ("Graphics context " + __makeref (graphicsDevice).GetHashCode () + " " + graphicsDevice.CurrentGraphicsContext + " " + Graphics.SDL.Window.SDL.GetHashCode ()/* + " " + GL.GetError ()*/);
//             if (graphicsDevice.CurrentGraphicsContext == IntPtr.Zero)
//             {
// // Console.WriteLine ("DOTNET UseOpenGLCreationContext B " + graphicsDevice.CurrentGraphicsContext);            
// //                 if (thread.ManagedThreadId ==3)
// //                 {
// // Console.WriteLine ("DOTNET UseOpenGLCreationContext B1 " + graphicsDevice.CurrentGraphicsContext);        
// // graphicsDevice.MainGraphicsContext.MakeCurrent();
// //                 }
// //                 else
//                 {
//                 needUnbindContext = true;
// //                 useDeviceCreationContext = true;
// 
//                 // Lock, since there is only one deviceCreationContext.
//                 // TODO: Support multiple deviceCreationContext (TLS creation of context was crashing, need to investigate why)
//                 asyncCreationLockObject = graphicsDevice.asyncCreationLockObject;
//                 Monitor.Enter(graphicsDevice.asyncCreationLockObject, ref asyncCreationLockTaken);
// 
//                 // Bind the context
// // Console.WriteLine ("DOTNET UseOpenGLCreationContext AA " + graphicsDevice.CurrentGraphicsContext +" - " + thread.ManagedThreadId + " - " + thread.Name);            
//                 graphicsDevice.MainGraphicsContext.MakeCurrent();
// // Console.WriteLine ("DOTNET UseOpenGLCreationContext AB " + graphicsDevice.CurrentGraphicsContext +" - " + thread.ManagedThreadId + " - " + thread.Name);            
// 
// graphicsDevice.MainGraphicsContext.Clear();
// // Console.WriteLine ("DOTNET UseOpenGLCreationContext AC " + graphicsDevice.CurrentGraphicsContext +" - " + thread.ManagedThreadId + " - " + thread.Name);            
// 
//                 deviceCreationContext = graphicsDevice.deviceCreationContext;
// // Console.WriteLine ("DOTNET UseOpenGLCreationContext AD " + graphicsDevice.CurrentGraphicsContext +" - " + thread.ManagedThreadId + " - " + thread.Name);            
//                 
// //                 deviceCreationContext.Clear();
//                 deviceCreationContext.MakeCurrent();
// // Console.WriteLine ("DOTNET UseOpenGLCreationContext AE " + graphicsDevice.CurrentGraphicsContext +" - " + thread.ManagedThreadId + " - " + thread.Name);            
//                 
// // Console.WriteLine ("DOTNET UseOpenGLCreationContext C " + graphicsDevice.deviceCreationContext);            
//                 }
//             }
//             else
//             {
//                 // TODO Hardcoded to the fact it uses only one command list, this should be fixed
//                 CommandList = graphicsDevice.InternalMainCommandList;
// // Console.WriteLine ("DOTNET UseOpenGLCreationContext D " + graphicsDevice.CurrentGraphicsContext);            
//             }
#endif            
        }

        public void Dispose()
        {
 #if STRIDE_GRAPHICS_API_OPENGLES           
            try
            {
                // Console.WriteLine ("DOTNET UseOpenGLCreationContext Dispose A " + gd?.CurrentGraphicsContext + " " + Environment.StackTrace);            
                if (needUnbindContext)
                {
                    var GL = gd?.GL;
                    // Console.WriteLine ("DOTNET UseOpenGLCreationContext Dispose B " + gd?.CurrentGraphicsContext + " - " + GL?.GetError ());
                    GL?.Flush();
                    
                    // Console.WriteLine ("DOTNET UseOpenGLCreationContext Dispose B1 " + gd?.CurrentGraphicsContext + " - " + GL?.GetError ());
                    var fenceId = GL.FenceSync( SyncCondition.SyncGpuCommandsComplete, SyncBehaviorFlags.None );
                    while(true)
                    {
                        // Console.WriteLine ("DOTNET UseOpenGLCreationContext Dispose B2 " + gd?.CurrentGraphicsContext + " - " + GL?.GetError () + " -- " + fenceId);
                        SyncStatus result = (SyncStatus) GL.ClientWaitSync(fenceId, SyncObjectMask.SyncFlushCommandsBit, 5000000000); //5 Second timeout
                        // Console.WriteLine ("DOTNET UseOpenGLCreationContext Dispose B3 " + gd?.CurrentGraphicsContext + " - " + GL?.GetError () + " -- " + fenceId + " - " + result);
                        if(result != SyncStatus.TimeoutExpired) break; //we ignore timeouts and wait until all OpenGL commands are processed!
                    }
                       
                   // Restore graphics context
                   //                     GraphicsDevice.UnbindGraphicsContext(deviceCreationContext);
                   // Console.WriteLine ("DOTNET UseOpenGLCreationContext Dispose C " + gd?.CurrentGraphicsContext);
                   GraphicsDevice.UnbindGraphicsContext(gd?.MainGraphicsContext);
                   // Console.WriteLine ("DOTNET UseOpenGLCreationContext Dispose D " + gd?.CurrentGraphicsContext);                                       
                }
            }
            finally
            {
                // Unlock
                // Console.WriteLine ("DOTNET UseOpenGLCreationContext Dispose E " + gd?.CurrentGraphicsContext + " - " + GL?.GetError ());
                
                if (asyncCreationLockTaken)
                {
                    // Console.WriteLine ("DOTNET UseOpenGLCreationContext Dispose F " + gd?.CurrentGraphicsContext);
                    
                    Monitor.Exit(asyncCreationLockObject);
                    asyncCreationLockTaken = false;
                }
                // Console.WriteLine ("DOTNET UseOpenGLCreationContext Dispose G " + gd?.CurrentGraphicsContext);
                                
            }
#else
            try
            {
                if (needUnbindContext)
                {
                    GL.Flush();

                    // Restore graphics context
                    GraphicsDevice.UnbindGraphicsContext(deviceCreationContext);
                }
            }
            finally
            {
                // Unlock
                if (asyncCreationLockTaken)
                {
                    Monitor.Exit(asyncCreationLockObject);
                }
            }
#endif        
        }
    }
}
#endif
