using Microsoft.AspNetCore.Components;

namespace StudentApp.Components.Pages
{
    public partial class Counter
    {
        [Parameter]
        public string PageTheme { get; set; } = default!;
        private int currentCount = 0;

        private void IncrementCount()
        {
            currentCount++;
        }
    }
}