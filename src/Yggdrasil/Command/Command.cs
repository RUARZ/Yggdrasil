using System;
using System.Windows.Input;

namespace Yggdrasil.Command
{
    /// <summary>
    /// Implementation of the <see cref="ICommand"/> interface.
    /// </summary>
    public class Command : ICommand
    {
        #region Private Fields

        private readonly Action<object> _executeAction;
        private readonly Func<object, bool> _canExecuteFunc;

        #endregion

        #region Constructor


        /// <summary>
        /// Creates a new instance of <see cref="Command"/> and sets the execute <see cref="Action"/> which should be executed.
        /// </summary>
        /// <param name="executeAction">The <see cref="Action"/> which should be executed for this command.</param>
        public Command(Action<object> executeAction)
            : this(executeAction, null)
        {

        }

        /// <summary>
        /// Creates a new instance of <see cref="Command"/> and sets the execute <see cref="Action"/> which should be executed.
        /// </summary>
        /// <param name="executeAction">The <see cref="Action"/> which should be executed for this command.</param>
        /// /// <param name="canExecuteFunc"><see cref="Func{TResult}"/> which defines if the command can be executed.</param>
        public Command(Action<object> executeAction, Func<object, bool> canExecuteFunc)
        {
            _executeAction = executeAction;
            _canExecuteFunc = canExecuteFunc;
        }

        #endregion

        #region Interface Implementation

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecuteFunc?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            _executeAction.Invoke(parameter);
        }

        #endregion
    }
}
