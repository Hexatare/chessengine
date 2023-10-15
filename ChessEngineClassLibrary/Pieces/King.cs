using System;

namespace ChessEngineClassLibrary.Pieces
{

    /// <summary>
    /// Class King that represents one King on the Board
    /// </summary>
    public class King : Piece
    {
        #region Properties and Members

        /// <summary>
        /// Kingside castling availability.
        /// </summary>
        public bool CanKingsideCastle { get; set; }

        /// <summary>
        /// Queenside castling availability.
        /// </summary>
        public bool CanQueensideCastle { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="chessboard">Reference to the Board</param>
        /// <param name="pColor"></param>
        public King(Board chessboard, PColor pColor) : base(chessboard, pColor)
        {
            PieceType = PType.King;
            CanKingsideCastle = true;
            CanQueensideCastle = true;
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
        /// <returns>True, if the King can move to this destination</returns>
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


        ///// <summary>
        ///// Method that implements the Castling movement of the King and the Rook
        ///// </summary>
        ///// <param name="destCell">Target Cell to move the King to</param>
        ///// <returns>TRUE if the castling was done</returns>
        //public bool KingCastlingMove(Move moveToDo)
        //{
        //    Cell? rookSourceCell;
        //    Cell? rookDestCell;

        //    if (moveToDo.End !=  null)
        //    {
        //        // Perform the move and move also the Rook
        //        if (moveToDo.End.Location[0] < 4)
        //        {
        //            rookSourceCell = chessBoard.GetCell(0, moveToDo.End.Location[0]);
        //            rookDestCell = chessBoard.GetCell(moveToDo.End.Location[0] + 1, moveToDo.End.Location[1]);
        //        }
        //        else
        //        {
        //            rookSourceCell = chessBoard.GetCell(7, moveToDo.End.Location[1]);
        //            rookDestCell = chessBoard.GetCell(moveToDo.End.Location[0] - 1, moveToDo.End.Location[1]);
        //        }

        //        // Create a Move and tell the Board store it
        //        Move rookMove = new(rookSourceCell, rookDestCell, this.PieceColor);

        //        // Store Info about this castling Move in the Move Object
        //        moveToDo.CastlingMove = true;
        //        moveToDo.RookLoc = rookSourceCell;

        //        CanQueensideCastle = CanKingsideCastle = false;

        //        // Inform the Rook, he has been moved
        //        Piece? rook = rookSourceCell.GetPiece();
        //        if (rook != null )
        //            rook.HasMoved = true;

        //        // Tell the Bord to do the Moves
        //        chessBoard.DoMove(moveToDo);
        //        chessBoard.DoMove(rookMove);
        //        return true;
        //    }
        //    return false;
        //}

        #endregion

    }
}
