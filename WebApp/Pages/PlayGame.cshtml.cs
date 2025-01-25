﻿﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DTO;
using GameBrain;

namespace WebApp.Pages
{
    public class PlayGame : PageModel
    {
        public readonly GameService _gameService;

        public PlayGame(GameService gameService)
        {
            _gameService = gameService;
        }

        [BindProperty(SupportsGet = true)] public string GameName { get; set; } = default!;
        [BindProperty(SupportsGet = true)] public string? UserName { get; set; }
        [BindProperty(SupportsGet = true)] public EGamePiece? SelectedPiece { get; set; }

        [BindProperty] public int? X { get; set; }
        [BindProperty] public int? Y { get; set; }
        [BindProperty] public int? FromX { get; set; }
        [BindProperty] public int? FromY { get; set; }
        [BindProperty] public GridDirection? SelectedDirection { get; set; }
        [BindProperty] public bool ShowSelectionForm { get; set; } = true;
        [BindProperty] public bool IsSelectingPieceToMove { get; set; }
        [BindProperty] public string? MoveAction { get; set; }
        [BindProperty] public bool PlayWithAi { get; set; }
        [BindProperty] public bool AiVsAi { get; set; }
        
        public EGamePiece[,] GameBoard { get; set; } = default!;
        public EGamePiece CurrentPlayer { get; set; }
        public string Message { get; set; } = "";
        public int PiecesLeft { get; set; }
        public string? PlayerX { get; set; }
        public string? PlayerO { get; set; }
        public string? Error { get; set; }
        
        public List<GridDirection> ValidGridDirections { get; set; } = [];
        public List<string> AvailableActions { get; set; } = [];
        
        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(GameName))
            {
                return RedirectToPage("./Index");
            }
            
            if (string.IsNullOrEmpty(UserName))
            {
                return RedirectToPage("./Index", new { error = "No username provided." });
            }
            
            var (gameState, playerX, playerO) = _gameService.LoadGame(GameName);

            PlayerX = playerX;
            PlayerO = playerO;
            ShowSelectionForm = !AiVsAi && (string.IsNullOrEmpty(PlayerX) || string.IsNullOrEmpty(PlayerO));
            
            if (gameState == null)
            {
                Error = "Failed to load the game.";
                _gameService.InitializeGame(new GameConfiguration());
                UpdateGameState();
                return Page();
            }
            
            PlayWithAi = gameState.PlayWithAI;
            AiVsAi = gameState.AiVsAi;
            
            _gameService.InitializeGame(gameState.GameConfiguration, gameState);
            
            
            CurrentPlayer = _gameService.CurrentPlayer;

            // AI vs AI seadistamine
            if (AiVsAi && (string.IsNullOrEmpty(PlayerX) || string.IsNullOrEmpty(PlayerO)))
            {
                PlayerX = "AI";
                PlayerO = "AI";
                _gameService.SaveGame(GameName, PlayerX, PlayerO);
                ShowSelectionForm = false;
                UpdateGameState();
            }

            // kas kasutaja peab valima nupu?
            if (SelectedPiece.HasValue)
            {
                if (SelectedPiece == EGamePiece.X && string.IsNullOrEmpty(PlayerX))
                {
                    PlayerX = UserName;
                    if (PlayWithAi)
                    {
                        PlayerO = "AI";
                        gameState.AIPlayerPiece = EGamePiece.O;
                        _gameService.SetCurrentGameState(gameState);
                    }
                }
                else if (SelectedPiece == EGamePiece.O && string.IsNullOrEmpty(PlayerO))
                {
                    PlayerO = UserName;
                    if (PlayWithAi)
                    {
                        PlayerX = "AI";
                        gameState.AIPlayerPiece = EGamePiece.X;
                        _gameService.SetCurrentGameState(gameState);
                    }
                }

                ShowSelectionForm = false;
                _gameService.SaveGame(GameName, PlayerX, PlayerO);
                UpdateGameState();

                if (PlayWithAi && CurrentPlayer == gameState.AIPlayerPiece)
                {
                    MakeAiMove();
                    UpdateGameState();
                }
            }

            // nupu seadistamine
            if (PlayerX == UserName)
            {
                SelectedPiece = EGamePiece.X;
                ShowSelectionForm = false;
            }
            else if (PlayerO == UserName)
            {
                SelectedPiece = EGamePiece.O;
                ShowSelectionForm = false;
            }
            else if (string.IsNullOrEmpty(PlayerX) || string.IsNullOrEmpty(PlayerO))
            {
                ShowSelectionForm = true;
            }
            else
            {
                ShowSelectionForm = false;
            }
            
            var winner = _gameService.CheckWinCondition();
            if (winner != null)
            {
                Message = winner == EGamePiece.Empty ? "The game is a tie!" : $"Player {winner} wins!";
                UpdateGameState();
                return Page();
            }

            // kui AI peab kohe käigu tegema mängu algul
            if (PlayWithAi && CurrentPlayer == gameState.AIPlayerPiece)
            {
                MakeAiMove();
                UpdateGameState();
            }

            UpdateGameState();

            return Page();
        }


        public IActionResult OnPost()
        {

            if (string.IsNullOrEmpty(GameName))
            {
                return RedirectToPage("./Index");
            }

            if (string.IsNullOrEmpty(UserName))
            {
                return RedirectToPage("./Index", new { error = "No username provided." });
            }
            
            var (gameState, playerX, playerO) = _gameService.LoadGame(GameName);
            
            if (gameState == null)
            {
                Error = "Failed to load the game.";
                _gameService.InitializeGame(new GameConfiguration());
                UpdateGameState();
                return Page();
            }
            
            PlayWithAi = gameState.PlayWithAI;
            AiVsAi = gameState.AiVsAi;

            PlayerX = playerX;
            PlayerO = playerO;

            _gameService.InitializeGame(gameState.GameConfiguration, gameState);
            CurrentPlayer = _gameService.CurrentPlayer;

            // Kui vajutati "AInextMove" nuppu 
            if (Request.Form.ContainsKey("AinextMove"))
            {
                MakeAiMove();
                _gameService.SaveGame(GameName, PlayerX, PlayerO);
                UpdateGameState();
                var winner = _gameService.CheckWinCondition();
                if (winner != null)
                {
                    Message = winner == EGamePiece.Empty ? "The game is a tie!" : $"Player {winner} wins!";
                    UpdateGameState();
                }

                return Page();
            }

            if (CurrentPlayer == EGamePiece.X && UserName != PlayerX)
            {
                Error = $"It's not your turn! It is currently {PlayerX}´s turn!";
                UpdateGameState();
                return Page();
            }
            else if (CurrentPlayer == EGamePiece.O && UserName != PlayerO)
            {
                Error = $"It's not your turn! It is currently {PlayerO}´s turn!";
                UpdateGameState();
                return Page();
            }

            GameBoard = _gameService.GameBoard;

            if (PlayerX == UserName || PlayerO == UserName)
            {
                ShowSelectionForm = false;
            }
            else
            {
                ShowSelectionForm = true;
            }

            if (ShowSelectionForm && SelectedPiece.HasValue)
            {
                if (PlayWithAi)
                {
                    if (SelectedPiece == EGamePiece.X && string.IsNullOrEmpty(PlayerX))
                    {
                        PlayerX = UserName;
                        PlayerO = "AI";
                        gameState.AIPlayerPiece = EGamePiece.O;
                        _gameService.SetCurrentGameState(gameState);
                    }
                    else if (SelectedPiece == EGamePiece.O && string.IsNullOrEmpty(PlayerO))
                    {
                        PlayerO = UserName;
                        PlayerX = "AI";
                        gameState.AIPlayerPiece = EGamePiece.X;
                        _gameService.SetCurrentGameState(gameState);
                    }
                }
                else if (!AiVsAi)
                {
                    if (SelectedPiece == EGamePiece.X && string.IsNullOrEmpty(PlayerX))
                    {
                        PlayerX = UserName;
                    }
                    else if (SelectedPiece == EGamePiece.O && string.IsNullOrEmpty(PlayerO))
                    {
                        PlayerO = UserName;
                    }
                }
                
                _gameService.SaveGame(GameName, PlayerX, PlayerO);
                ShowSelectionForm = false;
                UpdateGameState();

                if (PlayWithAi && CurrentPlayer == gameState.AIPlayerPiece)
                {
                    MakeAiMove();
                    UpdateGameState();
                }

                return RedirectToPage("./PlayGame", new { GameName, UserName });

            }


            var winnerTwo = _gameService.CheckWinCondition();
            if (winnerTwo != null)
            {
                Message = winnerTwo == EGamePiece.Empty ? "The game is a tie!" : $"Player {winnerTwo} wins!";

                UpdateGameState();
                return Page();
            }

            if (!string.IsNullOrEmpty(MoveAction) && !X.HasValue && !Y.HasValue && !FromX.HasValue && !FromY.HasValue &&
                !SelectedDirection.HasValue)
            {
                UpdateGameState();
                return Page();
            }

            if (MoveAction == "PlacePiece" && X.HasValue && Y.HasValue)
            {
                if (CurrentPlayer == EGamePiece.X && UserName == PlayerX ||
                    (CurrentPlayer == EGamePiece.O && UserName == PlayerO))
                {
                    bool moveResult = _gameService.PlacePiece(X.Value, Y.Value);
                    if (!moveResult)
                    {
                        Error = "Invalid move!";
                    }
                    else
                    {
                        _gameService.SaveGame(GameName, PlayerX, PlayerO);
                        UpdateGameState();
                        if (PlayWithAi && gameState.AIPlayerPiece == CurrentPlayer)
                        {
                            MakeAiMove();
                            UpdateGameState();
                        }

                        MoveAction = null;
                        return RedirectToPage("./PlayGame", new { GameName, UserName });

                    }
                }
                else
                {
                    Error = $"It's not your turn! Current turn: {CurrentPlayer}";
                }
            }
            else if (MoveAction == "MovePiece")
            {
                if ((CurrentPlayer == EGamePiece.X && UserName == PlayerX) ||
                    (CurrentPlayer == EGamePiece.O && UserName == PlayerO))
                {
                    if (!IsSelectingPieceToMove && X.HasValue && Y.HasValue)
                    {
                        if (GameBoard[X.Value, Y.Value] == CurrentPlayer)
                        {
                            FromX = X;
                            FromY = Y;
                            IsSelectingPieceToMove = true;
                        }
                        else
                        {
                            Error = "You can only move your own pieces!";
                        }
                    }
                    else if (IsSelectingPieceToMove && X.HasValue && Y.HasValue && FromX.HasValue && FromY.HasValue)
                    {
                        bool moveResult = _gameService.MovePiece(FromX.Value, FromY.Value, X.Value, Y.Value);
                        if (!moveResult)
                        {
                            Error = "Invalid piece movement!";
                        }
                        else
                        {
                            _gameService.SaveGame(GameName, PlayerX, PlayerO);
                            UpdateGameState();

                            if (PlayWithAi && gameState.AIPlayerPiece == CurrentPlayer)
                            {
                                MakeAiMove();
                                UpdateGameState();
                            }

                            Message = "Piece moved successfully.";
                            MoveAction = null;
                            IsSelectingPieceToMove = false;
                            FromX = null;
                            FromY = null;
                            return RedirectToPage("./PlayGame", new { GameName, UserName });
                        }
                    }
                    else
                    {
                        Message = "Please select a piece to move.";
                    }
                }
                else
                {
                    Error = $"It's not your turn to move piece! Current turn: {CurrentPlayer}";
                }
            }
            else if (MoveAction == "MoveGrid" && SelectedDirection.HasValue)
            {
                if (CurrentPlayer == EGamePiece.X && UserName == PlayerX ||
                    (CurrentPlayer == EGamePiece.O && UserName == PlayerO))
                {
                    bool moveResult = _gameService.MoveGrid(SelectedDirection.Value);
                    if (!moveResult)
                    {
                        Error = "Invalid grid movement!";
                    }
                    else
                    {
                        _gameService.SaveGame(GameName, PlayerX, PlayerO);
                        UpdateGameState();
                        if (PlayWithAi && gameState.AIPlayerPiece == CurrentPlayer)
                        {
                            MakeAiMove();
                            UpdateGameState();
                        }

                        MoveAction = null;
                        IsSelectingPieceToMove = false;
                        FromX = null;
                        FromY = null;
                        return RedirectToPage("./PlayGame", new { GameName, UserName });

                    }
                } else
                {
                    Error = $"It's not your turn to move grid! Current turn: {CurrentPlayer}";
                }
            }
            else
            {
                Error = "Invalid action!";
            }

            UpdateGameState();

            winnerTwo = _gameService.CheckWinCondition();
            if (winnerTwo != null)
            {
                Message = winnerTwo == EGamePiece.Empty ? "The game is a tie!" : $"Player {winnerTwo} wins!";
                UpdateGameState();
                return Page();
            }

            return Page();
        }

        public bool IsWithinGrid(int x, int y)
        {
            return _gameService.IsWithinGrid(x, y);
        }

        private void UpdateGameState()
        {
            GameBoard = _gameService.GameBoard;
            CurrentPlayer = _gameService.CurrentPlayer;
            PiecesLeft = _gameService.NumberOfPieces -
                         (CurrentPlayer == EGamePiece.X ? _gameService.TotalPiecesX : _gameService.TotalPiecesO);

            AvailableActions.Clear();

            if (_gameService.InitialPlacementPhase)
            {
                AvailableActions.Add("PlacePiece");
            }
            else
            {
                if (PiecesLeft > 0)
                {
                    AvailableActions.Add("PlacePiece");
                }

                AvailableActions.Add("MoveGrid");
                AvailableActions.Add("MovePiece");
            }

            ValidGridDirections = _gameService.GetValidGridMovements();
        }


        private void MakeAiMove()
        {
            var aiMoveType = AIPlayer.ChooseMoveType(_gameService);

            bool moveResult = false;

            switch (aiMoveType)
            {
                case MoveType.PlacePiece:
                    var placement = AIPlayer.ChoosePlacement(_gameService);
                    if (placement != null)
                    {
                        moveResult = _gameService.PlacePiece(placement.Value.x, placement.Value.y);
                    }

                    break;
                case MoveType.MovePiece:
                    var pieceMove = AIPlayer.ChoosePieceMove(_gameService);
                    if (pieceMove != null)
                    {
                        moveResult = _gameService.MovePiece(pieceMove.Value.fromX, pieceMove.Value.fromY,
                            pieceMove.Value.toX, pieceMove.Value.toY);
                    }

                    break;
                case MoveType.MoveGrid:
                    var gridMove = AIPlayer.ChooseGridMovement(_gameService);
                    if (gridMove != null)
                    {
                        moveResult = _gameService.MoveGrid(gridMove.Value);
                    }

                    break;
            }

            if (moveResult)
            {
                _gameService.SaveGame(GameName, PlayerX, PlayerO);
                UpdateGameState();
            }
            else
            {
                Console.WriteLine("AI did not make a valid move.");
            }
        }
    }
}