using System;

namespace ChessEngineClassLibrary.Pieces
{


    /// <summary>
    /// Class Bishop that represents one Pawn on the Board
    /// </summary>
    public class Knight : Piece
    {

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="chessboard">Reference to the Board</param>
        /// <param name="pColor"></param>
        public Knight(Board chessboard, PColor pColor) : base(chessboard, pColor)
        {
            PieceType = PType.Knight;
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
                return KnightMovement(destCell);
            }
            return false;
        }


        /// <summary>
        /// Movement logic of the Knight
        /// </summary>
        /// <param name="destCell">destination postion of the Pawn</param>
        /// <returns></returns>
        public bool KnightMovement(Cell destCell)
        {
            if (Math.Abs(Location[0] - destCell.Location[0]) == 2
              && Math.Abs(Location[1] - destCell.Location[1]) == 1)
                return true;

            if (Math.Abs(Location[0] - destCell.Location[0]) == 1
                && Math.Abs(Location[1] - destCell.Location[1]) == 2)
                return true;

            return false;
        }

        #endregion

    }
}
