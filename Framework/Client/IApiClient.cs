namespace Framework.Client
{
    public interface IApiClient
    {
        Task<T?> GetAsync<T>(string clientName, string actionUrl, Dictionary<string, string>? headers = null);

        Task PostAsync<T>(string url, T body);

        Task<R?> PutAsync<T, R>(string url, T body);

        Task<R?> PatchAsync<T, R>(string url, T body);

        Task DeleteAsync(string url);
    }
}
