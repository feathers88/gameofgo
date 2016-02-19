using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using GoG.Client.Models;
using GoG.Client.Models.OnlineGoService.MyGames;
using GoG.Client.Services;
using GoG.ViewModels.Board;

namespace GoG.Client.ViewModels.Games
{
    public interface IGameViewModel : IViewModelBase
    {
        
    }

    public class GameViewModel : ViewModelBase, IGameViewModel
    {
        #region Data

        private readonly IGoGService _onlineGoService;

        #endregion Data

        #region Ctor and Init

        public GameViewModel(Game gameModel, IGoGService onlineGoService)
        {
            _onlineGoService = onlineGoService;
            GameModel = gameModel;

            Name = gameModel.name;

            // Build a temporary dictionary with all empty piece states initially.
            Pieces = new Dictionary<string, PieceStateViewModel>();
            for (var x = 0; x < gameModel.width; x++)
            {
                for (var y = 0; y < gameModel.width; y++)
                {
                    var position = EngineHelpers.DecodePosition(x, y, gameModel.width);
                    Pieces.Add(position, new PieceStateViewModel(position, null, null, false, false, false));
                }
            }
        }

        #endregion Ctor and Init

        #region Public Methods

        public async Task LoadDetailsAsync()
        {
            try
            {
                var details = await _onlineGoService.GetGameDetailsAsync(GameModel.id);

                // Create a board to represent ours.
                var board = new char[details.width, details.height];

                // Add initial pieces.  If NOT free handicap placement, this will
                // contain the automatically placed handicap stones.
                var coords = ConvertStringToCoords(details.gamedata.initial_state.black);
                PopulatePieces(board, 'B', coords);
                coords = ConvertStringToCoords(details.gamedata.initial_state.white);
                PopulatePieces(board, 'W', coords);

                // Start adding moves, first as handicap, then as alternating black/white moves.
                coords = ConvertStringToCoords(details.gamedata.moves);

                // If free handicap placement, those will be in gamedata.moves.
                var skipHandicapMoves = 0;
                if (details.gamedata.free_handicap_placement)
                {
                    skipHandicapMoves = details.gamedata.handicap;
                    for (int i = 0; i < details.gamedata.handicap; i++)
                        board[(int)coords[i].X, (int)coords[i].Y] = 'B';
                }

                // Make moves as if a real game.  We skip the handicap moves.
                var whoseTurn = details.gamedata.initial_player == "black" ? 'B' : 'W';
                for (int i = skipHandicapMoves; i < coords.Count; i++)
                {
                    var coord = coords[i];

                    // Place piece.
                    board[(int)coord.X, (int)coord.Y] = whoseTurn;
                    // Switch sides if handicap is done.
                    whoseTurn = whoseTurn == 'B' ? 'W' : 'B';
                    // Remove captured pieces.
                    RemoveCapturedPieces(board, whoseTurn);
                }

                // Now put it all in Pieces.
                for (int x = 0; x < board.GetLength(0); x++)
                    for (int y = 0; y < board.GetLength(1); y++)
                    {
                        var position = EngineHelpers.DecodePosition(x, y, board.GetLength(0));
                        var p = Pieces[position];
                        switch (board[x, y])
                        {
                            case 'C':
                                p.Color = null;
                                p.IsNewCapture = true;
                                break;
                            case 'B':
                                p.Color = GoColor.Black;
                                break;
                            case 'W':
                                p.Color = GoColor.White;
                                break;
                        }
                        p.RaiseMultiplePropertiesChanged();
                    }
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }

        #endregion Public Methods

        #region Properties

        #region Name
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        #endregion Name

        #region Pieces
        public Dictionary<string, PieceStateViewModel> Pieces { get; private set; }
        #endregion Pieces

        #region GameModel

        public Game GameModel { get; private set; }
        #endregion Game

        #endregion Properties

        #region Commands



        #endregion Commands

        #region Virtuals



        #endregion Virtuals

        #region Private Helpers

        private IList<Point> ConvertStringToCoords(string s)
        {
            var rval = new List<Point>();

            for (int i = 0; i < s.Length - 1; i += 2)
                rval.Add(new Point(s[i] - 'a', s[i + 1] - 'a'));

            return rval;
        }

        private void PopulatePieces(char[,] board, char goColor, IEnumerable<Point> coords)
        {
            foreach (var coord in coords)
                board[(int) coord.X, (int) coord.Y] = goColor;
        }

        private void RemoveCapturedPieces(char[,] board, char color)
        {
            for (int x = 0; x < board.GetLength(0); x++)
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    var spot = board[x, y];
                    if (spot == color)
                    {
                        var captured = GetCapturedPoints(board, x, y);
                        foreach (var capture in captured)
                            board[(int)capture.X, (int)capture.Y] = 'C';
                    }
                }
        }

        private IList<Point> GetCapturedPoints(char[,] pos, int x, int y)
        {
            var rval = new List<Point>();

            // mask will contain '-' in spots to remove.
            var mask = new char[pos.GetLength(0),pos.GetLength(1)];

            if (!HasLiberties(pos, x, y, pos[x, y], mask))
                for (int x1 = 0; x1 < pos.GetLength(0); x1++)
                    for (int y1 = 0; y1 < pos.GetLength(1); y1++)
                        if (mask[x1, y1] == '-')
                            rval.Add(new Point(x1, y1));

            return rval;
        }

        // Don't call directly, a recursive helper method for GetCapturedPoints().
        private bool HasLiberties(char[,] pos, int x, int y, char color, char[,] mask)
        {
            // If this square is empty, we have liberties.
            var spot = pos[x, y];
            if (spot == '\0')
                return true;
            if (spot != color)
                return false;

            // '-' means been here before, nothing to see...
            mask[x, y] = '-';

            // Look at the neighbors we haven't looked at before.
            if (x < pos.GetUpperBound(0) && mask[x + 1, y] != '-')
                if (HasLiberties(pos, x + 1, y, color, mask))
                    return true;
            if (x > 0 && mask[x - 1, y] != '-')
                if (HasLiberties(pos, x - 1, y, color, mask))
                    return true;
            if (y < pos.GetUpperBound(1) && mask[x, y + 1] != '-')
                if (HasLiberties(pos, x, y + 1, color, mask))
                    return true;
            if (y > 0 && mask[x, y - 1] != '-')
                if (HasLiberties(pos, x, y - 1, color, mask))
                    return true;

            return false;
        }

        public void FindGroup(char[,] pos, int x, int y, char color, char[,] mask)
        {
            // If this square is the colour expected and has not been visited before.
            var spot = pos[x, y];
            if (spot == color && mask[x, y] == '\0' && spot != '-')
            {
                // save this group member
                mask[x, y] = spot;
                // look at the neighbours
                if (x < pos.GetUpperBound(0))
                    FindGroup(pos, x + 1, y, color, mask);
                if (x > 0)
                    FindGroup(pos, x - 1, y, color, mask);
                if (y < pos.GetUpperBound(1))
                    FindGroup(pos, x, y + 1, color, mask);
                if (y > 0)
                    FindGroup(pos, x, y - 1, color, mask);
            }
            else
            {
                mask[x, y] = '-';
            }
        }
        
        #endregion Private Helpers
    }
}
