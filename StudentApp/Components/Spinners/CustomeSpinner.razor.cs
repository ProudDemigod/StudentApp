using Microsoft.AspNetCore.Components;

namespace StudentApp.Components.Spinners
{
    public partial class CustomeSpinner
    {
        [Parameter]
        public bool IsVisible { get; set; }

        private string value = string.Empty;
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            ShowSpinner();
        }
        private void ShowSpinner()
        {
            if (IsVisible)
            {
                value = "block";
            }
            else
            {
                value = "none";
            }
            StateHasChanged();
        }
    }
}