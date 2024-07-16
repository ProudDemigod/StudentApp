using Microsoft.JSInterop;
using System.Threading.Tasks;
namespace StudentApp.JSServices
{
    public class StorageHelper
    {
        private readonly IJSRuntime _jsRuntime;

        public StorageHelper(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        // Method to set a value in local storage
        public async Task SetLocalStorage(string key, string value)
        {
            await _jsRuntime.InvokeVoidAsync("setLocalStorage", key, value);
        }

        // Method to get a value from local storage
        public async Task<string> GetLocalStorage(string key)
        {
            return await _jsRuntime.InvokeAsync<string>("getLocalStorage", key);
        }

        // Method to set a value in session storage
        public async Task SetSessionStorage(string key, string value)
        {
            await _jsRuntime.InvokeVoidAsync("setSessionStorage", key, value);
        }

        // Method to get a value from session storage
        public async Task<string> GetSessionStorage(string key)
        {
            return await _jsRuntime.InvokeAsync<string>("getSessionStorage", key);
        }
    }
}
