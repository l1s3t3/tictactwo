using DAL;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages;

public class Home : PageModel
{
    private readonly IGameRepository _gameRepository;

    public Home(IGameRepository gameRepository, GameService gameService)
    {
        _gameRepository = gameRepository;
    }

    [BindProperty(SupportsGet = true)]
    public string? UserName { get; set; }

    public SelectList GameSelectList { get; set; } = default!;

    [BindProperty]
    public string GameName { get; set; } = default!;

    public IActionResult OnGet(string userName)
    {
        
        if (string.IsNullOrEmpty(UserName))
        {
            return RedirectToPage("./Index", new { error = "No username provided." });
        }

        UserName = userName;
        
        var userGames = _gameRepository.GetUserGames(UserName);

        GameSelectList = new SelectList(userGames);

        return Page();
    }

    public IActionResult OnPost(string userName)
    {
        if (string.IsNullOrEmpty(userName))
        {
            return RedirectToPage("./Index", new { error = "No username provided." });
        }

        UserName = userName;
        
        if (string.IsNullOrEmpty(GameName))
        {
            ModelState.AddModelError("GameName", "Please select a game.");
            ReloadGameSelectList();
            return Page();
        }

        return RedirectToPage("./PlayGame", new { GameName, UserName });
    }

    private void ReloadGameSelectList()
    {
        if (!string.IsNullOrEmpty(UserName))
        {
            var userGames = _gameRepository.GetUserGames(UserName);
            GameSelectList = new SelectList(userGames);
        }
        else
        {
            GameSelectList = new SelectList(new List<string>());
        }
    }

}