using System.Globalization;
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

                if (typeof(T).IsEnum)
                {
                    bool parsed = Enum.TryParse(typeof(T), value, out object? result);
                    if (parsed) return (T)result;

                    return (T)Enum.ToObject(typeof(T), int.Parse(value));
                }

                try
                {
                    return (T)Convert.ChangeType(value, typeof(T), CultureInfo.CurrentCulture);
                }
                catch { }

                return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                throw new CommandParserException("Parameter " + value + " could not be cast to type " + typeof(T).Name);
            }
        }

        public static T CastClass<T>(string value) where T : class
        {
            try
            {
                ConstructorInfo ctor = (typeof(T).GetConstructor(new Type[] { typeof(string) }));

                T result = (T)ctor.Invoke(new object[] { value });
                if (result == null) throw new Exception();

                return result;
            }
            catch (Exception)
            {
                throw new CommandParserException("Parameter " + value + " could not be cast to type " + typeof(T).Name);
            }
        }
    }
}