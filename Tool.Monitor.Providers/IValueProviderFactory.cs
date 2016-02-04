namespace Tool.Monitor.Providers
{
    using System.Linq;

    /// <summary>
    ///
    /// </summary>
    public interface IValueProviderFactory
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="parameters"></param>
        void Initialize(ILookup<string, string> parameters);

        /// <summary>
        ///
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IValueProvider Create(ILookup<string, string> parameters);
    }
}
