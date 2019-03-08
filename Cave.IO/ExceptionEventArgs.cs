using System;

namespace Cave.IO
{
    /// <summary>
    /// Provides <see cref="EventArgs"/> for <see cref="Exception"/>handling of background threads using an <see cref="EventHandler"/>.
    /// </summary>
    public class ExceptionEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the <see cref="Exception"/> that was encountered.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionEventArgs"/> class.
        /// </summary>
        /// <param name="ex">The <see cref="Exception"/> that was encountered.</param>
        public ExceptionEventArgs(Exception ex)
        {
            Exception = ex;
        }
    }
}
