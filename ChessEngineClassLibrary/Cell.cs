using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Brushes = System.Windows.Media.Brushes;

namespace ChessEngineClassLibrary
{
    /// <summary>
    /// Class Cell. Represents one Cell on the Board
    /// </summary>
    public class Cell
    {
        // Color of the Cell 
        public enum CellColor
        {
            White,
            Black
        }

        // Property for the index of the cell
        public int Index { get; set; }

        // Actual Color or the Cell 
        public CellColor Color { get; set; }

        // Property to hold the piece type on the cell
        public int PieceOnCell { get; set; }

        // Chess Piece or null if cell is empty
        public Piece PieceOnTheCell;

        // Status if cell is occupied with a piece
        public bool IsCellEmpty { get; set; }
      
        // Graphical Element, that is placed in the Board
        public Grid Grid { get; }

        // Status if cell is selected
        private bool IsSelected = false;

        // Reference to the Board
        private readonly Board Board;
        
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
            IsCellEmpty = true;

            // Generate the Grid
            Grid = new Grid();
            Grid.AllowDrop = true;
            Grid.Drop += Grid_Drop;

            // Action, when Mouse Button is pushed
            Grid.MouseLeftButtonDown += Grid_MouseLeftButtonDown;
            Grid.MouseLeftButtonUp += Grid_MouseLeftButtonUp;
            

            // Set the color of the Cell
            if (cellColor == CellColor.White)
            {
                Grid.Background = new SolidColorBrush(Colors.Beige);
            }
            else
            {
                Grid.Background = new SolidColorBrush(Colors.Brown);
            }             
        }

        private void Grid_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Grid has bee clicked, set the Boarder Yellow
            // Create a Border
            //var border = new Border();

            //// Set the border properties
            //border.BorderBrush = Brushes.Yellow;
            //border.BorderThickness = new Thickness(1);

            if (!IsCellEmpty)
            {
                if (!IsSelected)
                {
                    IsSelected = true;

                    // Notify the Board, a occupied Cell is selected
                    Board.SetSelectedCell(Index, IsCellEmpty);
                }
                else
                {
                    IsSelected = false;
                    Board.SetSelectedCell(-1, IsCellEmpty);
                }
            }
            else
            { 
                // Selection of a empty cell for possible movement of a Piece
                Board.SetSelectedCell(this.Index, IsCellEmpty);
            }
        }

        public void SetPiece(Piece piece) //where T : Piece
        {
            PieceOnTheCell = piece;
            Grid.Children.Add(piece.Image);
            IsCellEmpty = false;
        }

        public Piece GetPiece()
        {
            return PieceOnTheCell;
        }


        public void RemovePiece()
        {
            Grid.Children.Clear();
            IsSelected = false;
            IsCellEmpty = true;
        }



        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Debug.WriteLine("Mouse pressed, Cell-Id: " + Index + " Empty: " + IsCellEmpty);

            // Set a Frame around the Grid

        }

        private void Grid_Drop(object sender, System.Windows.DragEventArgs e)
        {
            Debug.WriteLine("Mouse pressed, Cell-Id: " + Index + " Empty: " + IsCellEmpty);
            
        }
    }
}
