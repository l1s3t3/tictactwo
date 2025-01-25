﻿﻿using DAL;
using DTO;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class CreateGame : PageModel
{
    private readonly IGameRepository _gameRepository;
    public readonly GameService _gameService;

    public CreateGame(IGameRepository gameRepository, GameService gameService)
    {
        _gameRepository = gameRepository;
        _gameService = gameService;
    }
    
    [BindProperty(SupportsGet = true)]
    public string? UserName { get; set; }

    [BindProperty]
    public string NewGameName { get; set; } = default!;

    [BindProperty]
    public int BoardWidth { get; set; } = 5;

    [BindProperty]
    public int BoardHeight { get; set; } = 5;

    [BindProperty]
    public int GridSize { get; set; } = 3;

    [BindProperty]
    public EGamePiece StartingPlayer { get; set; } = EGamePiece.X;

    public SelectList StartingPlayerSelectList { get; set; } = new SelectList(new[] { EGamePiece.X, EGamePiece.O });
    


    public IActionResult OnGet(string userName)
    {
        if (string.IsNullOrEmpty(UserName))
        {
            return RedirectToPage("./Index", new { error = "No username provided." });
        }

        UserName = userName;

        return Page();
    }

    public IActionResult OnPost(string userName)
    {
        if (string.IsNullOrEmpty(UserName))
        {
            return RedirectToPage("./Index", new { error = "No username provided." });
        }
        
        UserName = userName;

        if (string.IsNullOrEmpty(NewGameName))
        {
            ModelState.AddModelError("NewGameName", "Please provide a game name.");
            return Page();
        }

        if (_gameRepository.GameExists(NewGameName))
        {
            ModelState.AddModelError("NewGameName", "A game with this name already exists.");
            return Page();
        }
        
        var gameConfig = _gameService.CreateGameConfiguration(BoardWidth, BoardHeight, GridSize, StartingPlayer);
        _gameService.InitializeGame(gameConfig);
        
        string? gameMode = Request.Form["GameMode"];
        bool playWithAi = false;
        bool aiVsAi = false;

        switch (gameMode)
        {
            case "PvAI":
                playWithAi = true;
                aiVsAi = false;
                break;
            case "AIvAI":
                playWithAi = true;
                aiVsAi = true;
                break;
            default:
                playWithAi = false;
                aiVsAi = false;
                break;
        }
        
        var gameState = _gameService.GetCurrentGameState();
        gameState.PlayWithAI = playWithAi;
        gameState.AiVsAi = aiVsAi;
        _gameService.SetCurrentGameState(gameState);
        
        _gameService.SaveGame(NewGameName);
        
        return RedirectToPage("./PlayGame", new { GameName = NewGameName, UserName});
    }

}
