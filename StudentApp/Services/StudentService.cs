using Azure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Radzen;
using StudentApp.Models;

namespace StudentApp.Services
{
    public class StudentService
    {
        private readonly HttpClient _httpClient;

        public StudentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<Student>?> GetStudentsAsync()
        {
            try
            {
                var request = await _httpClient.GetAsync("api/students");
                if (request.IsSuccessStatusCode)
                {
                    return await request.Content.ReadFromJsonAsync<List<Student>>();

                }
                else
                {
                    throw new Exception("Student Record Not Found");
                }
            }
            catch (Exception)
            {
                throw;
            }
          
        }
        public async Task<ActionResult<Student>> GetStudentByIdAsync(int Id)
        {
            try 
            {
                var result = await _httpClient.GetAsync($"api/students/{Id}");
                if(result.IsSuccessStatusCode)
                {
                    var student = await result.Content.ReadFromJsonAsync<Student>();
                    if(student != null)
                    {
                        return student;
                    }
                    else
                    {
                        throw new Exception("Object Is Null");
                    }
                    
                }
                else
                {
                    throw new Exception("Not Found");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task AddStudentAsync(Student student)
        {
            try
            {
               var post = await _httpClient.PostAsJsonAsync("api/students", student);
               post.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task UpdateStudentAsync(int Id, Student student)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/students/{Id}", student);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error updating data: {response.ReasonPhrase}");
            }
        }
        public async Task DeleteStudentAsync(int Id)
        {
            var request = await _httpClient.DeleteAsync($"api/students/{Id}");
            if (!request.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error deleting data: {request.ReasonPhrase}");
            }
        }
    }
}
