using Client.Services.IServices;
using Microsoft.JSInterop;
using System.Text.Json;

namespace Client.Services
{

    public class LocalStorageManager(IJSRuntime jsRuntime) : ILocalStorageManager
    {
        private IJSRuntime _jsRuntime = jsRuntime;

        public async Task<T> GetItem<T>(string key)
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);

            if (json == null)
                return default;

            return JsonSerializer.Deserialize<T>(json);
        }

        public async Task SetItem<T>(string key, T value)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, JsonSerializer.Serialize(value));
        }

        public async Task RemoveItem(string key)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        }
    }
}
