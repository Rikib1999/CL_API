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

        //positional arguments
        readonly string[] names;
        /// <summary>
        /// Name of the option with all its synonyms.
        /// </summary>
        public string[] Names { get { return names; } }

        //named arguments
        public bool IsRequired { get; set; } = false;
        public string HelpText { get; set; } = "";
        public object? DefaultValue { get; set; }
        /// <summary>
        /// Minimal numer of parameters, default is 0.
        /// </summary>
        public int MinParameterCount { get; set; } = 0;
        /// <summary>
        /// Maximal numer of parameters, default is MaxValue.
        /// </summary>
        public int MaxParameterCount { get; set; } = int.MaxValue;
    }
}