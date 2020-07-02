using System.Linq;
using System.Reflection;

namespace Books.Api.Utils
{
    /// <summary>
    /// Simplify and wraps Reflection calls.
    /// </summary>
    public class ReflectionUtils
    {
        /// <summary>
        /// Informational Version of the assembly containing the specified type.
        /// </summary>
        public static string GetAssemblyVersion<T>()
        {
            var containingAssembly = typeof(T).GetTypeInfo().Assembly;
            
            return containingAssembly
                .GetCustomAttributes<AssemblyInformationalVersionAttribute>()
                .FirstOrDefault()?
                .InformationalVersion;
        }
    }
}