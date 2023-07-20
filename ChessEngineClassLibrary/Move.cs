using ChessEngineClassLibrary.Pieces;
using System;
using System.Text;
using static ChessEngineClassLibrary.Pieces.Piece;

namespace ChessEngineClassLibrary
{
    /// <summary>
    /// Class that represents an Chess Move
    /// </summary>
    public class Move
    {
        #region Properties and Members

        /// <summary>
        /// Tags for the x-Axis
        /// </summary>
        private readonly char[] xCoordTag = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };

        /// <summary>
        /// Tags for the y-Axis
        /// </summary>
        private readonly char[] yCoordTag = { '1', '2', '3', '4', '5', '6', '7', '8' };

        /// <summary>
        /// Tags for the Pieces
        /// </summary>
        private readonly char[] pieceTag = { ' ', 'N', 'B', 'R', 'Q', 'K' };

        /// <summary>
        /// The Piece that was moved
        /// </summary>
        private readonly Piece? PieceMoved;

        /// <summary>
        /// The Color of the the Piece, that was moved, e.q. the Player that performed this move
        /// </summary>
        public Piece.PColor PColor;

        /// <summary>
        /// Starting Cell of the Move
        /// </summary>
        public Cell Start { set; get; }

        /// <summary>
        /// End Cell of the Move
        /// </summary>
        public Cell End { set; get; }

        /// <summary>
        /// A Piece that was killed
        /// </summary>
        public Piece? PieceKilled { set; get; }

        /// <summary>
        /// Flag, if this was the last Move to Checkmate
        /// </summary>
        public bool CheckMateMove { set; get; }   

        /// <summary>
        /// Flag for castling move 
        /// </summary>
        public bool CastlingMove { set; get; }

        /// <summary>
        /// Flag for Promotion move
        /// </summary>
        public bool PromotionMove { set; get; } 

        /// <summary>
        /// In Case of a Promotion Move, the newly created Piece
        /// </summary>
        public Piece.PType PromotionPiece { set; get; }

        /// <summary>
        /// In case of a castling move, the old position of the rook
        /// </summary>
        public Cell? RookLoc { set; get; }
         
        #endregion

        #region Constructor

        /// <summary>
        /// Contructor of the Class
        /// </summary>
        /// <param name="start">Start Cell of the Move</param>
        /// <param name="end">Destination Cell of the Move</param>
        public Move(Cell start, Cell end, Piece.PColor color)
        { 
            Start = start;
            End = end;
            PColor = color;

            Piece? piece = start.GetPiece();
            if(piece != null)
            {
                PieceMoved = start.GetPiece();
                PColor = piece.PieceColor;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the X Distance of the Move in absolute Value
        /// </summary>
        /// <returns>X Distance</returns>
        public int GetXMovement()
        {
            return Math.Abs(Start.Location[0] - End.Location[0]);
        }


        /// <summary>
        /// Returns the Y Distance of the Move in absolute Value
        /// </summary>
        /// <returns>X Distance</returns>
        public int GetYMovement()
        {
            return Math.Abs(Start.Location[1] - End.Location[1]);
        }


        /// <summary>
        /// Returns the Move in UCI Style
        /// </summary>
        /// <returns>Move in UCI Style</returns>
        public string GetUciMoveNaming()
        {
            StringBuilder sb = new StringBuilder();

            // Test for Castling Move
            if( this.CastlingMove && RookLoc != null)
            {
                sb.Append(RookLoc.Location[0] == 0 ? "0-0-0" : "0-0");
                return sb.ToString();
            }

            if( PieceMoved != null )
            {
                sb.Append(pieceTag[(int)PieceMoved.PieceType]);
            }

            // if cyptured piece, add x
            if(PieceKilled != null )
            {
                sb.Append('x');
            }
            
            sb.Append(xCoordTag[End.Location[0]]);
            sb.Append(yCoordTag[End.Location[1]]);

            // If Promotion Move, add = plus the new Piece
            if( this.PromotionMove )
            {
                sb.Append('=');
                sb.Append(pieceTag[(int)this.PromotionPiece]);
            }

            // If Checkmate Move
            if( this.CheckMateMove )
            {
                sb.Append('#');
            }

            return sb.ToString();
        }

        #endregion
    }
}
