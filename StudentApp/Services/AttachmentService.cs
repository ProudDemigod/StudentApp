using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<Attachment> GetAttachmentById(int? Id)
        {
            try 
            {
                var request = await _httpClient.GetAsync($"api/attachments/{Id}");
                if (request.IsSuccessStatusCode)
                {
                    var result = await request.Content.ReadFromJsonAsync<Attachment>();
                    return result;
                }
                else
                {
                    throw new Exception("Not found");
                }
            }
            catch(Exception) 
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
        public async Task UpdateAttachmentAsync(int Id, Attachment attachment)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/attachments/{Id}", attachment);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error updating data: {response.ReasonPhrase}");
            }
        }
    }
}
