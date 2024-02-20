using MailAPI.DataContext;
using MailAPI.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System.Net.Mail;

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


        //[HttpPost]
        //public async Task<ActionResult<MailModel>> MailPost(MailModel mail) // post
        //{
        //    _context.Mails.Add(mail);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction(nameof(GetMail), new {id = mail.Id}, mail);
        //}

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

        [HttpPost]
        public async Task<ActionResult<MailModel>> MailPost(MailModel mail)
        {
            _context.Mails.Add(mail);
            await _context.SaveChangesAsync();

            var builder = new BodyBuilder();
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("william.schuppe77@ethereal.email"));
            //email.To.Add(MailboxAddress.Parse("william.schuppe77@ethereal.email"));
            email.To.Add(MailboxAddress.Parse(mail.Email ));
            email.Subject =  mail.Title;    
            email.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = mail.Content };

            //byte[] fileBytes;
            //if(System.IO.File.Exists("Attachment/Files/text.txt"))
            //{
            //    FileStream file = new FileStream("Attachment/Files/text.txt", FileMode.Open, FileAccess.Read);
            //    using (var ms = new MemoryStream())
            //    {
            //        file.CopyTo(ms);
            //        fileBytes = ms.ToArray();
            //    }
            //    builder.Attachments.Add("text.txt", fileBytes, ContentType.Parse("application/octet-stream"));
            //}

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            var port = 587;
            var attachment = new MimePart("application", "octet-stream")
            {
                FileName = Path.GetFileName(mail.FileName)
            };
            builder.Attachments.Add(attachment);

            smtp.Connect("smtp.ethereal.email", port, SecureSocketOptions.StartTls);
            smtp.Authenticate("william.schuppe77@ethereal.email", "1vy4edV8AMn34kAtfU"); // with ethereal.email
            smtp.Send(email);
            smtp.Disconnect(true);

            return CreatedAtAction(nameof(GetMail), new { id = mail.Id }, mail);
        }
    }
}
