using ChessEngineClassLibrary.Pieces;
using System;

namespace ChessEngineClassLibrary.Models
{
    /// <summary>
    /// Data Object for Game State Information
    /// </summary>
    public class GameStateEventArgs : EventArgs
    {

        /// <summary>
        /// The current Player 
        /// </summary>
        public Piece.PColor CurrentPlayer { get; set; }

        /// <summary>
        /// If a Move was performed, information about it, e.q. UCI Move Format
        /// </summary>
        public string MoveInfo { get; set; } = string.Empty;

        /// <summary>
        /// The remaining Time for the Current Player
        /// </summary>
        public string TimeLeft { get; set; } = string.Empty;

        /// <summary>
        /// Number of Full Moves in ths Game
        /// </summary>
        public int FullMoveNbr { get; set; }


    }
}
