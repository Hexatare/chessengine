using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.PeerToPeer.Collaboration;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClassLibrary.Pieces
{

    /// <summary>
    /// Class Queen that represents one Queen on the Board
    /// </summary>
    public class Queen : Piece
    {

        #region Constructor 

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="chessboard">Reference to the Board</param>
        /// <param name="pColor"></param>
        /// <param name="imgName"></param>
        public Queen(Board chessboard, PColor pColor) : base(chessboard, pColor)
        {
            PieceType = PType.Queen;
        }

        #endregion

        #region Game Logic of the Piece and Helper Methods

        /// <summary>
        /// Overloaded Method from base call for Pawn
        /// </summary>
        /// <param name="destCell">Cell to move the Piece to</param>
        /// <returns>TURE if possible</returns>
        public override bool CanMoveToDest(Cell destCell)
        {
            if (CanMoveToDestGeneric(destCell))
            {
                return QueenMovement(destCell);
            }
            return false;
        }


        /// <summary>
        /// Movement logic of the Queen
        /// </summary>
        /// <param name="destCell">destination postion of the Pawn</param>
        /// <returns></returns>
        public bool QueenMovement(Cell destCell)
        {
            if (this.CanMoveStraight(destCell)
                || this.CanMoveDiagonal(destCell))
                return true;
            return false;
        }

        #endregion
    }
}
