using MailAPI.DataContext;
using MailAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MailAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailAPIController : ControllerBase
    {
        private readonly MailContext _context;

        public MailAPIController(MailContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MailModel>>> GetMails() //get tat ca mails
        {
            if(_context.Mails == null)
            {
                return NotFound();
            }
            
            return await _context.Mails.ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<MailModel>> GetMail(int id) // get voi mail id
        {
            if (_context.Mails == null)
            {
                return NotFound();
            }
            var mail = await _context.Mails.FindAsync(id);
            if (mail == null)
            {
                return NotFound();
            }

            return mail;
        }


        [HttpPost]
        public async Task<ActionResult<MailModel>> MailPost(MailModel mail) // post
        {
            _context.Mails.Add(mail);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMail), new {id = mail.Id}, mail);
        }

        [HttpPut] // phai nhap dung id vao swagger moi PUT duoc du lieu
        public async Task<ActionResult> MailPut(MailModel mail, int id)
        {
            if(id != mail.Id)
            {
                return BadRequest();
            }
            _context.Entry(mail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }

            catch (DbUpdateConcurrencyException)
            {
                if (!MailExist(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
           return Ok();
        }
        private bool MailExist(int id)
        {
            return (_context.Mails?.Any(m => m.Id == id)).GetValueOrDefault();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> MailDelete(MailModel mail, int id)
        {

            if(id != mail.Id)
            {
                return BadRequest();
            }
            else if(_context.Mails == null)
            {
                return NotFound();
            }

            _context.Mails.Remove(mail);

            await _context.SaveChangesAsync();

            return Ok();    
        }
    }
}
