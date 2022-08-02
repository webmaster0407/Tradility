using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tradility.Common.Exceptions;
using Tradility.Common.Extensions;

namespace Tradility.Common.Services
{
    public class CacheService
    {
        public string CacheDirectory { get; set; }

        public CacheService()
        {
            CacheDirectory = "Cache";
        }

        public async Task<CacheModel<T>?> TryGetOrCreateAsync<T>(string key, Func<Task<T>> asyncFactory)
        {
            if(asyncFactory == null)
                throw new ArgumentNullException(nameof(asyncFactory));

            var cacheModel = await TryGetAsync<T>(key);
            if (cacheModel == null)
            {
                var value = await asyncFactory.Invoke();
                cacheModel = new CacheModel<T>(value);
                await TrySetAsync(key, cacheModel.Value);
            }
            
            return cacheModel;
        }

        public async Task<CacheModel<T>?> TryGetAsync<T>(string key)
        {
            try
            {
                var cacheModel = await GetAsync<T>(key);
                return cacheModel;
            }
            catch (Exception ex)
            {
                return default;
            }
        }

        public async Task<CacheModel<T>?> GetAsync<T>(string key)
        {
            if (!File.Exists(GetFileName(key)))
                throw new TException("Key not found");

            var json = await ReadFromFileAsync(key);
            var value = json.ToModel<CacheModel<T>>();
            return value;
        }

        public async Task<bool> TrySetAsync<T>(string key, T value)
        {
            try
            {
                await SetAsync(key, value);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task SetAsync<T>(string key, T value)
        {
            var fileName = GetFileName(key);
            var cacheModel = new CacheModel<T>(value);
            var json = cacheModel.ToJson();
            await StoreToFileAsync(fileName, json);
        }

        private Task<string> ReadFromFileAsync(string key) => File.ReadAllTextAsync(GetFileName(key));
        private Task StoreToFileAsync(string path, string text) 
        {
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory) && directory != null)
                Directory.CreateDirectory(directory);

            return File.WriteAllTextAsync(path, text); 
        }

        private string GetFileName(string key) => Path.Combine(CacheDirectory, key + ".json");
    }

    public class CacheModel<T>
    {
        public DateTimeOffset Time { get; set; }
        public T Value { get; set; }

        public CacheModel(T value)
        {
            Time = Now.DateTimeOffset;
            Value = value;
        }

        public CacheModel()
        {

        }
    }
}
