using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;

using Avalonia.Threading;
using Avalonia.Interactivity;
using Stride.Core.Annotations;
using Stride.Core.Presentation.View;
using Stride.Core.Presentation.ViewModels;
using Stride.Core.Presentation.Windows;
using Stride.Core.Presentation.Behaviors;
using System.Windows.Input;
using Avalonia.Input;

namespace Stride.Core.Presentation.Commands
{
    /// <summary>
    /// A static class containing system commands to control a window.
    /// </summary>
    public class ApplicationCommands : Control
    {
        private enum CommandId : byte
        {
            Cut=0,
            Copy=1, 
            Paste=2,
            Undo=3, 
            Redo=4, 
            Delete=5,
            Find=6, 
            Replace=7,
            Help=8,
            SelectAll=9,
            New=10, 
            Open=11,
            Save=12, 
            SaveAs=13, 
            Print = 14,
            CancelPrint = 15, 
            PrintPreview = 16,
            Close = 17,
            Properties=18,
            ContextMenu=19, 
            CorrectionList=20,
            Stop=21, 
            NotACommand=22, 

            // Last 
            Last=23
        }
        
        private static ApplicationCommands instance = null;
        private static bool initialized = false;
        private static Dictionary<CommandId, ICommand> commands = new Dictionary<CommandId, ICommand> ();
        private static Dictionary<CommandId, KeyGesture> gestureTable = new Dictionary<CommandId, KeyGesture> ();
        static ApplicationCommands ()
        {
            Console.WriteLine ("Creating static ApplicationCommands");
//             var serviceProvider = new ViewModelServiceProvider(new[] { new DispatcherService(Dispatcher.UIThread) });
//             Save = new AnonymousCommand<string>(serviceProvider, OnSaveCommand);
        }
        
        private static void initialize ()
        {
            if (!initialized)
            {
                initialized = true;
                instance = new ApplicationCommands ();
            }
        }
        
        public ApplicationCommands ()
        {
            Console.WriteLine ("Creating ApplicationCommands instance");
            initialize ();
            
            var serviceProvider = new ViewModelServiceProvider(new[] { new DispatcherService(Dispatcher.UIThread) });
            foreach (CommandId cid in Enum.GetValues (typeof (CommandId)))
            {
              commands[cid] = new AnonymousCommand<object?>(serviceProvider, (v) => OnApplicationCommand(cid, v));
            }
            
            gestureTable[CommandId.Cut] = new KeyGesture(Key.X, KeyModifiers.Control);
            gestureTable[CommandId.Copy] = new KeyGesture(Key.C, KeyModifiers.Control);
            gestureTable[CommandId.Paste] = new KeyGesture(Key.V, KeyModifiers.Control);
            gestureTable[CommandId.Undo] = new KeyGesture(Key.Z, KeyModifiers.Control);
            gestureTable[CommandId.Redo] = new KeyGesture(Key.Y, KeyModifiers.Control);
            gestureTable[CommandId.Delete] = new KeyGesture(Key.Delete);
            gestureTable[CommandId.Find] = new KeyGesture(Key.F, KeyModifiers.Control);
            gestureTable[CommandId.Replace] = new KeyGesture(Key.H, KeyModifiers.Control);
            gestureTable[CommandId.Help] = new KeyGesture(Key.F1);
            gestureTable[CommandId.SelectAll] = new KeyGesture(Key.A, KeyModifiers.Control);
            gestureTable[CommandId.New] = new KeyGesture(Key.N, KeyModifiers.Control);
            gestureTable[CommandId.Open] = new KeyGesture(Key.O, KeyModifiers.Control);
            gestureTable[CommandId.Save] = new KeyGesture(Key.S, KeyModifiers.Control);
            gestureTable[CommandId.Print] = new KeyGesture(Key.P, KeyModifiers.Control);
            gestureTable[CommandId.PrintPreview] = new KeyGesture(Key.F2, KeyModifiers.Control);
            gestureTable[CommandId.Properties] = new KeyGesture(Key.F4);
            gestureTable[CommandId.ContextMenu] = new KeyGesture(Key.F10, KeyModifiers.Shift);
            gestureTable[CommandId.Stop] = new KeyGesture(Key.Escape);
        }
        
        public static ICommand Cut { get { initialize (); return commands[CommandId.Cut]; } }
        public static ICommand Copy { get { initialize (); return commands[CommandId.Copy]; } }
        public static ICommand Paste { get { initialize (); return commands[CommandId.Paste]; } }
        public static ICommand Undo { get { initialize (); return commands[CommandId.Undo]; } }
        public static ICommand Redo { get { initialize (); return commands[CommandId.Redo]; } }
        public static ICommand Delete { get { initialize (); return commands[CommandId.Delete]; } }
        public static ICommand Find { get { initialize (); return commands[CommandId.Find]; } }
        public static ICommand Replace { get { initialize (); return commands[CommandId.Replace]; } }
        public static ICommand Help { get { initialize (); return commands[CommandId.Help]; } }
        public static ICommand SelectAll { get { initialize (); return commands[CommandId.SelectAll]; } }
        public static ICommand New { get { initialize (); return commands[CommandId.New]; } }
        public static ICommand Open { get { initialize (); return commands[CommandId.Open]; } }
        public static ICommand Save { get { initialize (); return commands[CommandId.Save]; } }
        public static ICommand SaveAs { get { initialize (); return commands[CommandId.SaveAs]; } }
        public static ICommand Print { get { initialize (); return commands[CommandId.Print]; } }
        public static ICommand CancelPrint { get { initialize (); return commands[CommandId.CancelPrint]; } }
        public static ICommand PrintPreview { get { initialize (); return commands[CommandId.PrintPreview]; } }
        public static ICommand Close { get { initialize (); return commands[CommandId.Close]; } }
        public static ICommand Properties { get { initialize (); return commands[CommandId.Properties]; } }
        public static ICommand ContextMenu { get { initialize (); return commands[CommandId.ContextMenu]; } }
        public static ICommand CorrectionList { get { initialize (); return commands[CommandId.CorrectionList]; } }
        public static ICommand Stop { get { initialize (); return commands[CommandId.Stop]; } }
        
        private void OnApplicationCommand (CommandId name, object? v)
        {
                Console.WriteLine (name + " called");
            if (executedTable.Keys.Contains (name))
            {
                Console.WriteLine (name + " triggered");
                executedTable[name](v);
            }
        }
        
        private static Dictionary<CommandId, Action<object?>> executedTable = new Dictionary<CommandId, Action<object?>> ();
        public static void AddCommandBinding (ICommand c, Action<object?> executed, Action<object?> canExecute)
        {
            Console.WriteLine ("AddCommandBinding " + c + " added " + executed + " - " + canExecute);
            
            foreach (KeyValuePair <CommandId, ICommand> entry in commands)
            {
                if (entry.Value == c)
                {
                    executedTable[entry.Key] = executed;
                    
                    if (gestureTable.ContainsKey (entry.Key))
                    {
                        var binding = new KeyBinding() { Gesture = gestureTable[entry.Key], Command = c };
                        ((CommandBindingBehavior) executed.Target).AssociatedObject.KeyBindings.Add(binding);
                    }
                }
            }
        }
    }
}
