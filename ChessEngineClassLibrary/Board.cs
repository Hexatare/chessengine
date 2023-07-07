using ChessEngineClassLibrary.Pieces;
using System.Collections.Generic;
using System.Windows.Controls;

namespace ChessEngineClassLibrary
{
    /// <summary>
    /// Class Board implements the Chessboard with its 64 Cells.
    /// </summary>
    public class Board
    {
        #region Properties and Members

        // Number of Cells
        private const int NbrOfCells = 64;

        // Create an array of type Cell and call it CellsOnBoard
        private Cell[] CellsOnBoard;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor. The constructor is what the backend uses as the chessboard. It calls the methods CreateCells and CreatePieces
        /// </summary>
        /// <param name="chessGrid">Reference to the Main Application</param>
        public Board(Grid chessGrid)
        {
            // Create a new array with the length of 64
            CellsOnBoard = new Cell[NbrOfCells];

            // Create alls Cells
            for (int i = 0; i < NbrOfCells; i++)
            {
                int cellIndex = i + 1;

                // Check if the square is light or dark
                bool isLightSquare = (i / 8 + i % 8) % 2 == 0;

                // Set the color
                CellsOnBoard[i] = new Cell(this, isLightSquare ? Cell.CellColor.White : Cell.CellColor.Black, i);

                // Add Cell to the Grid
                chessGrid.Children.Add(CellsOnBoard[i].Grid);
                Grid.SetRow(CellsOnBoard[i].Grid, (i / 8));
                Grid.SetColumn(CellsOnBoard[i].Grid, (i % 8));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Place a Piece on the Board with x and y Coordinate
        /// </summary>
        /// <param name="piece">Piece to be placed</param>
        /// <param name="xIndex">X Coordinate on the Board</param>
        /// <param name="yIndex">Y Coordinate on the Board</param>
        public void PlacePlieceOnBoard(Piece piece, int xIndex, int yIndex)
        {
            // Index, starting from 0 (a1) to 63 (h8), in a regular Chessboard
            int cellIndex = xIndex + ((yIndex % 7) * 8);

            // Set a piece on the board
            CellsOnBoard[cellIndex].SetPiece(piece);
        }


        /// <summary>
        /// Place a Piece on the Board with Board Array [0..63]
        /// </summary>
        /// <param name="piece">Piece to be placed</param>
        /// <param name="arrayIndex">Array Index of the Board [0..63]</param>
        public void PlacePlieceOnBoard(Piece piece, int arrayIndex)
        {
            // Set a piece on the board
            CellsOnBoard[arrayIndex].SetPiece(piece);
        }


        /// <summary>
        /// Removes all Pieces on the board
        /// </summary>
        public void RemoveAllPieces()
        {
            foreach (Cell cell in CellsOnBoard)
            {
                if (!cell.IsEmpty)
                    cell.RemovePiece();
            }
        }


        /// <summary>
        /// Returns the array index of a Cell on the board
        /// </summary>
        /// <param name="xIndex"></param>
        /// <param name="yIndex"></param>
        /// <returns></returns>
        public int GetArrayIndex(int xIndex, int yIndex)
        {
            // Index on the chess board, from 0 (a8) to 63 (h7)
            return xIndex + ((7 - (yIndex % 8) ) * 8);
        }


        /// <summary>
        /// Returns the coordinate (x and y) on the board
        /// </summary>
        /// <param name="arrayIndex"></param>
        /// <returns>array with [x,y] values</returns>
        public int[] GetBoardIndex(int arrayIndex)
        { 
            return new int[] { (arrayIndex % 8), (7 - (arrayIndex / 8)) };
        }


        /// <summary>
        /// Returns the Cell on Location [x,y]
        /// </summary>
        /// <param name="xIndex">x Coordinate</param>
        /// <param name="yIndex">y Coordinate</param>
        /// <returns>the requested Cell</returns>
        public Cell GetCell(int xIndex, int yIndex) 
        {
            return CellsOnBoard[GetArrayIndex(xIndex, yIndex)];
        }

        /// <summary>
        /// Returns the Cell at Position index
        /// </summary>
        /// <param name="arrayIndex">Index of the >Cell</param>
        /// <returns>the requested Cell</returns>
        public Cell GetCell(int arrayIndex)
        {
            return CellsOnBoard[arrayIndex];
        }


        /// <summary>
        /// Returns a List with all Cells of the Board
        /// </summary>
        /// <returns>List with all Cells of the Board</returns>
        public List<Cell> GetCells()
        {
            List<Cell> cells = new List<Cell>();
            foreach (Cell cell in CellsOnBoard)
                cells.Add(cell);

            return cells;
        }

        #endregion
    }
}
