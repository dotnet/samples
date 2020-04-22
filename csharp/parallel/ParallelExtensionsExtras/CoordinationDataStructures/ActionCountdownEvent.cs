//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace System.Threading
{
    /// <summary>Runs an action when the CountdownEvent reaches zero.</summary>
    public class ActionCountdownEvent : IDisposable
    {
        private readonly CountdownEvent _event;
        private readonly Action _action;
        private readonly ExecutionContext _context;

        /// <summary>Initializes the ActionCountdownEvent.</summary>
        /// <param name="initialCount">The number of signals required to set the CountdownEvent.</param>
        /// <param name="action">The delegate to be invoked when the count reaches zero.</param>
        public ActionCountdownEvent(int initialCount, Action action)
        {
            // Validate arguments
            if (initialCount < 0) throw new ArgumentOutOfRangeException(nameof(initialCount));

            // Store the action and create the event from the initial count. If the initial count forces the
            // event to be set, run the action immediately. Otherwise, capture the current execution context
            // so we can run the action in the right context later on.
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _event = new CountdownEvent(initialCount);
            if (initialCount == 0) action();
            else _context = ExecutionContext.Capture();
        }

        /// <summary>Increments the current count by one.</summary>
        public void AddCount() => _event.AddCount();

        /// <summary>Registers a signal with the event, decrementing its count.</summary>
        public void Signal()
        {
            // If signaling the event causes it to become set
            if (_event.Signal())
            {
                // Execute the action.  If we were able to capture a context
                // at instantiation time, use that context to execute the action.
                // Otherwise, just run the action.
                if (_context != null)
                {
                    ExecutionContext.Run(_context, _ => _action(), null);
                }
                else _action();
            }
        }

        /// <summary>Releases all resources used by the current instance.</summary>
        public void Dispose() => Dispose(true);

        /// <summary>Releases all resources used by the current instance.</summary>
        /// <param name="disposing">
        /// true if called because the object is being disposed; otherwise, false.
        /// </param>
        protected void Dispose(bool disposing)
        {
            if (disposing) _event.Dispose();
        }
    }
}
