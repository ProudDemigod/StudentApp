using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentApp.Models;

namespace StudentApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgramsController : ControllerBase
    {
        private readonly StudentManagementContext _managementContext;
        public ProgramsController(StudentManagementContext managementContext)
        {
            _managementContext = managementContext;
        }
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            if (_managementContext.Programs == null)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var request = await _managementContext.Programs.ToListAsync();
                return Ok(request);
            }
        }
        [HttpPost]
        public async Task<IActionResult> PostAsync(Programs programs)
        {
            if (_managementContext.Programs == null)
            {
                return Problem("Entity set '_managementContext.Programs'  is null.");
            }
            _ = _managementContext.Programs.Add(programs);
            try
            {
                _ = await _managementContext.SaveChangesAsync();
                return Created();
            }
            catch (DbUpdateException)
            {
                if (PermitRequestExists(programs.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

        }
        private bool PermitRequestExists(int id)
        {
            return (_managementContext.Programs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

