using System;
using System.Collections.Generic;
using System.Linq;
using GoG.Database;
using GoG.Infrastructure.Engine;
using System.Diagnostics;
using GoG.ServerCommon.Logging;

namespace GoG.Repository.Engine
{
    public class DbGoGRepository : IGoGRepository
    {
        private const double SETTINGS_CACHE_LIFETIME_MINUTES = 1;

        private static DateTimeOffset? _lastSettingsRetrieval = null;
        private static string _activeMessage = null;
        private ILogger _logger;

        public DbGoGRepository(ILogger logger)
        {
            _logger = logger;
        }

        public string GetActiveMessage()
        {
            if (_lastSettingsRetrieval == null || _lastSettingsRetrieval < DateTimeOffset.Now.AddMinutes(-SETTINGS_CACHE_LIFETIME_MINUTES))
            {
                using (var ctx = new GoGEntities())
                {
                    var s = ctx.Settings.FirstOrDefault();
                    if (s != null)
                        _activeMessage = s.ActiveMessage;
                }
            }
            return _activeMessage;
        }

        public void RemoveGameIfExists(Guid gameId)
        {
            using (var ctx = new GoGEntities())
            {
                var g = ctx.Games.FirstOrDefault(game => game.GameId == gameId);
                if (g == null)
                    return;

                ctx.Games.Remove(g);
                ctx.SaveChanges();
            }
        }

        public bool GetGameExists(Guid gameId)
        {
            using (var ctx = new GoGEntities())
            {
                var g = ctx.Games.FirstOrDefault(game => game.GameId == gameId);
                return g != null;
            }
        }

        public GoGameState GetGameState(Guid gameId)
        {
            using (var ctx = new GoGEntities())
            {
                var g = ctx.Games.FirstOrDefault(game => game.GameId == gameId);
                if (g == null)
                    return null;

                var goMoveHistory = new List<GoMoveHistoryItem>();
                foreach (var h in g.Moves.OrderBy(h => h.Sequence))
                {
                    var newHistoryItem = new GoMoveHistoryItem();

                    newHistoryItem.Sequence = h.Sequence;

                    // Translate Move from db.
                    newHistoryItem.Move = new GoMove
                        {
                            Color = h.IsBlack ? GoColor.Black : GoColor.White,
                            Position = h.Position,
                            MoveType = (MoveType) h.MoveType
                        };

                    // Translate Result from db.
                    newHistoryItem.Result = new GoMoveResult(h.Captured);

                    goMoveHistory.Add(newHistoryItem);
                }

                var p1 = new GoPlayer
                                {
                                    Level = g.Player1Level,
                                    Name = g.Player1Name,
                                    PlayerType = (PlayerType)g.Player1Type,
                                    Score = g.Player1Score,
                                };
                var p2 = new GoPlayer
                {
                    Level = g.Player2Level,
                    Name = g.Player2Name,
                    PlayerType = (PlayerType)g.Player2Type,
                    Score = g.Player2Score
                };
                var rval = new GoGameState(g.Size,
                                           p1, p2,
                                           (GoGameStatus)g.Status,
                                           g.BlacksTurn ? GoColor.Black : GoColor.White,
                                           g.BlackStones, g.WhiteStones,
                                           goMoveHistory,
                                           g.WinMargin);
                return rval;
            }
        }

        public void SaveGameState(Guid gameId, GoGameState gameState)
        {
            var newMoveHistory = gameState.GoMoveHistory;
            
            using (var ctx = new GoGEntities())
            {
                // locate game by GameId
                var g = ctx.Games.FirstOrDefault(game => game.GameId == gameId);
                if (g == null)
                {
                    // add game
                    var newGame = new Game
                        {
                            BlackStones = gameState.BlackPositions,
                            BlacksTurn = gameState.WhoseTurn == GoColor.Black,
                            GameId = gameId,
                            Size = gameState.Size,
                            WhiteStones = gameState.WhitePositions,
                            Started = DateTimeOffset.Now,
                            LastMove = DateTimeOffset.Now,
                            Player1Level = (byte)gameState.Player1.Level,
                            Player2Level = (byte)gameState.Player2.Level,
                            Player1Name = gameState.Player1.Name,
                            Player2Name = gameState.Player2.Name,
                            Player1Score = gameState.Player1.Score,
                            Player2Score = gameState.Player2.Score,
                            Player1Type = (byte)gameState.Player1.PlayerType,
                            Player2Type = (byte)gameState.Player2.PlayerType,
                            Status = (byte)gameState.Status,
                            WinMargin = gameState.WinMargin
                        };
                    UpdateMoves(ctx, newGame, newMoveHistory);

                    ctx.Games.Add(newGame);
                }
                else
                {
                    // update game
                    g.BlackStones = gameState.BlackPositions;
                    g.BlacksTurn = gameState.WhoseTurn == GoColor.Black;
                    g.GameId = gameId;
                    g.Size = gameState.Size;
                    g.WhiteStones = gameState.WhitePositions;
                    g.LastMove = DateTimeOffset.Now;
                    g.Player1Level = (byte)gameState.Player1.Level;
                    g.Player2Level = (byte)gameState.Player2.Level;
                    g.Player1Name = gameState.Player1.Name;
                    g.Player2Name = gameState.Player2.Name;
                    g.Player1Score = gameState.Player1.Score;
                    g.Player2Score = gameState.Player2.Score;
                    g.Player1Type = (byte)gameState.Player1.PlayerType;
                    g.Player2Type = (byte)gameState.Player2.PlayerType;
                    g.Status = (byte)gameState.Status;
                    g.WinMargin = gameState.WinMargin;

                    UpdateMoves(ctx, g, newMoveHistory);
                }
                
                ctx.SaveChanges();
            }
        }

        // This method updates move history in the database incrementally, without a bunch of deletions.
        private void UpdateMoves(GoGEntities ctx, Game game, List<GoMoveHistoryItem> newMoveHistory)
        {
            try
            {
                if (newMoveHistory == null)
                    newMoveHistory = new List<GoMoveHistoryItem>();

                // Go through original game and update items with the same Sequence, while deleting items
                // that no longer exist.
                var dbMoves = game.Moves.Where(m => m.GameId == game.GameId).OrderBy(g => g.Sequence).ToArray();
                int max = Math.Max(dbMoves.Length, newMoveHistory.Count);
                for (int i = 0; i < max; i++)
                {
                    if (i >= dbMoves.Length)
                    {
                        // Past end of dbMoves, need to add new item.
                        game.Moves.Add(ConvertToDbMove(newMoveHistory[i], i, game.GameId));
                    }
                    else if (i >= newMoveHistory.Count)
                    {
                        // Past end of newMoveHistory, need to delete these items from db.
                        ctx.Set<Move>().Remove(dbMoves[i]);
                    }
                    else
                    {
                        // Move exists in both, update it.
                        UpdateDbMove(dbMoves[i], newMoveHistory[i]);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void UpdateDbMove(Move move, GoMoveHistoryItem goMoveHistoryItem)
        {
            // We never update moves, we only add and remove them.  So I've 
            // commented this code until I see some problem in the future. -- Chris Bordeman

            //move.Captured = goMoveHistoryItem.Result.CapturedStones;
            //move.IsBlack = goMoveHistoryItem.Move.Color == GoColor.Black;
            //move.MoveType = (byte)goMoveHistoryItem.Move.MoveType;
            //if (goMoveHistoryItem.Move.MoveType == MoveType.Normal)
            //    move.Position = goMoveHistoryItem.Move.Position;
        }

        private static Move ConvertToDbMove(GoMoveHistoryItem goMoveHistoryItem, int position, Guid gameId)
        {
            var move = new Move();
            if (goMoveHistoryItem.Move.MoveType == MoveType.Normal)
                move.Position = goMoveHistoryItem.Move.Position;
            move.GameId = gameId;
            move.Captured = goMoveHistoryItem.Result.CapturedStones;
            move.IsBlack = goMoveHistoryItem.Move.Color == GoColor.Black;
            move.MoveType = (byte) goMoveHistoryItem.Move.MoveType;
            move.Sequence = position;

            return move;
        }
    }
}
