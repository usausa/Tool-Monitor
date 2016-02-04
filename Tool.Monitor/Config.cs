namespace Tool.Monitor
{
    /// <summary>
    ///
    /// </summary>
    public class Config
    {
        /// <summary>
        ///
        /// </summary>
        public RRDTool RRDTool { get; set; }

        /// <summary>
        ///
        /// </summary>
        public TemplateInformation Template { get; set; }

        /// <summary>
        ///
        /// </summary>
        public CollectorInformation[] Collectors { get; set; }
    }
}
