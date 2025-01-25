using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages
{
    public class Rules : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string? UserName { get; set; }

        public void OnGet(string userName)
        {
            
            if (string.IsNullOrEmpty(userName))
            {
                RedirectToPage("/Index", new { error = "No username provided." });
            }

            UserName = userName;
        }
    }
}