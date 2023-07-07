using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClassLibrary.Pieces
{

    /// <summary>
    /// Class King that represents one King on the Board
    /// </summary>
    public class King : Piece
    {
       
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="chessboard">Reference to the Board</param>
        /// <param name="pColor"></param>
        /// <param name="imgName"></param>
        public King(Board chessboard, PColor pColor, string imgName) : base(chessboard, pColor, imgName)
        {
            PieceType = PType.King;
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
                return KingMovement(destCell);
            }
            return false;
        }


        /// <summary>
        /// Movement logic of the King
        /// </summary>
        /// <param name="destCell">destination postion of the Pawn</param>
        /// <returns></returns>
        public bool KingMovement(Cell destCell)
        {
            int absoluteX = Math.Abs(destCell.Location[0] - Location[0]);
            int absoluteY = Math.Abs(destCell.Location[1] - Location[1]);

            if (absoluteX <= 1 && absoluteY <= 1)
            {
                if (absoluteX == 0 && absoluteY == 0)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        #endregion

    }
}
