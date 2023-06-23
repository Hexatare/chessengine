using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Data;
using System.Data.Common;
using System.CodeDom;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using System.Diagnostics.Metrics;

namespace ChessEngineClassLibrary
{
    public class Board
    {
        // Create an array of type Cell and call it MyBoard
        public Cell[] MyBoard { get; set; }

        // List to hold all the pieces on the board
        public List<Piece> MyPieces { get; set; }

        // Constructor. The constructor is what the backend uses as the chessboard
        public Board()
        {
            // Create a new array with the length of 64
            MyBoard = new Cell[64];

            MyPieces = new List<Piece>();

            for (int i = 0; i < 64; i++)
            {
                int cellIndex = i + 1;
                // Check if the square is light or dark
                bool isLightSquare = (i / 8 + i % 8) % 2 == 0;

                // Set the color using an if-else statement
                if (isLightSquare)
                {
                    MyBoard[i] = new Cell(Brushes.Beige, cellIndex);
                }
                else
                {
                    MyBoard[i] = new Cell(Brushes.Brown, cellIndex);
                }
            }
        }

        // Create the chess board by creating Grids in each Square and coloring them
        public void InitChessBoard(Grid chessGrid)
        {
            // Loop through the files
            for (int file = 0; file < 8; file++)
            {
                // Loop through the ranks
                for (int rank = 0; rank < 8; rank++)
                {
                    int cellIndex = (rank * 8) + file;

                    // Create a new Grid with AllowDrop set to true
                    var cell = new Grid
                    {
                        AllowDrop = true,
                    };
                    cell.Drop += Grid_Drop;

                    // Set binding for Color of the cell
                    cell.SetBinding(Grid.BackgroundProperty, new Binding($"MyBoard[{cellIndex}].CellColor") { Source = this });

                    chessGrid.Children.Add(cell);
                    Grid.SetRow(cell, rank);
                    Grid.SetColumn(cell, file);
                }
            }
        }

        // Starting FEN string
        public const string startingFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR"; // w KQkq - 0 1"; -> still need a way to implement this

        // Method to load a specific position from a FEN string. If no string is passed to the method, the default string for a new Chess Game is used
        public void InitPosition(Grid chessGrid, string fenString = startingFen)
        {
            // Create a dictionary to convert the characters to integers
            var convertCharToPiece = new Dictionary<char, int>()
            {
                ['k'] = Piece.King,
                ['p'] = Piece.Pawn,
                ['n'] = Piece.Knight,
                ['b'] = Piece.Bishop,
                ['r'] = Piece.Rook,
                ['q'] = Piece.Queen
            };

            string fenBoard = fenString.Split(' ')[0];
            int file = 0;
            int rank = 0;

            foreach (char character in fenBoard)
            {
                if (character == '/')
                {
                    file = 0;
                    rank++;
                }
                else
                {
                    if (char.IsDigit(character))
                    {
                        file += (int)char.GetNumericValue(character);
                    }
                    else
                    {
                        // Define Attributes of that will be given to the Piece instance
                        int pieceColor = (char.IsUpper(character)) ? Piece.White : Piece.Black;
                        int pieceType = convertCharToPiece[char.ToLower(character)];
                        int cellIndex = file + rank;
                        int pieceAsInt = pieceColor | pieceType;

                        // Create new instance of Piece
                        Piece piece = new Piece(pieceAsInt);

                        // Add the instance to the List of pieces
                        MyPieces.Add(piece);

                        // Update the Array
                        UpdatePieceOnCell(cellIndex, pieceAsInt, piece, chessGrid);
                        file++;
                    }
                }
            }
        }

        // Function to update both the array and the graphical board. This MUST be called to update the PieceOnCell property of the array
        public void UpdatePieceOnCell(int cellIndex, int pieceAsInt, Piece piece, Grid chessGrid)
        {
            MyBoard[cellIndex].PieceOnCell = pieceAsInt;
            piece.LoadImage(pieceAsInt, cellIndex, chessGrid);
        }

        // Method for Dropping the Image
        public void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Bitmap))
            {
                // Retrieve the Grid from the sender
                Grid grid = sender as Grid;

                // Retrieve the Piece instance being dropped
                Piece droppedPiece = null; // <-- Find a way to get the instance of Piece being dropped onto the field. Currentl set to null so that the code can compile but will throw an error when trying to drop the piece

                if (droppedPiece != null)
                {
                    // Access the properties of the dropped Piece instance
                    Debug.WriteLine($"Dropped Piece Type: {droppedPiece.PieceInt}");

                    Image image = new Image();
                    image.MouseMove += droppedPiece.Image_MouseMove;

                    // Set the dropped image's source
                    BitmapSource source = e.Data.GetData(DataFormats.Bitmap) as BitmapSource;
                    if (source != null)
                    {
                        image.Source = source;
                        image.Width = 100;
                        image.Height = 100;
                    }

                    // Remove the image from its current parent (source container)
                    if (droppedPiece.pieceImage.Parent is Panel sourceContainer)
                    {
                        sourceContainer.Children.Remove(droppedPiece.pieceImage);
                    }

                    // Update the Piece.pieceImage reference
                    droppedPiece.pieceImage = image;

                    // Add the image to the Grid
                    if (grid != null && image != null)
                    {
                        grid.Children.Add(image);
                    }
                    else
                    {
                        throw new ArgumentNullException();
                    }
                }

                else
                {
                    throw new ArgumentNullException();
                }
            }
        }

        // Method to update the color of a specified cell
        public void UpdateCellColor(int cellIndex, SolidColorBrush newCellColor)
        {
            MyBoard[cellIndex].CellColor = newCellColor;
        }

        public void PlacePiece(int cellIndex, int pieceType, int pieceColor)
        {
            int piece = pieceColor | pieceType;

            MyBoard[cellIndex].PieceOnCell = piece;
            MyBoard[cellIndex].CurrentlyOccupied = true;
        }
    }
}
