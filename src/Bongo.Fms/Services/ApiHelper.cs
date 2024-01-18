using Newtonsoft.Json;

namespace Bongo.Fms.Services
{
    public static class ApiHelper
    {
        public static async Task<T> ExecuteAsync<T>( Func<Task<T>> func, string? filename = null) where T: class
        {
            NetworkAccess accessType = Connectivity.Current.NetworkAccess;
            if (accessType == NetworkAccess.Internet || accessType ==  NetworkAccess.Local)
            {
                // Connection to internet is available
                try
                {
                    var result = await func();
                    if (filename != null)
                    {
                        var sprintJson = JsonConvert.SerializeObject(result);
                        string targetFile = Path.Combine(FileSystem.CacheDirectory, filename);
                        File.WriteAllText(targetFile, sprintJson);
                    }
                    return result;
                }
                catch
                {
                    // failed to get from api
                }
            }
            try
            {
                if (filename != null)
                {
                    string targetFile = Path.Combine(FileSystem.CacheDirectory, filename);
                    var sprintJson = File.ReadAllText(targetFile);
                    var sprints = JsonConvert.DeserializeObject<T>(sprintJson);
                    return sprints;
                }
            }
            catch
            {
                // failed to get from cache
            }
            return default;
        }
    }
}
