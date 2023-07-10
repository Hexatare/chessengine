using ChessEngineClassLibrary.Pieces;
using Microsoft.Win32;
using System;

namespace ChessEngineClassLibrary
{
    /// <summary>
    /// Class that represents an Chess Move
    /// </summary>
    public class Move
    {
        #region Properties and Members

        // The Piece that was moved
        private Piece PieceMoved; 

        // Starting Cell of the Move
        public Cell Start { get; set; }

        // End Cell of the Move
        public Cell End { get; set; }

        // A Piece that was killed
        public Piece PieceKilled { set; get; }

        // Castling Move
        public bool CastlingMove { set; get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Contructor of the Class
        /// </summary>
        /// <param name="start">Start Cell of the Move</param>
        /// <param name="end">Destination Cell of the Move</param>
        public Move(Cell start, Cell end)
        { 
            Start = start;
            End = end;
            PieceMoved = Start.GetPiece();
        }

        /// <summary>
        /// Returns the X Distance of the Move in absolute Value
        /// </summary>
        /// <returns>X Distance</returns>
        public int GetXMovement()
        {
            return Math.Abs(Start.Location[0] - End.Location[0]);
        }

        /// <summary>
        /// Returns the Y Distance of the Move in absolute Value
        /// </summary>
        /// <returns>X Distance</returns>
        public int GetYMovement()
        {
            return Math.Abs(Start.Location[1] - End.Location[1]);
        }


        #endregion
    }
}
