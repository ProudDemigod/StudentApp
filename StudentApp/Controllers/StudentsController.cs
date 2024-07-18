using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentApp.Models;

namespace StudentApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly StudentManagementContext _managementContext;
        public StudentsController(StudentManagementContext managementContext)
        {
            _managementContext = managementContext;
        }
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            if (_managementContext.Students == null)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var request = await _managementContext.Students.ToListAsync();
                return Ok(request);
            }
        }
        [HttpGet("{Id}")]
        public async Task<ActionResult<Student>> GetByIdAsync(int Id)
        {
            if (_managementContext.Students == null)
            {
                return NotFound();
            }
            var student = await _managementContext.Students.FindAsync(Id);

            if (student == null)
            {
                return NotFound();
            }

            return student;
        }
        [HttpPost]
        public async Task<IActionResult> PostAsync(Student student)
        {
            if (_managementContext.Students == null)
            {
                return Problem("Entity set '_managementContext.Students'  is null.");
            }
            _ = _managementContext.Students.Add(student);
            try
            {
                _ = await _managementContext.SaveChangesAsync();
                return Created();
            }
            catch (DbUpdateException)
            {
                if (StudentExists(student.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, Student student)
        {
            if (id != student.Id)
            {
                return BadRequest();
            }

            _managementContext.Entry(student).State = EntityState.Modified;

            try
            {
                _ = await _managementContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        private bool StudentExists(int id)
        {
            return (_managementContext.Students?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteAsync(int Id)
        {
            if (_managementContext.Students == null)
            {
                return NotFound();
            }
            var student = await _managementContext.Students.FindAsync(Id);
            if (student == null)
            {
                return NotFound();
            }

            _ = _managementContext.Students.Remove(student);
            _ = await _managementContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
