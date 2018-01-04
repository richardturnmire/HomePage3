using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HomePage3.Pages
{
    public class ContactModel : PageModel
    {
        [DataType(DataType.MultilineText)]
        [Required]
        public string Message { get; set; }
        
        [DataType(DataType.Text)]
        [Required]
        public string Name { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [DataType(DataType.Text)]
        [Required]
        public string Subject { get; set; }
        
        public void OnGet()
        {
            Message = "Your contact page.";
        }
    }
}
