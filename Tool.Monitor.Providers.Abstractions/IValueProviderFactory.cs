namespace Tool.Monitor.Providers
{
    public interface IValueProviderFactory
    {
        void Initialize(IDictionary<string, string[]> parameters);

        IValueProvider Create(IDictionary<string, string[]> parameters);
    }
}
