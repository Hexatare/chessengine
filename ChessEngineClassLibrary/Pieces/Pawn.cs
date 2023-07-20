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
        public Pawn(Board chessboard, PColor pColor) : base(chessboard, pColor)
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
                    if (Location[0] == destCell.Location[0] && destCell.IsEmpty && CanMoveStraight(destCell))
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// Method that implements the En Passant Pawn Movement
        /// </summary>
        /// <param name="moveToDo">The move to perform</param>
        /// <param name="previousMove">The previouse move, that was performed</param>
        /// <returns>TRUE, if the En Pasant Pawn move was done</returns>
        public bool PawnEnPassantMove(Move moveToDo, Move previousMove)
        {
            Cell? pawnToRemove;

            // Do the Move and remove the Opponents Pawn
            if (moveToDo.PColor == Piece.PColor.White)
            {
                pawnToRemove = chessBoard.GetCell(moveToDo.End.Location[0], moveToDo.End.Location[1] - 1);
            }
            else
            {
                pawnToRemove = chessBoard.GetCell(moveToDo.End.Location[0], moveToDo.End.Location[1] + 1);
            }

            // Call the Move Method
            chessBoard.DoMove(moveToDo);

            if (pawnToRemove != null)
            {
                moveToDo.PieceKilled = pawnToRemove.GetPiece();
                pawnToRemove.RemovePiece();
            }
            return true;
        }


        /// <summary>
        ///  Method that implements the Promotion Pawn Movement
        /// </summary>
        /// <param name="moveToDo">Moveto perform</param>
        /// <param name="promotionPiece">Type of the Piece, that replaces the Pawn</param>
        /// <returns></returns>
        public bool PawnPromotionMove(Move moveToDo, Piece.PType promotionPiece)
        {
            Piece? newPiece = null;

            // Greate the new Piece and replace whith the old one
            // Use switch statement to update the array based on the Piece type
            switch (promotionPiece)
            {
                case Piece.PType.Knight:

                    newPiece = new Knight(chessBoard, moveToDo.PColor);
                    break;

                case Piece.PType.Bishop:

                    newPiece = new Bishop(chessBoard, moveToDo.PColor);
                    break;

                case Piece.PType.Rook:

                    newPiece = new Rook(chessBoard, moveToDo.PColor);
                    break;

                case Piece.PType.Queen:

                    newPiece = new Queen(chessBoard, moveToDo.PColor);
                    break;
            }

            // Tell to board to do the move
            chessBoard.DoMove(moveToDo);

            // Remove the Piece from the destCell and set the new created Piece there
            Piece? oldPawn = moveToDo.End.GetPiece();

            if(oldPawn != null && newPiece != null)
            { 
                moveToDo.End.RemovePiece();
                moveToDo.End.SetPiece(newPiece);
                moveToDo.PromotionMove = true;
                moveToDo.PromotionPiece = promotionPiece;
            }

            return true;
        }

        #endregion

    }
}
