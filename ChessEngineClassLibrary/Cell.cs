using ChessEngineClassLibrary.Models;
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

        #region Enumerator

        /// <summary>
        /// Enummeration of the Cell Border Color
        /// </summary>
        public enum CellBorderColor
        {
            None,
            Yellow,
            Green,
            Red
        }

        #endregion

        #region Properties and private Members

        /// <summary>
        /// Property for the index of the cell
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Location of the piece on the board, [0] = x, [1] = y; 
        /// </summary>
        public int[] Location { get; set; }

        /// <summary>
        /// Status if cell is occupied with a piece
        /// </summary>
        public bool IsEmpty { get; set; }

        /// <summary>
        /// Chess Piece or null if cell is empty
        /// </summary>
        private Piece? PieceOnTheCell;

        /// <summary>
        /// The color of the Cell
        /// </summary>
        public CellColor CellColor { get; set; }

        /// <summary>
        /// Current Border Color Selection
        /// </summary>
        public CellBorderColor CurrCellBorderColor { set; get; }

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
            CellColor = cellColor;
            IsEmpty = true;
            Location = new int[] { (cellIndex % 8), (7 - (cellIndex / 8)) };
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
            IsEmpty = true;
            PieceOnTheCell = null;
        }

        #endregion
    }
}
