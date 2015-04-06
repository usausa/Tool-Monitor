namespace Tool.Monitor.Providers.Redis
{
    using System.Linq;

    /// <summary>
    /// 
    /// </summary>
    public class RedisValueProviderFactory : IValueProviderFactory
    {
        private RedisInformation redisInformation;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public void Initialize(ILookup<string, string> parameters)
        {
            redisInformation = new RedisInformation(parameters["Client"].First(), parameters["Option"].First());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IValueProvider Create(ILookup<string, string> parameters)
        {
            return new RedisValueProvider(redisInformation, parameters["Key"].ToArray());
        }
    }
}
