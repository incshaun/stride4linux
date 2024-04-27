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
            commands[CommandId.Save] = new AnonymousCommand<RoutedEventArgs?>(serviceProvider, (v) => OnApplicationCommand(CommandId.Save, v));
        }
        
        public static readonly StyledProperty<ICommand> SaveProperty = StyledProperty<ICommand>.Register<ApplicationCommands, ICommand>("SaveCommand");
        
//         public ICommand SaveCommand
//         {
//             get { return (ICommand)GetValue(SaveProperty); }
//             set { SetValue(SaveProperty, value); }
//         }
//         
        public static ICommand Save 
        {
            get { initialize (); return commands[CommandId.Save]; }
        }
        
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
