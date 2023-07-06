using ChessEngineClassLibrary.Pieces;
using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ChessEngineClassLibrary
{
    /// <summary>
    /// Class Cell. Represents one Cell on the Board
    /// </summary>
    public class Cell
    {
        #region Enumerations


        // Color of the Cell 
        public enum CellColor
        {
            White,
            Black
        }

        #endregion

        #region Properties and private Members

        // Property for the index of the cell
        public int Index { get; set; }

        // Location of the piece on the board, [0] = x, [1] = y; 
        public int[] Location { get; set; }

        // Actual Color or the Cell 
        public CellColor Color { get; set; }

        // Status if cell is occupied with a piece
        public bool IsEmpty { get; set; }
      
        // Graphical Element, that is placed in the Board
        public Grid Grid { get; }

        // A public Eventdelegate for a Cell that is selected
        public event EventHandler CellSelected;

        // Status if cell is selected
        private bool IsSelected = false;

        // Chess Piece or null if cell is empty
        private Piece PieceOnTheCell;

        // Reference to the Board
        private readonly Board Board;

        // Background Color for this Cell
        private Color CellBackgroundColor;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="chessBoard"></param>
        /// <param name="cellColor"></param>
        /// <param name="cellIndex"></param>
        public Cell(Board chessBoard, CellColor cellColor, int cellIndex)
        {
            Board = chessBoard;
            Index = cellIndex;
            Color = cellColor;
            IsEmpty = true;
            Location = new int[] { (cellIndex % 8), (7 - (cellIndex / 8)) };

            // Generate the Grid
            Grid = new Grid();
            Grid.AllowDrop = true;
            Grid.Drop += Grid_Drop;

            // Action, when Mouse Button is pushed
            Grid.MouseLeftButtonDown += Grid_MouseLeftButtonDown;
            Grid.MouseLeftButtonUp += Grid_MouseLeftButtonUp;

            // Set the color of the Cell
            if ((cellColor == CellColor.White))
                CellBackgroundColor = Colors.Beige;
            else
                CellBackgroundColor = Colors.Brown;

            // Set the Background of the Cell
            Grid.Background = new SolidColorBrush(CellBackgroundColor);
        }

        #endregion

        #region Methods 

        /// <summary>
        /// Set a Piece on the board
        /// </summary>
        /// <param name="piece"></param>
        public void SetPiece(Piece piece)
        {
            PieceOnTheCell = piece;
            Grid.Children.Add(piece.Image);
            IsEmpty = false;

            //piece.SetNewPosition(this.Index);
        }

        /// <summary>
        /// Returns the piece on this cell
        /// </summary>
        /// <returns>The piece on this cell</returns>
        public Piece GetPiece()
        {
            return PieceOnTheCell;
        }


        /// <summary>
        /// Removes the piece from the cell 
        /// </summary>
        public void RemovePiece()
        {

            // Empty the Cell and set internal state
            Grid.Children.Clear();
            IsSelected = false;
            IsEmpty = true;

            // Set the Background of the Cell
            Grid.Background = new SolidColorBrush(CellBackgroundColor);
        }


        public void SetSelected(bool isSelected)
        {   
            if(isSelected)
            {
                IsSelected = true;

                // Set the Background of the Cell
                Grid.Background = new SolidColorBrush(Colors.Yellow);
            }
            else
            { 
                IsSelected = false; 

                // Set the Background of the Cell
                Grid.Background = new SolidColorBrush(CellBackgroundColor);
            }
        }


        public virtual void OnCellSelected(EventArgs e)
        {
            CellSelected?.Invoke(this, e);
        }

        /// <summary>
        /// Mouse Event, wenn the Cell is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Notify all Eventhandler on the Cell
            this.OnCellSelected(e);

        }


        /// <summary>
        /// Mouse Event for drag and drop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Debug.WriteLine("Mouse pressed, Cell-Id: " + Index + " Empty: " + IsEmpty);

            // Set a Frame around the Grid

        }

        /// <summary>
        /// Event, when a object was dropped on the cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_Drop(object sender, System.Windows.DragEventArgs e)
        {
            Debug.WriteLine("Mouse pressed, Cell-Id: " + Index + " Empty: " + IsEmpty);
            
        }

        #endregion
    }
}
