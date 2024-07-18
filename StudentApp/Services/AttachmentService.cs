using StudentApp.Models;

namespace StudentApp.Services
{
    public class AttachmentService
    {
        private readonly HttpClient _httpClient;
        public AttachmentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IEnumerable<Attachment>?> GetAttachmentsAsync()
        {
            try
            {
                var request = await _httpClient.GetAsync("api/attachments");
                if (request.IsSuccessStatusCode)
                {
                    return await request.Content.ReadFromJsonAsync<IEnumerable<Attachment>>();

                }
                else
                {
                    return Enumerable.Empty<Attachment>();
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
        public async Task AddAttachmentAsync(Attachment attachment)
        {
            try
            {
                var post = await _httpClient.PostAsJsonAsync("api/attachments", attachment);
                post.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
