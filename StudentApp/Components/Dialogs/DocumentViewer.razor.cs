using Microsoft.AspNetCore.Components;

namespace StudentApp.Components.Dialogs
{
    public partial class DocumentViewer
    {
        string? documentUrl;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            // Retrieve document content from the database (replace this with your actual code)
            _ = DocumentContent;
            // Convert the byte array to a base64 string
            string base64String = Convert.ToBase64String(DocumentContent);
            // Construct the data URL with base64-encoded content
            string mimeType = DocumentMimeType; // Change this according to your document type
            documentUrl = $"data:{mimeType};base64,{base64String}";
        }
        [Parameter] public byte[] DocumentContent { get; set; } = default!;
        [Parameter] public string DocumentMimeType { get; set; } = default!;
        private readonly string url = "${data:@DocumentMimeType;base64,@DocumentContent.ToString()}";
    }
}