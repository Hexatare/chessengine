using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ChessEngineClassLibrary.Resources;

namespace ChessEngineClassLibrary
{
    public class Board
    {
        // Number of Cells
        private const int NbrOfCells = 64;

        // Create an array of type Cell and call it MyBoard
        private Cell[] MyBoard { get; set; }

        // List to hold all the pieces on the board
        public List<Piece> MyPieces { get; set; }

        // Selected Cell form which a Piece could be moved. Default set to -1
        private int SelectedCellIndex = -1;
        
        /// <summary>
        /// Constructor. The constructor is what the backend uses as the chessboard. It calls the methods CreateCells and CreatePieces
        /// </summary>
        /// <param name="chessGrid">Reference to the Main Application</param>
        public Board(Grid chessGrid)
        {
            // Create all Cells
            CreateCells(chessGrid);

            // Create all Pieces
            // CreatePieces(chessGrid);
            //MyPieces = new List<Piece>();

            // Create all Pieces
            InitPosition();


        }

        /// <summary>
        /// Method to create all Cells on the Board. Each Cell contains a Grid Element,
        /// which is added to the main grid of the Board
        /// </summary>
        /// <param name="chessGrid">Reference to the main board</param>
        private void CreateCells(Grid chessGrid)
        {
            // Create a new array with the length of 64
            MyBoard = new Cell[NbrOfCells];

            // Create alls Cells
            for (int i = 0; i < NbrOfCells; i++)
            {
                int cellIndex = i + 1;

                // Check if the square is light or dark
                bool isLightSquare = (i / 8 + i % 8) % 2 == 0;

                // Set the color
                MyBoard[i] = new Cell(this, isLightSquare ? Cell.CellColor.White : Cell.CellColor.Black, i);

                // Add Cell to the Grid
                chessGrid.Children.Add(MyBoard[i].Grid);
                Grid.SetRow(MyBoard[i].Grid, (i / 8));
                Grid.SetColumn(MyBoard[i].Grid, (i % 8));
            }
        }


        /// <summary>
        /// Create all Pieces and place them on the Board
        /// </summary>
        /// <param name="chessGrid"></param>
        /*
        private void CreatePieces(Grid chessGrid)
        {
            // Create a list to hold the pieces
            MyPieces = new List<Piece>();
            
            // Generate all white Pieces
            for (int i = 0; i < 1; i++)
            {
                Piece piece = new Piece(Piece.PType.Pawn, Resource1.wPawn);
                piece.PieceColor = Piece.PColor.White;
                MyBoard[0].SetPiece(piece);
            }


            // Place the Pieces on the Board
            

            // Enumerate the assembly's manifest resources
            //foreach (string rName in assembly.GetManifestResourceNames())
            //{
            //    MessageBox.Show(rName);
            //}

            // Get the full name of the resource
            //string resourceName = "Resources/Pictures/blackBishop.png";
            //resourceName = "C:\\Users\\rolfm\\source\\repos\\chessengine\\ChessEngineClassLibrary\\Resources\\Pictures\\blackBishop.png";

            //// Create Bitmap Image
            //BitmapImage bitmap = new BitmapImage(new Uri(resourceName));

            //string fullResourceName = assembly.GetName().Name + "." + resourceName.Replace("/", ".").Replace("\\", ".");
            //Stream ImgStream = assembly.GetManifestResourceStream(fullResourceName);

            //if (ImgStream != null)
            //{
            //    // Create Bitmap Image
            //    System.Drawing.Image PieceImage = new Bitmap(ImgStream);
            //}


        }

        */


        // Create the chess board by creating Grids in each Square and coloring them
        /*
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

            // Create the Pieces and load the images
            
            // Get the assembly containing the resource
            //Assembly assembly = Assembly.GetExecutingAssembly();

            //// Enumerate the assembly's manifest resources
            ////foreach (string rName in assembly.GetManifestResourceNames())
            ////{
            ////    MessageBox.Show(rName);
            ////}

            //    // Get the full name of the resource
            //    string resourceName = "Resources/Pictures/blackBishop.png";
            ////resourceName = "C:\\Users\\rolfm\\source\\repos\\chessengine\\ChessEngineClassLibrary\\Resources\\Pictures\\blackBishop.png";

            ////// Create Bitmap Image
            ////BitmapImage bitmap = new BitmapImage(new Uri(resourceName));

            //string fullResourceName = assembly.GetName().Name + "." + resourceName.Replace("/", ".").Replace("\\", ".");
            //Stream ImgStream = assembly.GetManifestResourceStream(fullResourceName);
            
            //if (ImgStream != null)
            //{
            //    // Create Bitmap Image
            //    System.Drawing.Image PieceImage = new Bitmap(ImgStream);
            //}
            
        }
        */

        // Normal starting FEN string for Chess game
        public const string startingFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR"; // w KQkq - 0 1"; -> still need a way to implement this

        // Method to load a specific position from a FEN string. If no string is passed to the method, the default string for a new Chess Game is used
        public void InitPosition(string fenString = startingFen)
        {
            // Create variable to keep track of the index of the cells
            int cellIndex = 0;

            string fenBoard = fenString.Split(' ')[0];

            foreach (char character in fenBoard)
            {
                Debug.WriteLine(character);
                Debug.WriteLine(cellIndex);

                if (char.IsDigit(character))
                {
                    cellIndex += (int)char.GetNumericValue(character);
                }
                else if (character == '/')
                {
                    // Do nothing
                }
                else
                {
                    // Define Attributes of that will be given to the Piece instance
                    Piece.PColor pieceColor = (char.IsUpper(character)) ? Piece.PColor.White : Piece.PColor.Black;

                    // Use switch statement to update the array based on the Piece type
                    switch (char.ToLower(character))
                    {
                        case 'p':
                            Pawn newPawn;

                            if (pieceColor == Piece.PColor.White)
                                newPawn = new Pawn(Piece.PColor.White, Resource1.wPawn);
                            else
                                newPawn = new Pawn(Piece.PColor.Black, Resource1.bPawn);
                                
                            MyBoard[cellIndex].SetPiece(newPawn);
                            break;

                        case 'n':
                            Knight newKnight;

                            if (pieceColor == Piece.PColor.White)
                                newKnight = new Knight(Piece.PColor.White, Resource1.wKnight);
                            else
                                newKnight = new Knight(Piece.PColor.Black, Resource1.bKnight);

                            MyBoard[cellIndex].SetPiece(newKnight);
                            break;

                        case 'b':
                            Bishop newBishop;

                            if (pieceColor == Piece.PColor.White)
                                newBishop = new Bishop(Piece.PColor.White, Resource1.wBishop);
                            else
                                newBishop = new Bishop(Piece.PColor.Black, Resource1.bBishop);

                            MyBoard[cellIndex].SetPiece(newBishop);
                            break;

                        case 'r':
                            Rook newRook;

                            if (pieceColor == Piece.PColor.White)
                                newRook = new Rook(Piece.PColor.White, Resource1.wRook);
                            else
                                newRook = new Rook(Piece.PColor.Black, Resource1.bRook);

                            MyBoard[cellIndex].SetPiece(newRook);
                            break;

                        case 'q':
                            Queen newQueen;

                            if (pieceColor == Piece.PColor.White)
                                newQueen = new Queen(Piece.PColor.White, Resource1.wQueen);
                            else
                                newQueen = new Queen(Piece.PColor.Black, Resource1.bQueen);

                            MyBoard[cellIndex].SetPiece(newQueen);
                            break;

                        case 'k':
                            King newKing;

                            if (pieceColor == Piece.PColor.White)
                                newKing = new King(Piece.PColor.White, Resource1.wKing);
                            else
                                newKing = new King(Piece.PColor.Black, Resource1.bKing);

                            MyBoard[cellIndex].SetPiece(newKing);
                            break;
                    }

                    cellIndex++;
                }
            }
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

                    System.Windows.Controls.Image image = new System.Windows.Controls.Image();
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
                    //if (droppedPiece.pieceImage.Parent is Panel sourceContainer)
                    //{
                        //sourceContainer.Children.Remove(droppedPiece.pieceImage);
                    //}

                    // Update the Piece.pieceImage reference
                    //droppedPiece.pieceImage = image;

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
        
        internal void SetSelectedCell(int index, bool isCellEmpty)
        {
            // Origin Cell is selected, move to targe cell 
            if(isCellEmpty && SelectedCellIndex != -1)
            {
                // Check if the move is legal
                List<int> legalMoves = GetLegalMoves(SelectedCellIndex, index, MyBoard[SelectedCellIndex].GetPiece().PieceColor, MyBoard[SelectedCellIndex].GetPiece());

                if (legalMoves.Contains(index))
                {
                    // Get Image from selected cell
                    Cell cell = MyBoard[SelectedCellIndex];
                    Piece piece = cell.GetPiece();

                    MyBoard[SelectedCellIndex].RemovePiece();
                    MyBoard[index].SetPiece(piece);
                }
                else
                {
                    // Move is not legal
                    Debug.WriteLine("Move is not legal");
                }
                             
                SelectedCellIndex = -1;
            }
            else if(!isCellEmpty) 
            {
                SelectedCellIndex = index;
            }
            
            Debug.WriteLine("Selected Cell " + index);
        }

        // Get the legal moves for a piece
        public List<int> GetLegalMoves(int startCellIndex, int endCellIndex, Piece.PColor pieceColor, Object piece)
        {
            // Create a list to store the legal moves
            List<int> legalMoves = new List<int>();

            // Use switch statement for the piece type
            switch (piece)
            {
                // Pawn
                case Pawn:
                    // Check if the pawn is on the 2nd or 7th rank, to see if it can move 2 squares
                    if ((startCellIndex < 16 && pieceColor == Piece.PColor.Black) || (startCellIndex > 47 && pieceColor == Piece.PColor.White))
                    {
                        // Check if the pawn can move 2 squares
                        if ((endCellIndex == startCellIndex - 16 && pieceColor == Piece.PColor.White) || (endCellIndex == startCellIndex + 16 && pieceColor == Piece.PColor.Black))
                        {
                            // Check if the pawn can move 2 squares
                            if (MyBoard[endCellIndex].PieceOnTheCell == null && MyBoard[endCellIndex - 8].PieceOnTheCell == null)
                            {
                                // Add the move to the list of legal moves
                                legalMoves.Add(endCellIndex);
                            }
                        }
                    }

                    // Check if the pawn can move 1 square
                    if ((endCellIndex == startCellIndex - 8 && pieceColor == Piece.PColor.White) || (endCellIndex == startCellIndex + 8 && pieceColor == Piece.PColor.Black))
                    {
                        // Check if the pawn can move 1 square
                        if (MyBoard[endCellIndex].PieceOnTheCell == null)
                        {
                            // Add the move to the list of legal moves
                            legalMoves.Add(endCellIndex);
                        }
                    }

                    // Check if the pawn can capture a piece
                    if (((endCellIndex == startCellIndex - 7 || endCellIndex == startCellIndex - 9) && pieceColor == Piece.PColor.White) || ((endCellIndex == startCellIndex - 7 || endCellIndex == startCellIndex - 9) && pieceColor == Piece.PColor.Black))
                    {
                        // Check if the pawn can capture a piece
                        if (MyBoard[endCellIndex].PieceOnTheCell != null)
                        {
                            // Add the move to the list of legal moves
                            legalMoves.Add(endCellIndex);
                        }
                    }

                    break;

                case Knight:
                    // Check if the Knight can move to the cell
                    if ((endCellIndex == startCellIndex - 6 || endCellIndex == startCellIndex - 10 || endCellIndex == startCellIndex - 15 || endCellIndex == startCellIndex - 17 || endCellIndex == startCellIndex - 6 || endCellIndex == startCellIndex - 10 || endCellIndex == startCellIndex - 15 || endCellIndex == startCellIndex - 17) && pieceColor != MyBoard[endCellIndex].PieceOnTheCell.PieceColor)
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

                    if (bishopRowDiff == bishopColDiff && pieceColor != MyBoard[endCellIndex].PieceOnTheCell.PieceColor)
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

                    if ((rookStartRow == rookEndRow || rookStartCol == rookEndCol) && pieceColor != MyBoard[endCellIndex].PieceOnTheCell.PieceColor)
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
                                if (MyBoard[cellIndex].PieceOnTheCell != null)
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
                                if (MyBoard[cellIndex].PieceOnTheCell != null)
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

                    if ((queenStartRow == queenEndRow || queenStartCol == queenEndCol || queenRowDiff == queenColDiff) && pieceColor != MyBoard[endCellIndex].PieceOnTheCell.PieceColor)
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
                                if (MyBoard[cellIndex].PieceOnTheCell != null)
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
                                if (MyBoard[cellIndex].PieceOnTheCell != null)
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
                                if (MyBoard[cellIndex].PieceOnTheCell != null)
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

                    if ((kingRowDiff <= 1 && kingColDiff <= 1) && pieceColor != MyBoard[endCellIndex].PieceOnTheCell.PieceColor)
                    {
                        // Add the move to the list of legal moves
                        legalMoves.Add(endCellIndex);
                    }
                    break;
            }

            return legalMoves;

        }
    }
}
