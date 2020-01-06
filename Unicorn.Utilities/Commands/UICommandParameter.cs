using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Unicorn.Utilities.Commands
{
        public class UICommandParameter
        {


        }
        public class UICommandParameter<T> : UICommandParameter
        {
                public T CommandID
                {
                        get;
                }

                private TaskCompletionSource<UICommandResult> _taskSource = null;
                public TaskCompletionSource<UICommandResult> TaskSource
                {
                        get => _taskSource ?? (_taskSource = new TaskCompletionSource<UICommandResult>());
                }

                public object Sender
                {
                        get;
                }

                public object Parameter
                {
                        get => CanExecuteRoutedEventArgs != null ? CanExecuteRoutedEventArgs.Parameter : ExecutedRoutedEventArgs.Parameter;
                }

                public CanExecuteRoutedEventArgs CanExecuteRoutedEventArgs
                {
                        get;
                }

                public ExecutedRoutedEventArgs ExecutedRoutedEventArgs
                {
                        get;
                }

                internal UICommandParameter(T commandID, object sender, CanExecuteRoutedEventArgs canExecuteRoutedEventArgs, ExecutedRoutedEventArgs executedRoutedEventArgs)
                {
                        CommandID = commandID;
                        Sender = sender;
                        CanExecuteRoutedEventArgs = canExecuteRoutedEventArgs;
                        ExecutedRoutedEventArgs = executedRoutedEventArgs;
                }
        }
}
