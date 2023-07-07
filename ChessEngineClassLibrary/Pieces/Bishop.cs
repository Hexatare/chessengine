using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClassLibrary.Pieces
{

    /// <summary>
    /// Class Bishop that represents one Pawn on the Board
    /// </summary>
    public class Bishop : Piece
    {

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="chessboard">Reference to the Board</param>
        /// <param name="pColor"></param>
        /// <param name="imgName"></param>
        public Bishop(Board chessboard, PColor pColor, string imgName) : base(chessboard, pColor, imgName)
        {
            PieceType = PType.Bishop;
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
                return BishopMovement(destCell);
            }
            return false;
        }


        /// <summary>
        /// Movement logic of the Bishop
        /// </summary>
        /// <param name="destCell">destination postion of the Pawn</param>
        /// <returns></returns>
        public bool BishopMovement(Cell destCell)
        {
            return this.CanMoveDiagonal(destCell);
        }

        #endregion
    }
}
