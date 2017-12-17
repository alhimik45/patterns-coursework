namespace WebRanging
{
    class ConfigProvider : IConfigProvider
    {
        public string StoreSitesFolder { get; }

        public ConfigProvider(string storeSitesFolder)
        {
            StoreSitesFolder = storeSitesFolder;
        }
    }
}