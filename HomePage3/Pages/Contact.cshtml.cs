using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using HomePage3.Utils;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using MimeKit;
using MimeKit.Text;

namespace HomePage3.Pages
{
    public class ContactModel : PageModel
    {
        private IConfiguration Configuration;
        private IEmailService _emailService;
        private IEmailConfiguration _emailConfiguration;

        public ContactModel(IConfiguration config,  IEmailService emailService, IEmailConfiguration emailConfiguration)
        {
            Configuration = config;
            _emailService = emailService;
            _emailConfiguration = emailConfiguration;
        }

        [DataType(DataType.MultilineText)]
        [MaxLength(2048)]
        [Required]
        public string Message { get; set; }

        [DataType(DataType.Text)]
        [Required]
        public string Name { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }

        [DataType(DataType.Text)]
        [Required]
        public string Subject { get; set; }

        [BindProperty]
        public Contact Contact { get; set; }

        public void OnGet()
        {
            // Message = "Your contact page.";
        }

        [HttpPost]
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();
            
            var emailMessage = new EmailMessage
            {
                Content = Contact.Message,
                Subject = Contact.Subject,
            };
            
            emailMessage.FromAddresses.Add(new EmailAddress()
            {
                Name = Contact.Name,
                Address = Contact.Email
            });
            
            emailMessage.ToAddresses.Add(new EmailAddress()
            {
                Name = "Message from web", 
                Address = _emailConfiguration.SmtpUsername   
            });
            
            emailMessage.CcAddresses.Add(new EmailAddress()
            {
                Name = Contact.Name,
                Address = Contact.Email
            });
            
            _emailService.Send(emailMessage);
            
            return RedirectToPage("Index");
            
        }
    }

    public class Contact
    {
        [DataType(DataType.MultilineText)]
        [MaxLength(2048)]
        [Required]
        public string Message { get; set; }

        [DataType(DataType.Text)]
        [Required]
        public string Name { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }

        [DataType(DataType.Text)]
        [Required]
        public string Subject { get; set; }

    }
}
