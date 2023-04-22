namespace CommandLineParser
{
    public static class ValueCaster<T> where T : IConvertible
    {
        public static T Cast(string value)
        {
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (Exception)
            {
                throw new CommandParserException("Argument " + value + " could not be cast to type " + typeof(T).Name);
            }
        }
    }
}