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
using System.Windows.Input;

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
              commands[cid] = new AnonymousCommand<RoutedEventArgs?>(serviceProvider, (v) => OnApplicationCommand(cid, v));
            }
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
        
        private void OnApplicationCommand (CommandId name, RoutedEventArgs? v)
        {
            if (executedTable.Keys.Contains (name))
            {
                Console.WriteLine (name + " triggered");
                executedTable[name](v);
            }
        }
        
        private static Dictionary<CommandId, Action<RoutedEventArgs?>> executedTable = new Dictionary<CommandId, Action<RoutedEventArgs?>> ();
        public static void AddCommandBinding (ICommand c, Action<RoutedEventArgs?> executed, Action<RoutedEventArgs?> canExecute)
        {
            foreach (KeyValuePair <CommandId, ICommand> entry in commands)
            {
                if (entry.Value == c)
                {
                    executedTable[entry.Key] = executed;
                }
            }
        }
    }
}
