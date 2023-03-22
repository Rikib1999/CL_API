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

        #region positional arguments

        readonly int order;
        public int Order { get { return order; } }

        #endregion

        #region named arguments

        /// <summary>
        /// Sets the optionality of this argument. Arguments will be passed by their order, so ommited arguments should come at the end of the command.
        /// </summary>
        public bool IsRequired { get; set; } = false;
        public string HelpText { get; set; } = "";

        #endregion
    }
}