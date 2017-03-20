namespace pstudio.PowerConsole.Host
{
    /// <summary>
    /// The interface any host of a <see cref="PowerConsole"/> instance must implement.
    /// The host handles the UI representation of a console.
    /// The host is used by PowerConsole to prompt user for additional details, and for outputting the final result in text format.
    /// </summary>
    public interface IHost
    {
        /// <summary>
        /// Indicates if the host supports using <see cref="OutputColorType"/>
        /// </summary>
        bool SupportsColor { get; }

        /// <summary>
        /// Utility function for marking the string with a certain <see cref="OutputColorType"/>.
        /// This is used by <see cref="PowerConsole"/> to color parts of the output.
        /// If the host does not support color the method will simply return message.
        /// </summary>
        /// <param name="message">The string that should be colored.</param>
        /// <param name="colorType">The color type.</param>
        /// <returns>The message encoded with color information useful for the host.</returns>
        string FormatColor(string message, OutputColorType colorType);

        #region Output methods

        /// <summary>
        /// Write output to the host.
        /// </summary>
        /// <param name="message"></param>
        void Write(string message);

        /// <summary>
        /// Write a debug message to the host.
        /// </summary>
        /// <param name="debugMessage"></param>
        void WriteDebug(string debugMessage);

        /// <summary>
        /// Write an error message to the host.
        /// </summary>
        /// <param name="errorMessage"></param>
        void WriteError(string errorMessage);

        #endregion
    }

}
