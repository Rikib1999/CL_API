namespace CommandLineParser
{
    /// <summary>
    /// Property attribute for defining boundary arguments. (inclusive)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class Boundaries<T> : Attribute where T : IComparable
    {
        public Boundaries(T lowerBound, T upperBound)
        {
            this.lowerBound = lowerBound;
            this.upperBound = upperBound;
        }

        #region positional arguments

        readonly T lowerBound;
        /// <summary>
        /// Minimal value of IComparable argument.
        /// </summary>
        public T LowerBound { get { return lowerBound; } }

        readonly T upperBound;
        /// <summary>
        /// Maximal value of IComparable argument.
        /// </summary>
        public T UpperBound { get { return upperBound; } }

        #endregion
    }
}