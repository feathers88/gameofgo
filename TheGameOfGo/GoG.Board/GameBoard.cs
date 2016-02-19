using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Windows.UI.Xaml.Media.Animation;
using GoG.Infrastructure;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using GoG.Board.Extensions;
using System.Text;
using GoG.Infrastructure.Engine;

namespace GoG.Board
{
    [TemplatePart(Name = "PART_GameCanvas", Type = typeof(Canvas))]
    public sealed class GameBoard : Control
    {
        private Grid _topGrid;
        private Storyboard _displayMessageStoryboard;
        private Storyboard _hideMessageStoryboard;
        private Grid _messageDisplay;
        private Grid _gridContainer;
        // The UI elements, indexed by their position, like "A19".
        private Dictionary<string, GamePiece> _pieces;
        private Border _gameBorder;
        private double _circleMargin;

        public GameBoard()
        {
            DefaultStyleKey = typeof(GameBoard);
            Loaded += GameBoard_Loaded;
        }

        //// Changes final size to a square
        //protected override Size ArrangeOverride(Size availableSize)
        //{
        //    // Note that it is necessary to call the base.
        //    var s = new Size();
        //    s.Height = s.Width = Math.Min(availableSize.Width, availableSize.Height);
        //    s = base.ArrangeOverride(s);

        //    return s;
        //}

        //// Changes final size to a square
        //protected override Size MeasureOverride(Size availableSize)
        //{
        //    // Note that it is necessary to call the base.
        //    var s = new Size();
        //    s.Height = s.Width = Math.Min(availableSize.Width, availableSize.Height);
        //    s = base.MeasureOverride(s);

        //    return s;
        //}


        private void GameBoard_Loaded(object sender, RoutedEventArgs e)
        {
            if (BoardEdgeSize != 0)
                CreateBoard();
        }

        protected override void OnApplyTemplate()
        {
            _topGrid = GetTemplateChild("TopGrid") as Grid;
            _gameBorder = GetTemplateChild("PART_GameBorder") as Border;
            _gridContainer = GetTemplateChild("PART_GridContainer") as Grid;
            _displayMessageStoryboard = _topGrid.Resources["DisplayMessageStoryboard"] as Storyboard;
            _hideMessageStoryboard = _topGrid.Resources["HideMessageStoryboard"] as Storyboard;
            _messageDisplay = GetTemplateChild("MessageDisplay") as Grid;

            base.OnApplyTemplate();
        }

        //void _gameBorder_PointerPressed(object sender, PointerRoutedEventArgs e)
        //{
        //    if(PressedCommand != null && PressedCommand.CanExecute(CurrentPointerPosition))
        //        PressedCommand.Execute(CurrentPointerPosition);
        //}

        //void _gameBorder_PointerMoved(object sender, PointerRoutedEventArgs e)
        //{
        //    Point mousePos = GetMouseHoverPoint(e.Pointer, _gameCanvas);
        //    var boardPoint = ConvertScreenPointToBoardPoint(mousePos);
        //    string gameCoordinates = DecodePosition(boardPoint);
        //    CurrentPointerPosition = gameCoordinates;
        //}

        static readonly Brush LineBrush = new SolidColorBrush(Colors.Black);

        public void DisplayMessageAnimation()
        {
            if (_displayMessageStoryboard != null)
                _displayMessageStoryboard.Begin();
        }

        private void DisplayMessageStoryboardOnCompleted(object sender, object o)
        {
            //_displayMessageStoryboard.Stop();
        }

        public void HideMessageAnimation()
        {
            if (_hideMessageStoryboard != null)
                _hideMessageStoryboard.Begin();
        }

        /// <summary>
        /// Adds rows and columns to the grid based on the BoardEdgeSize property, 
        /// and then populates the grid with GamePiece instances.
        /// </summary>
        private void CreateBoard()
        {
            //return;

            var edgesize = BoardEdgeSize;

            if (edgesize == 0 || _gameBorder == null || _gridContainer == null)
                return;

            // Handle various GameViewModel events to trigger animations and visual state changes.
            //_gameViewModel = DataContext as GameViewModel;
            //if (_gameViewModel == null) return;
            //_gameViewModel.ForcedPass += (s2, e2) =>
            //{
            //    PassStoryboard.Begin();
            //    PassStoryboard.Completed += (s3, e3) => PassStoryboard.Stop();
            //};
            //_gameViewModel.PropertyChanged += (s2, e2) =>
            //{
            //    if (e2.PropertyName.Equals("IsGameOver")) UpdateState();
            //};
            //_gameViewModel.Clock.PropertyChanged += (s2, e2) =>
            //{
            //    if (e2.PropertyName.Equals("IsShowingPauseDisplay")) UpdateState();
            //};

            _pieces = new Dictionary<string, GamePiece>();

            _pieces.Clear();
            for (int i = _gridContainer.Children.Count - 1; i >= 0; i--)
            {
                var e = _gridContainer.Children[i];
                if (!(e is Grid || e is Border))
                    _gridContainer.Children.Remove(e);
            }
            //_gridContainer.Children.Clear();
            _gridContainer.ColumnDefinitions.Clear();
            _gridContainer.RowDefinitions.Clear();

            // Add left and top column and row.
            _gridContainer.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0, GridUnitType.Auto) });
            _gridContainer.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0, GridUnitType.Auto) });

            // Add columns and rows for pieces.
            for (int i = 0; i < edgesize * 2; i++)
            {
                _gridContainer.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                _gridContainer.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            // Add right and bottom column and row.
            _gridContainer.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0, GridUnitType.Auto) });
            _gridContainer.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0, GridUnitType.Auto) });

            // Put grids in the right place.
            Grid.SetColumnSpan(_gameBorder, edgesize * 2);
            Grid.SetRowSpan(_gameBorder, edgesize * 2);
            Grid.SetColumnSpan(_messageDisplay, edgesize * 2);
            Grid.SetRowSpan(_messageDisplay, edgesize * 2);


            double lineThickness = edgesize == 9 ? 2.2 : (edgesize == 13 ? 1.8 : 1.5);

            // Add row labels and lines.
            for (int row = 0; row < edgesize; row++)
            {
                // Create row labels.
                if (ShowHeaders)
                {
                    var tb = CreateTextBlock((edgesize - row).ToString());
                    tb.HorizontalAlignment = HorizontalAlignment.Right;
                    tb.VerticalAlignment = VerticalAlignment.Center;
                    tb.SetValue(Grid.RowSpanProperty, 2);
                    tb.SetValue(Grid.RowProperty, (row * 2) + 1);
                    _gridContainer.Children.Add(tb);

                    tb = CreateTextBlock((edgesize - row).ToString());
                    tb.HorizontalAlignment = HorizontalAlignment.Left;
                    tb.VerticalAlignment = VerticalAlignment.Center;
                    tb.SetValue(Grid.ColumnProperty, (edgesize * 2) + 1);
                    tb.SetValue(Grid.RowSpanProperty, 2);
                    tb.SetValue(Grid.RowProperty, (row * 2) + 1);
                    _gridContainer.Children.Add(tb);
                }

                // Create horizontal line.
                var line = new Line
                    {
                        StrokeThickness = lineThickness - .5,
                        Stroke = LineBrush,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Center,
                        X1 = -5,
                        X2 = short.MaxValue,
                    };

                line.SetValue(Grid.ColumnProperty, 2);
                line.SetValue(Grid.ColumnSpanProperty, (edgesize * 2) - 2);
                line.SetValue(Grid.RowProperty, (row * 2) + 1);
                line.SetValue(Grid.RowSpanProperty, 2);

                _gridContainer.Children.Add(line);
            }

            // Add dots
            switch (edgesize)
            {
                case 9:
                    AddDot(2, 2);
                    AddDot(6, 2);
                    AddDot(4, 4);
                    AddDot(2, 6);
                    AddDot(6, 6);
                    break;
                case 13:
                    AddDot(3, 3);
                    AddDot(6, 3);
                    AddDot(9, 3);
                    AddDot(3, 6);
                    AddDot(6, 6);
                    AddDot(9, 6);
                    AddDot(3, 9);
                    AddDot(6, 9);
                    AddDot(9, 9);
                    break;
                case 19:
                    AddDot(3, 3);
                    AddDot(9, 3);
                    AddDot(15, 3);
                    AddDot(3, 9);
                    AddDot(9, 9);
                    AddDot(15, 9);
                    AddDot(3, 15);
                    AddDot(9, 15);
                    AddDot(15, 15);
                    break;
            }

            // Add column labels and lines.
            for (int col = 0; col < edgesize; col++)
            {
                // Create column labels.
                if (ShowHeaders)
                {
                    var letter = EngineHelpers.GetColumnLetter(col);
                    var tb = CreateTextBlock(letter);
                    tb.HorizontalAlignment = HorizontalAlignment.Center;
                    tb.SetValue(Grid.ColumnSpanProperty, 2);
                    tb.SetValue(Grid.ColumnProperty, (col * 2) + 1);
                    _gridContainer.Children.Add(tb);

                    tb = CreateTextBlock(letter);
                    tb.HorizontalAlignment = HorizontalAlignment.Center;
                    tb.SetValue(Grid.RowProperty, (edgesize * 2) + 1);
                    tb.SetValue(Grid.ColumnSpanProperty, 2);
                    tb.SetValue(Grid.ColumnProperty, (col * 2) + 1);
                    _gridContainer.Children.Add(tb);
                }

                // Create vertical line.
                var line = new Line
                {
                    StrokeThickness = lineThickness,
                    Stroke = LineBrush,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Y1 = -5,
                    Y2 = short.MaxValue,
                };

                line.SetValue(Grid.RowProperty, 2);
                line.SetValue(Grid.RowSpanProperty, (edgesize * 2) - 2);
                line.SetValue(Grid.ColumnProperty, (col * 2) + 1);
                line.SetValue(Grid.ColumnSpanProperty, 2);

                _gridContainer.Children.Add(line);
            }

            // Smaller boards allow more space between pieces.
            _circleMargin = edgesize == 9 ? 6 : (edgesize == 13 ? 4 : 3);

            for (int row = 0; row < edgesize; row++)
            {
                for (int column = 0; column < edgesize; column++)
                {
                    // Create piece (every other row/column) and set its column, row, colspan, and rowspan.
                    var gamePiece = new GamePiece(EngineHelpers.DecodePosition(column, row, edgesize), _circleMargin, null, GoColor.Black, false, false, false);
                    gamePiece.SetValue(Grid.ColumnProperty, (column * 2) + 1);
                    gamePiece.SetValue(Grid.ColumnSpanProperty, 2);
                    gamePiece.SetValue(Grid.RowProperty, (row * 2) + 1);
                    gamePiece.SetValue(Grid.RowSpanProperty, 2);

                    gamePiece.Click += GamePieceOnClick;


                    _pieces[DecodePosition(new Point(column, row))] = gamePiece;

                    _gridContainer.Children.Add(gamePiece);
                }
            }

            //_gameBorder.Visibility = Visibility.Visible;

            AttemptLinkToPieces();
        }

        private void AddDot(int col, int row)
        {
            FrameworkElement dot = null;
            switch (BoardEdgeSize)
            {
                case 19:
                    dot = new Viewbox
                    {
                        Child = new Ellipse
                        {
                            Width = 10,
                            Height = 10,
                            Margin = new Thickness(20),
                            Fill = LineBrush,
                            Stroke = LineBrush,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        },
                    };
                    break;
                case 13:
                    dot = new Ellipse
                    {
                        Width = 7,
                        Height = 7,
                        Fill = LineBrush,
                        Stroke = LineBrush,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    break;
                case 9:
                    dot = new Ellipse
                    {
                        Width = 9,
                        Height = 9,
                        Fill = LineBrush,
                        Stroke = LineBrush,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    break;
            }

            // Put in correct position.
            dot.SetValue(Grid.ColumnProperty, col * 2 + 1);
            dot.SetValue(Grid.ColumnSpanProperty, 2);
            dot.SetValue(Grid.RowProperty, row * 2 + 1);
            dot.SetValue(Grid.RowSpanProperty, 2);

            _gridContainer.Children.Add(dot);
        }

        public void FixPieces()
        {
            foreach (GamePiece p in _gridContainer.Children.Where(piece => piece is GamePiece))
                p.UpdateVisualState();
        }

        private void GamePieceOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var gamePiece = sender as GamePiece;
            if (gamePiece == null)
                return;

            if (PressedCommand != null && PressedCommand.CanExecute(gamePiece.Position))
                PressedCommand.Execute(gamePiece.Position);
        }

        private void CreatePointIndicators(Point location, double fundgeFactor = 1.0)
        {
            Shape indicator = null;
            indicator = new Ellipse() { Height = 5 * fundgeFactor, Width = 5 * fundgeFactor };
            indicator.Fill = LineBrush;

            Grid.SetColumn(indicator, (int)location.X * 2); //Set the x location
            Grid.SetColumnSpan(indicator, 2);
            indicator.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetRow(indicator, (int)location.Y * 2);   //Set the y location
            Grid.SetRowSpan(indicator, 2);
            indicator.VerticalAlignment = VerticalAlignment.Center;
            _gridContainer.Children.Add(indicator);
        }

        /// <summary>
        /// Inverts the Y axis and moves to index base A and 1 instead of 0,0.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private string DecodePosition(Point p)
        {
            return EngineHelpers.DecodePosition((int)p.X, (int)p.Y, BoardEdgeSize);
        }

        private Point GetMouseHoverPoint(Pointer elementPoint, UIElement relativeTo)
        {
            return elementPoint.GetPosition(relativeTo);
        }

        private TextBlock CreateTextBlock(string content)
        {
            var tb = new TextBlock
                {
                    FontWeight = FontWeights.Bold,
                    FontSize = 16,
                    Text = content,
                    Foreground = new SolidColorBrush(Color.FromArgb(139, 234, 234, 234)),
                    Margin = new Thickness(5, 1, 5, 1)
                };
            return tb;
        }

        #region Pieces
        public Dictionary<string, PieceStateViewModel> Pieces
        {
            get { return (Dictionary<string, PieceStateViewModel>)GetValue(PiecesProperty); }
            set { SetValue(PiecesProperty, value); }
        }
        public static readonly DependencyProperty PiecesProperty =
            DependencyProperty.Register("Pieces", typeof(Dictionary<string, PieceStateViewModel>), typeof(GameBoard), new PropertyMetadata(null, OnPiecesChanged));
        #endregion Pieces

        private static void OnPiecesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var board = sender as GameBoard;
            Debug.Assert(board != null);

            if (e.OldValue != null)
            {
                // Unwind event subscriptions on old pieces.
                var oldPieces = e.OldValue as Dictionary<string, PieceStateViewModel>;
                Debug.Assert(oldPieces != null);
                foreach (var piece in oldPieces.Values)
                    piece.MultiplePropertiesChanged -= board.piece_MultiplePropertiesChanged;
            }

            board.AttemptLinkToPieces();
        }

        private void AttemptLinkToPieces()
        {
            // If the board has been initialized, we create the linkages.  If not, we don't.
            // This method is called when Pieces changes, and after the board is loaded, so
            // the linkage will happen in one place or another, maybe both.
            if (_pieces == null || Pieces == null)
                return;

            // Avoid double subscription.
            foreach (var piece in Pieces.Values)
            {
                piece.MultiplePropertiesChanged -= piece_MultiplePropertiesChanged;
            }

            // This allows us to respond to changes in each individual piece 
            // and reflect it on the corresponding GamePiece control.
            foreach (var piece in Pieces.Values)
            {
                // Set the initial state on the control.
                piece_MultiplePropertiesChanged(piece, null);
                // Respond to future updates.
                piece.MultiplePropertiesChanged += piece_MultiplePropertiesChanged;
            }
        }

        void piece_MultiplePropertiesChanged(object sender, EventArgs args)
        {
            var piece = (PieceStateViewModel)sender;

            // Get and update the UI control.
            var ctl = _pieces[piece.Position];
            Debug.Assert(ctl != null, "ctl was null");
            ctl.Sequence = piece.Sequence;
            ctl.Color = piece.Color;
            ctl.IsHint = piece.IsHint;
            ctl.IsLastMove = piece.IsNewPiece;
            ctl.IsNewCapture = piece.IsNewCapture;

            ctl.UpdateVisualState();
        }

        #region CurrentPointerPosition
        /// <summary>
        /// Give the Pressed command the response of GO Positions
        /// </summary>
        public string CurrentPointerPosition
        {
            get { return (string)GetValue(CurrentPointerPositionProperty); }
            set { SetValue(CurrentPointerPositionProperty, value); }
        }
        public static readonly DependencyProperty CurrentPointerPositionProperty =
            DependencyProperty.Register("CurrentPointerPosition", typeof(string), typeof(GameBoard), new PropertyMetadata(null, CurrentPointerPositionChanged));

        private static void CurrentPointerPositionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {

        }
        #endregion CurrentPointerPosition

        #region PressedCommand
        /// <summary>
        /// Command for ViewModel to execute the command for pressing.
        /// </summary>
        public ICommand PressedCommand
        {
            get { return (ICommand)GetValue(PressedCommandProperty); }
            set { SetValue(PressedCommandProperty, value); }
        }
        public static readonly DependencyProperty PressedCommandProperty =
            DependencyProperty.Register("PressedCommand", typeof(ICommand), typeof(GameBoard), new PropertyMetadata(null));
        #endregion PressedCommand

        #region BoardEdgeSize
        public int BoardEdgeSize
        {
            get { return (int)GetValue(BoardEdgeSizeProperty); }
            set { SetValue(BoardEdgeSizeProperty, value); }
        }
        public static readonly DependencyProperty BoardEdgeSizeProperty =
            DependencyProperty.Register("BoardEdgeSize", typeof(int), typeof(GameBoard), new PropertyMetadata(0, BoardEdgeSizePropertyChangedCallback));

        private static void BoardEdgeSizePropertyChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((int)args.NewValue != 0)
                ((GameBoard)obj).CreateBoard();
        }

        #endregion BoardEdgeSize

        #region ShowHeaders
        public bool ShowHeaders
        {
            get { return (bool)GetValue(ShowHeadersProperty); }
            set { SetValue(ShowHeadersProperty, value); }
        }
        public static readonly DependencyProperty ShowHeadersProperty =
            DependencyProperty.Register("ShowHeaders", typeof(bool), typeof(GameBoard), new PropertyMetadata(true));
        #endregion ShowHeaders

        #region MessageText
        public string MessageText
        {
            get { return (string)GetValue(MessageTextProperty); }
            set { SetValue(MessageTextProperty, value); }
        }
        public static readonly DependencyProperty MessageTextProperty =
            DependencyProperty.Register("MessageText", typeof(string), typeof(GameBoard), new PropertyMetadata(null));
        #endregion MessageText

        #region IsBusy
        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }
        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register("IsBusy", typeof(bool), typeof(GameBoard), new PropertyMetadata(false));
        #endregion IsBusy
    }
}