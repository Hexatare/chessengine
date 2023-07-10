using ChessEngineClassLibrary.Pieces;
using System;
using System.Diagnostics;
using System.Windows;
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
        private Piece? PieceOnTheCell;

        // Background Color for this Cell
        private Color CellBackgroundColor;

        // The Border of the Cell
        public Border CellBorder { get; set; }

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
            Index = cellIndex;
            Color = cellColor;
            IsEmpty = true;
            Location = new int[] { (cellIndex % 8), (7 - (cellIndex / 8)) };

            // Create the Border
            CellBorder = new Border();
            CellBorder.BorderBrush = Brushes.Green;
            CellBorder.BorderThickness = new Thickness(0);
            CellBorder.Name = "CellBorder";

            // Generate the Grid
            Grid = new Grid();

            // Action, when Mouse Button is pushed
            Grid.MouseLeftButtonUp += Grid_MouseLeftButtonUp;

            // Set the color of the Cell
            if ((cellColor == CellColor.White))
                CellBackgroundColor = Colors.Beige;
            else
                CellBackgroundColor = Colors.Brown;

            // Set the Background of the Cell
            Grid.Background = new SolidColorBrush(CellBackgroundColor);

            // Add the Boarder to the Grid
            Grid.Children.Add(CellBorder);

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

            piece.SetNewPosition(Index);
        }

        /// <summary>
        /// Returns the piece on this cell
        /// </summary>
        /// <returns>The piece on this cell</returns>
        public Piece? GetPiece()
        {
            return PieceOnTheCell;
        }


        /// <summary>
        /// Removes the piece from the cell 
        /// </summary>
        public void RemovePiece()
        {

            // None the Cell and set internal state
            if(PieceOnTheCell != null)
            {
                Grid.Children.Remove(PieceOnTheCell.Image);
            }

            //Grid.Children.Clear();
            IsSelected = false;
            IsEmpty = true;
            PieceOnTheCell = null;

            // Set the Border of the Cell
            CellBorder.BorderThickness = new Thickness(0);
            
            // Set the Background of the Cell
            //Grid.Background = new SolidColorBrush(CellBackgroundColor);
        }


        /// <summary>
        /// Set the Background Color of this Cell
        /// </summary>
        /// <param name="isSelected">TRUE if selected, otherwise FALS</param>
        public void SetSelected(bool isSelected, int? selection = 0)
        {   
            if(isSelected)
            {
                IsSelected = true;

                // Set the Border of the Cell
                if (selection == 0)
                    CellBorder.BorderBrush = Brushes.Yellow;
                else if (selection == 1)
                    CellBorder.BorderBrush = Brushes.Green;
                else if (selection == 2)
                    CellBorder.BorderBrush = Brushes.Red;

                CellBorder.BorderThickness = new Thickness(3);
                //Grid.Background = new SolidColorBrush(Colors.Yellow);
            }
            else
            { 
                IsSelected = false;

                // Set the Border of the Cell
                CellBorder.BorderThickness = new Thickness(0);

                // Set the Background of the Cell
                //Grid.Background = new SolidColorBrush(CellBackgroundColor);
            }
        }


        /// <summary>
        /// Eventhandler for Cell Selection
        /// </summary>
        /// <param name="e"></param>
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

        #endregion
    }
}
