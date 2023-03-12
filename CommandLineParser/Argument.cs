namespace CommandLineParser
{
    /// <summary>
    /// Property attribute for defining command arguments.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class Argument : Attribute
    {
        /// <param name="order">Order of the argument in the command.</param>
        public Argument(int order)
        {
            this.order = order;
        }

        //positional arguments
        readonly int order;
        public int Order { get { return order; } }

        //named arguments
        public bool IsRequired { get; set; } = false;
        public string HelpText { get; set; } = "";
        /// <summary>
        /// Minimal value of numeric argument, default is MinValue.
        /// </summary>
        public int LowerBound { get; set; } = int.MinValue;
        /// <summary>
        /// Maximal value of numeric argument, default is MaxValue.
        /// </summary>
        public int UpperBound { get; set; } = int.MaxValue;
    }
}