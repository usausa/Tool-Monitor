namespace Tool.Monitor.Providers.Redis
{
    using System;
    using System.Linq;

    /// <summary>
    /// 
    /// </summary>
    public sealed class RedisValueProvider : IValueProvider
    {
        private readonly RedisInformation redisInformation;

        private readonly string[] keys;

        public string[] DataSources { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redisInformation"></param>
        /// <param name="keys"></param>
        public RedisValueProvider(RedisInformation redisInformation, string[] keys)
        {
            this.redisInformation = redisInformation;
            this.keys = keys;
            DataSources = keys.Select(_ => _.Substring(0, Math.Min(_.Length, 19))).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public float?[] Collect(DateTime dateTime)
        {
            return redisInformation.GetValues(dateTime, keys);
        }
    }
}
