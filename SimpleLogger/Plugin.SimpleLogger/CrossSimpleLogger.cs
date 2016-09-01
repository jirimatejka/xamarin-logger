using Plugin.SimpleLogger.Abstractions;
using System;

namespace Plugin.SimpleLogger
{
  /// <summary>
  /// Cross platform SimpleLogger implemenations
  /// </summary>
  public class CrossSimpleLogger
  {       
    static Lazy<ISimpleLogger> Implementation = new Lazy<ISimpleLogger>(() => CreateSimpleLogger(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

    /// <summary>
    /// Logged instance
    /// </summary>
    public static ISimpleLogger Current
    {
      get
      {
        var ret = Implementation.Value;
        if (ret == null)
        {
          throw NotImplementedInReferenceAssembly();
        }
        return ret;
      }
    }

    static ISimpleLogger CreateSimpleLogger()
    {
#if PORTABLE
        return null;
#else
        return new SimpleLoggerImplementation();
#endif
    }

    internal static Exception NotImplementedInReferenceAssembly()
    {
      return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
    }
  }
}
