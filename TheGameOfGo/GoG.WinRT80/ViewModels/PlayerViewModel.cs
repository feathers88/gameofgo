using System;
using GoG.Infrastructure.Engine;
using Microsoft.Practices.Prism.StoreApps;

namespace GoG.WinRT80.ViewModels
{
    public class PlayerViewModel : ViewModel
    {
        public PlayerViewModel(GoPlayer p, GoColor color)
        {
            _color = color;
            _name = p.Name;
            _playerType = p.PlayerType;
            _level = p.Level;
            _score = p.Score;
        }

        #region MoveCount
        private int _moveCount;
        public int MoveCount
        {
            get { return _moveCount; }
            set { _moveCount = value; OnPropertyChanged("MoveCount"); }
        }
        #endregion MoveCount

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private PlayerType _playerType;
        public PlayerType PlayerType
        {
            get { return _playerType; }
            set { SetProperty(ref _playerType, value); }
        }

        private int _level;
        public int Level
        {
            get { return _level; }
            set { SetProperty(ref _level, value); }
        }

        private GoColor _color;
        public GoColor Color
        {
            get { return _color; }
            set { SetProperty(ref _color, value); }
        }

        private decimal _score = 0;
        [Obsolete("Score is showed through prisoners")]
        public decimal Score
        {
            get { return _score; }
            set { SetProperty(ref _score, value); }
        }

        private int _prisoners = 0;
        public int Prisoners
        {
            get { return _prisoners; }
            set { SetProperty(ref _prisoners, value); }
        }


    }
}
