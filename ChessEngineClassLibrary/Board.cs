using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ChessEngineClassLibrary.Pieces;
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

        // Selected Cell from which a Piece could be moved. Default set to -1
        private int SelectedCellIndex = -1;
        

        /// <summary>
        /// Constructor. The constructor is what the backend uses as the chessboard. It calls the methods CreateCells and CreatePieces
        /// </summary>
        /// <param name="chessGrid">Reference to the Main Application</param>
        public Board(Grid chessGrid)
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
        /// A cell was selected by the user
        /// </summary>
        /// <param name="index">Array index of the selected Call</param>
        /// <param name="isCellEmpty"></param>
        internal void CellSelected(int index, bool isCellEmpty)
        {

            // Origin Cell is selected, move to targe cell 
            if (isCellEmpty && SelectedCellIndex != -1)
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
                            if (MyBoard[endCellIndex].GetPiece() == null && MyBoard[endCellIndex - 8].GetPiece() == null)
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
                        if (MyBoard[endCellIndex].GetPiece() == null)
                        {
                            // Add the move to the list of legal moves
                            legalMoves.Add(endCellIndex);
                        }
                    }

                    // Check if the pawn can capture a piece
                    if (((endCellIndex == startCellIndex - 7 || endCellIndex == startCellIndex - 9) && pieceColor == Piece.PColor.White) || ((endCellIndex == startCellIndex - 7 || endCellIndex == startCellIndex - 9) && pieceColor == Piece.PColor.Black))
                    {
                        // Check if the pawn can capture a piece
                        if (MyBoard[endCellIndex].GetPiece() != null)
                        {
                            // Add the move to the list of legal moves
                            legalMoves.Add(endCellIndex);
                        }
                    }

                    break;

                case Knight:
                    // Check if the Knight can move to the cell
                    if ((endCellIndex == startCellIndex - 6 || endCellIndex == startCellIndex - 10 || endCellIndex == startCellIndex - 15 || endCellIndex == startCellIndex - 17 || endCellIndex == startCellIndex - 6 || endCellIndex == startCellIndex - 10 || endCellIndex == startCellIndex - 15 || endCellIndex == startCellIndex - 17) && pieceColor != MyBoard[endCellIndex].GetPiece().PieceColor)
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

                    if (bishopRowDiff == bishopColDiff && pieceColor != MyBoard[endCellIndex].GetPiece().PieceColor)
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

                    if ((rookStartRow == rookEndRow || rookStartCol == rookEndCol) && pieceColor != MyBoard[endCellIndex].GetPiece().PieceColor)
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
                                if (MyBoard[cellIndex].GetPiece() != null)
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
                                if (MyBoard[cellIndex].GetPiece() != null)
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

                    if ((queenStartRow == queenEndRow || queenStartCol == queenEndCol || queenRowDiff == queenColDiff) && pieceColor != MyBoard[endCellIndex].GetPiece().PieceColor)
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
                                if (MyBoard[cellIndex].GetPiece() != null)
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
                                if (MyBoard[cellIndex].GetPiece() != null)
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
                                if (MyBoard[cellIndex].GetPiece() != null)
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

                    if ((kingRowDiff <= 1 && kingColDiff <= 1) && pieceColor != MyBoard[endCellIndex].GetPiece().PieceColor)
                    {
                        // Add the move to the list of legal moves
                        legalMoves.Add(endCellIndex);
                    }
                    break;
            }

            return legalMoves;

        }


        public void PlacePlieceOnBoard(Piece piece, int xIndex, int yIndex)
        {
            // Index, starting from 0 (a1) to 63 (h8), in a regular Chessboard
            int cellIndex = xIndex + (yIndex * 8);
                    
            // Set a piece on the board
            MyBoard[GetBoardIndex(xIndex, yIndex)].SetPiece(piece);
        }


        public void PlacePlieceOnBoard(Piece piece, int arrayIndex)
        {
            // Set a piece on the board
            MyBoard[arrayIndex].SetPiece(piece);
        }

        /// <summary>
        /// Returns the array index of a Cell on the board
        /// </summary>
        /// <param name="xIndex"></param>
        /// <param name="yIndex"></param>
        /// <returns></returns>
        public int GetBoardIndex(int xIndex, int yIndex)
        {
            // Index on the chess board, from 0 (a8) to 63 (h7)
            return xIndex + ((yIndex % 7) * 8);
        }

        /// <summary>
        /// Removes all Pieces on the board
        /// </summary>
        public void RemoveAllPieces()
        { 
            foreach(Cell cell in MyBoard) 
            {
                if (!cell.IsCellEmpty)
                    cell.RemovePiece();
            }
        }

    }
}
