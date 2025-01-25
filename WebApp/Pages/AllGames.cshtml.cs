using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GameBrain;

namespace WebApp.Pages
{
    public class AllGames : PageModel
    {
        private readonly GameService _gameService;

        public AllGames(GameService gameService)
        {
            _gameService = gameService;
        }

        public List<string> Games { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? UserName { get; set; }

        public void OnGet(string userName)
        {
            if (string.IsNullOrEmpty(UserName))
            {
                RedirectToPage("/Index", new { error = "No username provided." });
            }

            UserName = userName;

            Games = _gameService.GetSavedGameNames();
        }

        public IActionResult OnPostDelete(string gameName)
        {
            _gameService.DeleteGame(gameName);

            return RedirectToPage(new { UserName });
        }

        public IActionResult OnPostPlay(string gameName)
        {
            if (string.IsNullOrEmpty(UserName))
            {
                return RedirectToPage("/Index", new { error = "No username provided." });
            }

            return RedirectToPage("/PlayGame", new { GameName = gameName, UserName });
        }
    }
}