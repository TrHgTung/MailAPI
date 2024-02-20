﻿using MailAPI.DataContext;
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
        public async Task<ActionResult<MailModel>> GetMail(int id)
        {
            if(_context.Mails == null)
            {
                return NotFound();
            }
            var mail = await _context.Mails.FindAsync(id);
            if(mail == null)
            {
                return NotFound();
            }

            return mail;
        }


        [HttpPost]
        public async Task<ActionResult<MailModel>> MailPost(MailModel mail)
        {
            _context.Mails.Add(mail);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMail), new {id = mail.Id}, mail);
        }
    }
}