using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using StudentApp.Models;

namespace StudentApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentsController : ControllerBase
    {
        private readonly StudentManagementContext _managementContext;
        public AttachmentsController(StudentManagementContext studentManagementContext)
        {
            _managementContext = studentManagementContext;
        }
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            if (_managementContext.Attachments == null)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var request = await _managementContext.Attachments.ToListAsync();
                return Ok(request);
            }
        }
        [HttpGet("{Id}")]
        public async Task<ActionResult<Attachment>> GetByIdAsync(int Id)
        {
            if (_managementContext.Attachments == null)
            {
                return NotFound();
            }
            var attachment = await _managementContext.Attachments.FindAsync(Id);

            if (attachment == null)
            {
                return NotFound();
            }

            return attachment;
        }
        [HttpPost]
        public async Task<IActionResult> PostAsync(Attachment attachment)
        {
            if (_managementContext.Attachments == null)
            {
                return Problem("Entity set '_managementContext.Attachment'  is null.");
            }
            _ = _managementContext.Attachments.Add(attachment);
            try
            {
                _ = await _managementContext.SaveChangesAsync();
                return Created();
            }
            catch (DbUpdateException)
            {
                if (AttachmentExists(attachment.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

        }
        public async Task<IActionResult> UpdateAsync(int id, Attachment attachment)
        {
            if (id != attachment.Id)
            {
                return BadRequest();
            }
            _managementContext.Entry(attachment).State = EntityState.Modified;
            try
            {
                _ = await _managementContext.SaveChangesAsync();
               // await _hubContext.Clients.All.SendAsync("ReceiveStudentData", student);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AttachmentExists(id))
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
        private bool AttachmentExists(int id)
        {
            return (_managementContext.Attachments?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
