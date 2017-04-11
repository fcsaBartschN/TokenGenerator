using System.IO;
using System.Text;

namespace FCSAmerica.McGruff.TokenGenerator.BrowserBased
{
    public class TokenCacheObject : ITokenCache
    {
        private readonly string _cacheDirectory;
        private readonly string _cacheFileName;

        public TokenCacheObject(string user = null, string partnerId = null, string applicationName = null, string ecsServerName = null, string cacheDirectory = null)
        {
            _cacheDirectory = cacheDirectory ?? Path.GetTempPath();

            if (ecsServerName != null)
            {
                ecsServerName = ecsServerName.Split('.')[0];
            }
            var fileName = $"_FCSATOKEN_CACHE_{user}_{partnerId}_{applicationName}_{ecsServerName}";

            var invalidCharactersForFiles = Path.GetInvalidFileNameChars();
            foreach (var invalidCharacter in invalidCharactersForFiles)
            {
                fileName = fileName.Replace(invalidCharacter, '-');
            }
            _cacheFileName = _cacheDirectory + fileName;
        }

        public virtual void SaveToCache(string stringToCache)
        {
            File.WriteAllText(_cacheFileName, stringToCache);
        }

        public virtual string LoadFromCache()
        {
            if (File.Exists(_cacheFileName))
            {
                return File.ReadAllText(_cacheFileName, Encoding.ASCII);
            }
            return null;
        }

        public virtual string GetCacheDirectory()
        {
            return _cacheDirectory;
        }

        public virtual string GetCacheFileName()
        {
            return _cacheFileName;
        }

        public virtual void ClearCache()
        {
            File.Delete(_cacheFileName);
        }
    }
}