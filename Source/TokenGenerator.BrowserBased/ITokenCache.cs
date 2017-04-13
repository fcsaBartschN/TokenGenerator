namespace FCSAmerica.McGruff.TokenGenerator.BrowserBased
{
    public interface ITokenCache
    {
        void SaveToCache(string stringToCache);
        void SaveAuditInfoToCache(string stringToCache);
        string LoadFromCache();
        string LoadAuditInfoFromCache();
        string GetCacheDirectory();
        string GetCacheFileName();
        void ClearCache();
    }
}