using ChessEngineClassLibrary.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChessEngineClassLibrary.Models
{
    /// <summary>
    /// Class to generate a FEN String form the current state of the Game
    /// </summary>
    public class FenGenerator
    {
        /// <summary>
        /// Referenz to the Chessboard
        /// </summary>
        private Board ChessBoard;

        /// <summary>
        /// The Players
        /// </summary>
        private Player[] Players;

        /// <summary>
        /// Color of the curren Player
        /// </summary>
        private Piece.PColor CurrentPlayer;

        /// <summary>
        /// Number of half Moves done
        /// </summary>
        private int HalfMoveCounter;

        /// <summary>
        /// Number of FullMoves done
        /// </summary>
        private int FullMoveNumber;

        /// <summary>
        /// White's kingside castling availability.
        /// </summary>
        private bool[] CanKingsideCastle;

        /// <summary>
        /// White's queenside castling availability.
        /// </summary>
        private bool[] CanQueensideCastle;

        /// <summary>
        /// Constructor of the Class
        /// </summary>
        /// <param name="chessBoard">Reference to the Chee Board</param>
        /// <param name="players">Reference to the Playser</param>
        /// <param name="currentPlayer">Color of the current Player</param>
        /// <param name="halfMoveCounter">Number of HalfMoves</param>
        /// <param name="fullMoveNumber">Number of FullMoves</param>
        public FenGenerator(Board chessBoard, Player[] players, Piece.PColor currentPlayer, int halfMoveCounter, int fullMoveNumber)
        {
            ChessBoard = chessBoard;
            Players = players;
            CurrentPlayer = currentPlayer;
            HalfMoveCounter = halfMoveCounter;
            FullMoveNumber = fullMoveNumber;

            // Setup Castling Information
            CanKingsideCastle = new bool[] { Players[0].CanKingsideCastle, Players[1].CanKingsideCastle };
            CanQueensideCastle = new bool[] { Players[0].CanQueensideCastle, Players[1].CanQueensideCastle };
        }

        /// <summary>
        /// Method to generate the FenString
        /// </summary>
        /// <returns></returns>
        public string GeneratFen()
        {
            string[] boardRows = new string[8];

            StringBuilder sb = new StringBuilder();

            // Parse the Board for a Pieces
            foreach(Cell? cell in ChessBoard.GetCells())
            {
                if (!cell.IsEmpty)
                { 
                    switch( cell.GetPiece().PieceType ) 
                    {
                        case Piece.PType.Pawn:
                            sb.Append( cell.GetPiece().PieceColor ==  Piece.PColor.Black ? 'p' : 'P');
                            break;

                        case Piece.PType.Knight:
                            sb.Append( cell.GetPiece().PieceColor == Piece.PColor.Black ? 'n' : 'N');
                            break;

                        case Piece.PType.Bishop:
                            sb.Append(cell.GetPiece().PieceColor == Piece.PColor.Black ? 'b' : 'B');
                            break;

                        case Piece.PType.Rook:
                            sb.Append( cell.GetPiece().PieceColor ==  Piece.PColor.Black ? 'r' : 'R');
                            break;

                        case Piece.PType.Queen:
                            sb.Append( cell.GetPiece().PieceColor == Piece.PColor.Black ? 'q' : 'Q');
                            break;

                        case Piece.PType.King:
                            sb.Append(cell.GetPiece().PieceColor == Piece.PColor.Black ? 'k' : 'K');
                            break;

                    }
                }
                else
                    sb.Append('-');

                // Row completed
                if (cell.Location[0] == 7)
                {
                    boardRows[cell.Location[1]] = Regex.Replace(sb.ToString(), @"(-)\1+", match => match.Length.ToString());
                    boardRows[cell.Location[1]] = boardRows[cell.Location[1]].Replace('-', '1');
                    sb.Clear();
                }
            }

            // Now add all rows to the string
            for(int i1 = boardRows.Length; i1 > 0; i1--)
            {
                sb.Append(boardRows[i1 - 1]);

                if(i1 > 1)
                    sb.Append('/');
            }
            sb.Append(' ');

            // Add current Player
            sb.Append(CurrentPlayer == Piece.PColor.Black ? 'b' : 'w');
            sb.Append(' ');

            // Add Castling Information
            if (CanKingsideCastle[0] || CanKingsideCastle[1] || CanQueensideCastle[0] || CanQueensideCastle[1])
            {
                if (CanKingsideCastle[0])
                    sb.Append("K");
                if (CanQueensideCastle[0])
                    sb.Append("Q");
                if (CanKingsideCastle[1])
                    sb.Append("k");
                if (CanQueensideCastle[1])
                    sb.Append("q");
            }
            else
                sb.Append('-');
            sb.Append(' ');

            // En Passant Information - currently not implemente
            sb.Append('-');
            sb.Append(' ');

            // Add Half and Fullmove Counter
            sb.Append(HalfMoveCounter.ToString());
            sb.Append(' ');
            sb.Append(FullMoveNumber.ToString());

            // Return the FEN String
            return sb.ToString();
        }
    }
}
