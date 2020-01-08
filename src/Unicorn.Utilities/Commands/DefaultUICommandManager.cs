using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Unicorn.Utilities.Commands
{
        public class DefaultUICommandManager : UICommandManager<string>
        {
                public static DefaultUICommandManager Instance { get; }

                private readonly Dictionary<string, RoutedUICommand> _cmdContainer = new Dictionary<string, RoutedUICommand>();
                static DefaultUICommandManager()
                {
                        Instance = new DefaultUICommandManager(0);
                }
                private DefaultUICommandManager(int cmdlast)
                        : base(cmdlast)
                {

                }

                protected override RoutedUICommand CreateRoutedUICommand(string cmd)
                {
                        if (string.IsNullOrEmpty(cmd))
                        {
                                throw new ArgumentNullException(nameof(cmd));
                        }

                        if (!_cmdContainer.TryGetValue(cmd, out RoutedUICommand routedUICommand))
                        {
                                routedUICommand = new RoutedUICommand(cmd, cmd, this.GetType());
                                CommandManager.RegisterClassCommandBinding(typeof(UIElement), new CommandBinding(routedUICommand, CommandExecute, CommandCanExecute));
                                _cmdContainer.Add(cmd, routedUICommand);
                        }

                        return routedUICommand;
                }

                protected override int ConverterKeyToIndex(string key)
                {
                        throw new NotImplementedException();
                }

                protected override string ConverterKeyFromName(string name)
                {
                        return name;
                }
        }
}
