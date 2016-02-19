using System;
using GoG.Client.Models;
using GoG.Client.ViewModels;

namespace GoG.ViewModels.Board
{
    public class PieceStateViewModel : ViewModelBase
    {
        public PieceStateViewModel(string position, string sequence, GoColor? color,
            bool isHint, bool isLastMove, bool isNewCapture)
        {
            _position = position;
            _sequence = sequence;
            _color = color;
            _isHint = isHint;
            _isLastMove = isLastMove;
            _isNewCapture = isNewCapture;
        }

        public event EventHandler MultiplePropertiesChanged;

        readonly string _position;
        public string Position 
        {
            get { return _position; }
        }

        string _sequence;
        public string Sequence
        {
            get { return _sequence; }
            set { SetProperty(ref _sequence, value); }
        }

        GoColor? _color;
        public GoColor? Color
        {
            get { return _color; }
            set { SetProperty(ref _color, value); }
        }

        bool _isHint;
        public bool IsHint
        {
            get { return _isHint; }
            set { SetProperty(ref _isHint, value); }
        }

        bool _isLastMove;
        public bool IsLastMove
        {
            get { return _isLastMove; }
            set { SetProperty(ref _isLastMove, value); }
        }

        bool _isNewCapture;
        public bool IsNewCapture
        {
            get { return _isNewCapture; }
            set { SetProperty(ref _isNewCapture, value); }
        }

        public void RaiseMultiplePropertiesChanged()
        {
            var e = MultiplePropertiesChanged;
            if (e != null)
                e(this, null);
        }
    }
}
