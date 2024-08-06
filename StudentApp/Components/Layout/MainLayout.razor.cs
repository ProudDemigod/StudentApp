using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using StudentApp.JSServices;

namespace StudentApp.Components.Layout
{
    public partial class MainLayout
    {
        [Inject] NavigationManager NavigationManager { get; set; } = default!;
        bool sidebarExpanded = false;
        private string CurrentPage = string.Empty;
        private string Theme = "Default";
        private string ThemeValue = string.Empty;
        [Inject] StorageHelper storageHelper { get; set; } = default!;
        [Inject] IJSRuntime JSRuntime { get; set; } = default!;
        private readonly string[] Pages = { "Home", "Counter", "Weather" };
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            CurrentPage = Pages[0];
            Theme = ThemeValue;
            //PageTheme = ThemeValue;
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            ThemeValue = await GetThemeValue();
            if (ThemeValue.IsNullOrEmpty())
            {
                await SetThemeValue("Material.css");
            }
            Theme = ThemeValue;
            StateHasChanged();
        }
        private async void ChangeTheme(object value)
        {
            var Value = $"{value}.css";
            await SetThemeValue(Value);
            _ = await GetThemeValue();
        }
        public async Task SetThemeValue(string ThemeValue)
        {
            await storageHelper.SetLocalStorage("Theme", ThemeValue);
        }
        public async Task<string> GetThemeValue()
        {
            ThemeValue = await storageHelper.GetLocalStorage("Theme");
            return ThemeValue;
        }

        private void NavgateToPage(string Page, string Theme)
        {
            switch (Page)
            {
                case "Home": NavigationManager.NavigateTo($"/index/{Theme}"); break;
                case "Counter": NavigationManager.NavigateTo($"/counter/{Theme}"); break;
                case "Weather": NavigationManager.NavigateTo($"/weather/{Theme}"); break;
                default: NavigationManager.NavigateTo("/"); break;
            }
            CurrentPage = Page;
        }
    }
}