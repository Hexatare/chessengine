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

        // Constructor. The constructor is what the backend uses as the chessboard
        public Board(Grid chessGrid)
        {
            // Create a new array with the length of 64
            MyBoard = new Cell[64];

            for (int i = 0; i < 64; i++)
            {
                int cellIndex = i + 1;
                // Check if the square is light or dark
                bool isLightSquare = (i / 8 + i % 8) % 2 == 0;

                // Create a new Grid to Add to the array
                Grid cellGrid = new Grid();
                cellGrid.AllowDrop = true;
                cellGrid.Background = Brushes.Blue;
                // cellGrid.Drop += Grid_Drop;

                // Set binding for Color of the cell    
                cellGrid.SetBinding(Grid.BackgroundProperty, new Binding($"MyBoard[{i}].CellColor") { Source = this });

                // Set row and column for the Grid
                Grid.SetRow(cellGrid, i / 8);
                Grid.SetColumn(cellGrid, i % 8);

                // Create a new Cell object and add it to the array
                MyBoard[i] = new Cell(isLightSquare ? Brushes.White : Brushes.Gray, cellIndex, null, cellGrid, chessGrid);

                chessGrid.Children.Add(MyBoard[i].CellGrid);
            }
        }

        // Normal starting FEN string for Chess game
        public const string startingFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR"; // w KQkq - 0 1"; -> still need a way to implement this

        // Method to load a specific position from a FEN string. If no string is passed to the method, the default string for a new Chess Game is used
        public void InitPosition(Board myBoard, Grid chessGrid, string fenString = startingFen)
        {
            // Create variable to keep track of the index of the cells
            int cellIndex = 0;;

            string fenBoard = fenString.Split(' ')[0];

            foreach (char character in fenBoard)
            {
                if (character == '/')
                {
                    cellIndex++;
                }
                else
                {
                    if (char.IsDigit(character))
                    {
                        cellIndex += (int)char.GetNumericValue(character);
                    }
                    else
                    {
                        // Define Attributes of that will be given to the Piece instance
                        string pieceColor = (char.IsUpper(character)) ? "white" : "black";

                        // Use switch statement to update the array based on the Piece type
                        switch (char.ToLower(character))
                        {
                            case 'p':
                                myBoard.MyBoard[cellIndex].PieceOnCell = new Pawn(pieceColor, cellIndex);
                                break;
                            
                            case 'n':
                                myBoard.MyBoard[cellIndex].PieceOnCell = new Knight(pieceColor, cellIndex);
                                break;
                            
                            case 'b':
                                myBoard.MyBoard[cellIndex].PieceOnCell = new Bishop(pieceColor, cellIndex); 
                                break;
                            
                            case 'r':
                                myBoard.MyBoard[cellIndex].PieceOnCell = new Rook(pieceColor, cellIndex); 
                                break;
                            
                            case 'q':
                                myBoard.MyBoard[cellIndex].PieceOnCell = new Queen(pieceColor, cellIndex); 
                                break;
                            
                            case 'k':
                                myBoard.MyBoard[cellIndex].PieceOnCell = new King(pieceColor, cellIndex); 
                                break;
                        }
                    }
                }
            }
        }

        // Get the legal moves for a piece
        public List<int> GetLegalMoves(int startCellIndex, int endCellIndex, string pieceColor, Object piece)
        {
            // Create a list to store the legal moves
            List<int> legalMoves = new List<int>();


            // Use switch statement for the piece type
            switch (piece)
            {
                // Pawn
                case Pawn:
                    // Check if the pawn is on the 2nd or 7th rank, to see if it can move 2 squares
                    if ((startCellIndex < 16 && pieceColor == "white") || (startCellIndex > 47 && pieceColor == "black"))
                    {
                        // Check if the pawn can move 2 squares
                        if ((endCellIndex == startCellIndex + 16 && pieceColor == "white") || (endCellIndex == startCellIndex - 16 && pieceColor == "black"))
                        {
                            // Check if the pawn can move 2 squares
                            if (MyBoard[endCellIndex].PieceOnCell == null && MyBoard[endCellIndex - 8].PieceOnCell == null)
                            {
                                // Add the move to the list of legal moves
                                legalMoves.Add(endCellIndex);
                            }
                        }
                    }

                    // Check if the pawn can move 1 square
                    if ((endCellIndex == startCellIndex + 8 && pieceColor == "white") || (endCellIndex == startCellIndex - 8 && pieceColor == "black"))
                    {
                        // Check if the pawn can move 1 square
                        if (MyBoard[endCellIndex].PieceOnCell == null)
                        {
                            // Add the move to the list of legal moves
                            legalMoves.Add(endCellIndex);
                        }
                    }

                    // Check if the pawn can capture a piece
                    if (((endCellIndex == startCellIndex + 7 || endCellIndex == startCellIndex + 9) && pieceColor == "white") || ((endCellIndex == startCellIndex - 7 || endCellIndex == startCellIndex - 9) && pieceColor == "black"))
                    {
                        // Check if the pawn can capture a piece
                        if (MyBoard[endCellIndex].PieceOnCell != null)
                        {
                            // Add the move to the list of legal moves
                            legalMoves.Add(endCellIndex);
                        }
                    }

                    break;

                case Knight:
                    // Check if the Knight can move to the cell
                    if ((endCellIndex == startCellIndex + 6 || endCellIndex == startCellIndex + 10 || endCellIndex == startCellIndex + 15 || endCellIndex == startCellIndex + 17 || endCellIndex == startCellIndex - 6 || endCellIndex == startCellIndex - 10 || endCellIndex == startCellIndex - 15 || endCellIndex == startCellIndex - 17) && pieceColor != MyBoard[endCellIndex].PieceOnCell.PieceColor)
                    {
                        // Add the move to the list of legal moves
                        legalMoves.Add(endCellIndex);
                    }

                    break;

                case Bishop:
                    // Check if the Bishop can move to the cell
                    int bishopStartRow = startCellIndex / 8;
                    int bishopStartCol = startCellIndex % 8;
                    int bishopEndRow = endCellIndex / 8;
                    int bishopEndCol = endCellIndex % 8;

                    int bishopRowDiff = Math.Abs(bishopEndRow - bishopStartRow);
                    int bishopColDiff = Math.Abs(bishopEndCol - bishopStartCol);

                    if (bishopRowDiff == bishopColDiff && pieceColor != MyBoard[endCellIndex].PieceOnCell.PieceColor)
                    {
                        // Add the move to the list of legal moves
                        legalMoves.Add(endCellIndex);
                    }
                    break;

                case Rook:
                    // Check if the Rook can move to the cell
                    int rookStartRow = startCellIndex / 8;
                    int rookStartCol = startCellIndex % 8;
                    int rookEndRow = endCellIndex / 8;
                    int rookEndCol = endCellIndex % 8;

                    if ((rookStartRow == rookEndRow || rookStartCol == rookEndCol) && pieceColor != MyBoard[endCellIndex].PieceOnCell.PieceColor)
                    {
                        // Check if any pieces are blocking the rook's path
                        bool isPathClear = true;

                        // Check vertical movement
                        if (rookStartCol == rookEndCol)
                        {
                            int minRow = Math.Min(rookStartRow, rookEndRow);
                            int maxRow = Math.Max(rookStartRow, rookEndRow);

                            for (int row = minRow + 1; row < maxRow; row++)
                            {
                                int cellIndex = row * 8 + rookStartCol;
                                if (MyBoard[cellIndex].PieceOnCell != null)
                                {
                                    isPathClear = false;
                                    break;
                                }
                            }
                        }
                        // Check horizontal movement
                        else if (rookStartRow == rookEndRow)
                        {
                            int minCol = Math.Min(rookStartCol, rookEndCol);
                            int maxCol = Math.Max(rookStartCol, rookEndCol);

                            for (int col = minCol + 1; col < maxCol; col++)
                            {
                                int cellIndex = rookStartRow * 8 + col;
                                if (MyBoard[cellIndex].PieceOnCell != null)
                                {
                                    isPathClear = false;
                                    break;
                                }
                            }
                        }

                        if (isPathClear)
                        {
                            // Add the move to the list of legal moves
                            legalMoves.Add(endCellIndex);
                        }
                    }
                    break;

                case Queen:
                    // Check if the Queen can move to the cell
                    int queenStartRow = startCellIndex / 8;
                    int queenStartCol = startCellIndex % 8;
                    int queenEndRow = endCellIndex / 8;
                    int queenEndCol = endCellIndex % 8;

                    int queenRowDiff = Math.Abs(queenEndRow - queenStartRow);
                    int queenColDiff = Math.Abs(queenEndCol - queenStartCol);

                    if ((queenStartRow == queenEndRow || queenStartCol == queenEndCol || queenRowDiff == queenColDiff) && pieceColor != MyBoard[endCellIndex].PieceOnCell.PieceColor)
                    {
                        // Check if any pieces are blocking the queen's path
                        bool isPathClear = true;

                        // Check vertical movement
                        if (queenStartCol == queenEndCol)
                        {
                            int minRow = Math.Min(queenStartRow, queenEndRow);
                            int maxRow = Math.Max(queenStartRow, queenEndRow);

                            for (int row = minRow + 1; row < maxRow; row++)
                            {
                                int cellIndex = row * 8 + queenStartCol;
                                if (MyBoard[cellIndex].PieceOnCell != null)
                                {
                                    isPathClear = false;
                                    break;
                                }
                            }
                        }
                        // Check horizontal movement
                        else if (queenStartRow == queenEndRow)
                        {
                            int minCol = Math.Min(queenStartCol, queenEndCol);
                            int maxCol = Math.Max(queenStartCol, queenEndCol);

                            for (int col = minCol + 1; col < maxCol; col++)
                            {
                                int cellIndex = queenStartRow * 8 + col;
                                if (MyBoard[cellIndex].PieceOnCell != null)
                                {
                                    isPathClear = false;
                                    break;
                                }
                            }
                        }
                        // Check diagonal movement
                        else if (queenRowDiff == queenColDiff)
                        {
                            int minRow = Math.Min(queenStartRow, queenEndRow);
                            int maxRow = Math.Max(queenStartRow, queenEndRow);
                            int minCol = Math.Min(queenStartCol, queenEndCol);
                            int maxCol = Math.Max(queenStartCol, queenEndCol);

                            int row = minRow + 1;
                            int col = minCol + 1;

                            while (row < maxRow && col < maxCol)
                            {
                                int cellIndex = row * 8 + col;
                                if (MyBoard[cellIndex].PieceOnCell != null)
                                {
                                    isPathClear = false;
                                    break;
                                }
                                row++;
                                col++;
                            }
                        }

                        if (isPathClear)
                        {
                            // Add the move to the list of legal moves
                            legalMoves.Add(endCellIndex);
                        }
                    }
                    break;

                case King:
                    // Check if the King can move to the cell
                    int kingStartRow = startCellIndex / 8;
                    int kingStartCol = startCellIndex % 8;
                    int kingEndRow = endCellIndex / 8;
                    int kingEndCol = endCellIndex % 8;

                    int kingRowDiff = Math.Abs(kingEndRow - kingStartRow);
                    int kingColDiff = Math.Abs(kingEndCol - kingStartCol);

                    if ((kingRowDiff <= 1 && kingColDiff <= 1) && pieceColor != MyBoard[endCellIndex].PieceOnCell.PieceColor)
                    {
                        // Add the move to the list of legal moves
                        legalMoves.Add(endCellIndex);
                    }
                    break;
            }

            return legalMoves;

        }

        /*
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
                    Debug.WriteLine($"Dropped Piece Type: {droppedPiece}");

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
        */
    }
}
