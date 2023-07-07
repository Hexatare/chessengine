using System;

namespace ChessEngineClassLibrary.Pieces
{
    /// <summary>
    /// Class Pawn that represents one Pawn on the Board
    /// </summary>
    public class Pawn : Piece
    {

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="chessboard">Reference to the Board</param>
        /// <param name="pColor"></param>
        /// <param name="imgName"></param>
        public Pawn(Board chessboard, PColor pColor, string imgName) : base(chessboard, pColor, imgName)
        {
            PieceType = PType.Pawn;
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
                return PawnMovement(destCell);
            }
            return false;
        }


        /// <summary>
        /// Movement logic of the Pawn
        /// </summary>
        /// <param name="destCell">destination postion of the Pawn</param>
        /// <returns>True, if movement is possible</returns>
        public bool PawnMovement(Cell destCell)
        {
            int one_step;
            int two_step;

            if (this.PieceColor == Piece.PColor.White)
            {
                one_step = 1;
                two_step = 2;
            }
            else
            {
                one_step = -1;
                two_step = -2;
            }

            // Moving one step forward
            if (destCell.Location[1] - Location[1] == one_step)
            {
                // Straight
                if ((Location[0] == destCell.Location[0]) && destCell.IsEmpty)
                {
                    return true;
                }
                // Diagonally
                if (Math.Abs(Location[0] - destCell.Location[0]) == 1 && !destCell.IsEmpty)
                {
                    return true;
                }
            }
            // Two spaces
            else if (!HasMoved)
            {
                if (destCell.Location[1] - Location[1] == two_step)
                {
                    if (Location[0] == destCell.Location[0] && destCell.IsEmpty)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion

    }
}
