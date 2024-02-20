using System.ComponentModel.DataAnnotations;

namespace MailAPI.Models
{
    public class MailModel
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public string Receiver { get; set; }
        public string? Title { get; set; }
        public string Content { get; set; }
        public string? FileName { get; set; }
    }
}
