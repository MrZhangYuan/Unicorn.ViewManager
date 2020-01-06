using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Unicorn.Utilities.Commands
{
        public abstract class UICommandManager<T>
        {
                private readonly RoutedUICommand[] _internalCommands;
                public UICommandManager(int captcy)
                {
                        _internalCommands = new RoutedUICommand[captcy];
                }

                public RoutedUICommand this[T cmd]
                {
                        get
                        {
                                return this.CreateRoutedUICommand(cmd);
                        }
                }

                protected virtual RoutedUICommand CreateRoutedUICommand(T cmd)
                {
                        int index = this.ConverterKeyToIndex(cmd);
                        if (this._internalCommands[index] == null)
                        {
                                lock (this._internalCommands.SyncRoot)
                                {
                                        if (this._internalCommands[index] == null)
                                        {
                                                var command = new RoutedUICommand(cmd.ToString(), cmd.ToString(), this.GetType());
                                                CommandManager.RegisterClassCommandBinding(typeof(UIElement), new CommandBinding(command, CommandExecute, CommandCanExecute));
                                                this._internalCommands[index] = command;
                                        }
                                }
                        }

                        return this._internalCommands[index];
                }

                protected abstract int ConverterKeyToIndex(T key);

                protected abstract T ConverterKeyFromName(string name);

                public Func<T, UICommandParameter<T>, bool> CommandCanExecuteAction
                {
                        get;
                        set;
                }

                public Action<T, UICommandParameter<T>> CommandExecuteAction
                {
                        get;
                        set;
                }

                protected void CommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
                {
                        e.Handled = true;

                        T key = this.ConverterKeyFromName(((RoutedCommand)e.Command).Name);

                        e.CanExecute = this.CommandCanExecuteAction == null
                                || this.CommandCanExecuteAction.Invoke(key, new UICommandParameter<T>(key, sender, e, null));
                }

                protected void CommandExecute(object sender, ExecutedRoutedEventArgs e)
                {
                        e.Handled = true;

                        T key = this.ConverterKeyFromName(((RoutedCommand)e.Command).Name);

                        this.CommandExecuteAction?.Invoke(key, new UICommandParameter<T>(key, sender, null, e));
                }
        }

}
