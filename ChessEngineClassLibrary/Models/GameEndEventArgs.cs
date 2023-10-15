using ChessEngineClassLibrary.Pieces;
using System;
using System.Collections.Generic;

namespace ChessEngineClassLibrary.Models
{

    /// <summary>
    /// Data Object for End Game Information
    /// </summary>
    public class GameEndEventArgs : EventArgs
    {
        /// <summary>
        /// Reason for the End of the Game
        /// </summary>
        public GameEndReason Reason { get; set; }

        /// <summary>
        /// Color of the Winner
        /// </summary>
        public Piece.PColor Winner { get; set; }

        /// <summary>
        /// Number of Moves by the Winner
        /// </summary>
        public int NbrOfMoves { get; set; }

        /// <summary>
        /// Time the Winner played
        /// </summary>
        public string TimePlayed { get; set; } = "";

        /// <summary>
        /// Number of Pieces the Winner captured
        /// </summary>
        public List<string> CapturedPieces { get; set; } = new List<string>();
    }
}
