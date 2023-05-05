namespace CommandLineParser
{
    /// <summary>
    /// Property attribute for defining command options.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class Option : Attribute
    {

        /// <param name="names"></param>
        /// <exception cref="ArgumentException"></exception>
        public Option(params string[] names)
        {
            if (names == null || names.Length <= 0) throw new ArgumentException("Option name is required.");
            this.names = names;
        }

        #region positional arguments

        readonly string[] names;
        /// <summary>
        /// Name of the option with all its synonyms.
        /// </summary>
        public string[] Names { get { return names; } }

        #endregion

        #region named arguments

        public bool IsRequired { get; set; } = false;

        public string HelpText { get; set; } = "";

        /// <summary>
        /// Minimal numer of parameters, default is 0.
        /// </summary>
        public int MinParameterCount { get; set; } = 0;

        /// <summary>
        /// Maximal numer of parameters, default is MaxValue.
        /// </summary>
        public int MaxParameterCount { get; set; } = int.MaxValue;

        /// <summary>
        /// Names of options this option is dependent on.
        /// </summary>
        public string[]? Dependencies { get; set; } = null;

        /// <summary>
        /// Names of options this option is exclusive with.
        /// </summary>
        public string[]? Exclusivities { get; set; } = null;

        /// <summary>
        /// Delimeter of option arguments, default is single space.
        /// </summary>
        public char Delimeter { get; set; } = ' ';

        #endregion
    }
}