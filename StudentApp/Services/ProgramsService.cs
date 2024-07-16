using StudentApp.Models;

namespace StudentApp.Services
{
    public class ProgramsService
    {
        private readonly HttpClient _httpClient;

        public ProgramsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IEnumerable<Programs>?> GetProgramsAsync()
        {
            try
            {
                var request = await _httpClient.GetAsync("api/programs");
                if (request.IsSuccessStatusCode)
                {
                    return await request.Content.ReadFromJsonAsync<IEnumerable<Programs>>();

                }
                else
                {
                    return Enumerable.Empty<Programs>();
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
        public async Task AddProgramAsync(Programs programs)
        {
            try
            {
                var post = await _httpClient.PostAsJsonAsync("api/programs", programs);
                post.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

