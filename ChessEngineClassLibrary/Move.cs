using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChessEngineClassLibrary
{
    /// <summary>
    /// Class that represents an Chess Move
    /// </summary>
    internal class Move
    {
        // Reference to the Player  
        private Player Player;

        // Starting Cell of the Move
        private Cell Start;

        // End Cell of the Move
        private Cell End;

        // The Piece that was moved
        private Piece PieceMoved; 

        // A Piece that was killed
        private Piece PieceKilled { set; get; }

        // Castling Move
        private bool CastlingMove { set; get; }


        /// <summary>
        /// Contructor of the Class
        /// </summary>
        /// <param name="player">Refernz to the Player</param>
        /// <param name="start">Start Cell of the Move</param>
        /// <param name="end">Destination Cell of the Move</param>
        public Move(Player player, Cell start, Cell end)
        { 
            Player = player;
            Start = start;
            End = end;
            PieceMoved = Start.GetPiece();
        }
    }
}
