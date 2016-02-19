using System;
using GoG.Infrastructure.Engine;
using Microsoft.Practices.Prism.StoreApps;

namespace GoG.Board
{
    public class PieceStateViewModel : ViewModel
    {
        public PieceStateViewModel(string position, string sequence, GoColor? color,
            bool isHint, bool isNewPiece, bool isNewCapture)
        {
            _position = position;
            _sequence = sequence;
            _color = color;
            _isHint = isHint;
            _isNewPiece = isNewPiece;
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

        bool _isNewPiece;
        public bool IsNewPiece
        {
            get { return _isNewPiece; }
            set { SetProperty(ref _isNewPiece, value); }
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
