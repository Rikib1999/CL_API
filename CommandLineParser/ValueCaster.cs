using System.Reflection;

namespace CommandLineParser
{
    public static class ValueCaster
    {
        public static T Cast<T>(string value) where T : IConvertible
        {
            try
            {
                ConstructorInfo ctor = (typeof(T).GetConstructor(new Type[] { typeof(string) }));
                if (ctor != null)
                {
                    T result = (T)ctor.Invoke(new object[] { value });
                    if (result != null) return result;
                }

                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (Exception)
            {
                throw new CommandParserException("Argument " + value + " could not be cast to type " + typeof(T).Name);
            }
        }
    }
}