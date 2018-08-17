using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HomePage3.Pages
{
    public class ErrorModel : PageModel
    {
        public string RequestId { get; private set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public void OnGet()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }
}
