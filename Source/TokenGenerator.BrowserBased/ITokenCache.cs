namespace FCSAmerica.McGruff.TokenGenerator.BrowserBased
{
    public interface ITokenCache
    {
        void SaveToCache(string stringToCache);
        string LoadFromCache();
        string GetCacheDirectory();
        string GetCacheFileName();
        void ClearCache();
    }
}