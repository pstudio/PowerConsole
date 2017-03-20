namespace pstudio.PowerConsole.Host
{
    /// <summary>
    /// A IHost may provide the option to color output using a color palette.
    /// These are the color options the host provides.
    /// </summary>
    public enum OutputColorType
    {
        /// <summary>
        /// Default output color.
        /// </summary>
        Default,

        /// <summary>
        /// Accent or highlight color.
        /// </summary>
        Accented,

        /// <summary>
        /// Debug output color.
        /// </summary>
        Debug,

        /// <summary>
        /// Error output color.
        /// </summary>
        Error,

        /// <summary>
        /// Text string output color.
        /// </summary>
        Text,

        /// <summary>
        /// Number output color.
        /// </summary>
        Number,
    }

}
