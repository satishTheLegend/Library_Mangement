using System;

namespace EDFSCore
{
    /// <summary>
    /// Cross EDFSCore
    /// </summary>
    public static class CrossEDFSCore
    {
        static Lazy<IEDFSCore> implementation = new Lazy<IEDFSCore>(() => CreateEDFSCore(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Gets if the plugin is supported on the current platform.
        /// </summary>
        public static bool IsSupported => implementation.Value == null ? false : true;

        /// <summary>
        /// Current plugin implementation to use
        /// </summary>
        public static IEDFSCore Current
        {
            get
            {
                IEDFSCore ret = implementation.Value;
                if (ret == null)
                {
                    throw NotImplementedInReferenceAssembly();
                }
                return ret;
            }
        }

        static IEDFSCore CreateEDFSCore()
        {
#if NETSTANDARD|| XAMARIN_ANDROID || XAMARIN_iOS
            return null;
#else
            return new EDFSFunctionManager();
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly() =>
            new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");

    }
}
