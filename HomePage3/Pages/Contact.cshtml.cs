using HomePage3.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;

namespace HomePage3.Pages
{
    public class ContactModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IEmailConfiguration _emailConfiguration;

        public ContactModel (IConfiguration config, IEmailService emailService, IEmailConfiguration emailConfiguration)
        {
            _configuration = config;
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

        [BindProperty] private Contact Contact { get; set; }

        public void OnGet ()
        {
            // Message = "Your contact page.";
        }

        [HttpPost]
        public IActionResult OnPost ()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            EmailMessage emailMessage = new EmailMessage
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
