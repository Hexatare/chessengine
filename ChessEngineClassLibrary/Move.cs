using ChessEngineClassLibrary.Pieces;
using Microsoft.Win32;

namespace ChessEngineClassLibrary
{
    /// <summary>
    /// Class that represents an Chess Move
    /// </summary>
    public class Move
    {
        #region Properties and Members

        // Starting Cell of the Move
        private Cell Start;

        // End Cell of the Move
        private Cell End;

        // The Piece that was moved
        private Piece PieceMoved; 

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

        #endregion
    }
}
