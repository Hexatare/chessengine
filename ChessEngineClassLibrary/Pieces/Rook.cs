namespace ChessEngineClassLibrary.Pieces
{
    /// <summary>
    /// Class Rook that represents one Rook on the Board
    /// </summary>
    public class Rook : Piece
    {

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="chessboard">Reference to the Board</param>
        /// <param name="pColor"></param>
        public Rook(Board chessboard, PColor pColor) : base(chessboard, pColor)
        {
            PieceType = PType.Rook;
        }

        #endregion

        #region Game Logic of the Piece and Helper Methods

        /// <summary>
        /// Overloaded Method from base call for Rook
        /// </summary>
        /// <param name="destCell">Cell to move the Piece to</param>
        /// <returns>TURE if possible</returns>
        public override bool CanMoveToDest(Cell destCell)
        {
            if (CanMoveToDestGeneric(destCell))
            {
                return RookMovement(destCell);
            }
            return false;
        }

        /// <summary>
        /// Movement logic of the Rook
        /// </summary>
        /// <param name="destCell">destination postion of the Rook</param>
        /// <returns></returns>
        public bool RookMovement(Cell destCell)
        {
            return this.CanMoveStraight(destCell);
        }

        #endregion

    }
}
