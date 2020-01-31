using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ChiDaram.Api.Classes.ServiceClient
{
    public abstract class BaseServiceClient
    {
        private readonly HttpClient _httpClient;
        protected BaseServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        protected static async Task<T> GetResult<T>(HttpResponseMessage httpResponseMessage)
        {
            if (!httpResponseMessage.IsSuccessStatusCode)
                throw new Exception(await GetErrorMessageAsync(httpResponseMessage));
            return await httpResponseMessage.Content.ReadAsAsync<T>();
        }
        protected static async Task<string> GetResultAsString(HttpResponseMessage httpResponseMessage)
        {
            if (!httpResponseMessage.IsSuccessStatusCode)
                throw new Exception(await GetErrorMessageAsync(httpResponseMessage));
            return await httpResponseMessage.Content.ReadAsStringAsync();
        }
        protected static async Task<string> GetErrorMessageAsync(HttpResponseMessage httpResponseMessage)
        {
            var errorMessage = await httpResponseMessage.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(errorMessage)) return errorMessage;
            return $"StatusCode: {httpResponseMessage.StatusCode}";
        }

        private bool _loggedIn;
        protected virtual Task LoginAsync(HttpClient httpClient)
        {
            return Task.CompletedTask;
        }
        private async Task LoginPrivateAsync()
        {
            if (_loggedIn) return;
            await LoginAsync(_httpClient);
            _loggedIn = true;
        }

        protected async Task<T> SendPostRequest<T>(string url, HttpContent httpContent)
        {
            await LoginPrivateAsync();
            using var httpResponseMessage = await _httpClient.PostAsync(url, httpContent);
            return await GetResult<T>(httpResponseMessage);
        }
        protected async Task<T> SendPostAsJsonRequest<T>(string url, object objectToSend)
        {
            await LoginPrivateAsync();
            using var httpResponseMessage = await _httpClient.PostAsJsonAsync(url, objectToSend);
            return await GetResult<T>(httpResponseMessage);
        }
        protected async Task<T> SendPutAsJsonRequest<T>(string url, object objectToSend)
        {
            await LoginPrivateAsync();
            using var httpResponseMessage = await _httpClient.PutAsJsonAsync(url, objectToSend);
            return await GetResult<T>(httpResponseMessage);
        }
        protected async Task<T> SendGetRequest<T>(string url)
        {
            await LoginPrivateAsync();
            using var httpResponseMessage = await _httpClient.GetAsync(url);
            return await GetResult<T>(httpResponseMessage);
        }
        protected async Task<string> SendGetRequest(string url)
        {
            await LoginPrivateAsync();
            using var httpResponseMessage = await _httpClient.GetAsync(url);
            return await GetResultAsString(httpResponseMessage);
        } protected async Task<T> SendDeleteRequest<T>(string url)
        {
            await LoginPrivateAsync();
            using var httpResponseMessage = await _httpClient.DeleteAsync(url);
            return await GetResult<T>(httpResponseMessage);
        }
    }
}
