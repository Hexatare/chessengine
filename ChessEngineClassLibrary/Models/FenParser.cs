using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace ChessEngineClassLibrary.Models
{
    public class FenParser
    {

        #region Fields

        /// <summary>
        /// The unparsed FEN string.
        /// </summary>
        private string Fen { get; set; }

        /// <summary>
        /// A FEN substring representing the position of the white and black pieces on the board.
        /// </summary>
        private string PiecePlacementString { get; set; }

        /// <summary>
        /// A FEN substring representing the active player.
        /// </summary>
        private string ActiveColorString { get; set; }

        /// <summary>
        /// A FEN substring representing the availability of castling for each player.
        /// </summary>
        private string CastlingAvailabilityString { get; set; }

        /// <summary>
        /// A FEN substring representing the square which is currently available for "en passant" capture ('-' if a square is not available).  
        /// </summary>
        private string EnPassantSquareString { get; set; }

        /// <summary>
        /// A FEN substring representing the number of half moves since the last pawn advance or piece capture (used to determine stalemate).
        /// </summary>
        private string HalfmoveClockString { get; set; }

        /// <summary>
        /// A FEN substring representing the game turn (incremented after Black moves).
        /// </summary>
        private string FullmoveNumberString { get; set; }

        /// <summary>
        /// Model of the current board state
        /// </summary>
        public FenModel BoardStateData { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Method to parse a FEN String splitted up it in its substrings
        /// </summary>
        /// <param name="fenSubstrings">splitted FEN String</param>
        public void ParseFenSubstrings(string[] fenSubstrings)
        {
            PiecePlacementString = fenSubstrings[0];
            ActiveColorString = fenSubstrings[1];
            CastlingAvailabilityString = fenSubstrings[2];
            EnPassantSquareString = fenSubstrings[3];
            HalfmoveClockString = fenSubstrings[4];
            FullmoveNumberString = fenSubstrings[5];
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public FenParser()
        {
            PiecePlacementString = string.Empty;
            ActiveColorString = string.Empty;
            CastlingAvailabilityString = string.Empty;
            EnPassantSquareString = string.Empty;
            HalfmoveClockString = string.Empty;
            FullmoveNumberString = string.Empty;
        }

        /// <summary>
        /// Constructor with FEN String
        /// </summary>
        /// <param name="fen">FEN String with board state</param>
        public FenParser(string fen) : this()
        {
            Fen = fen;
            ParseFenSubstrings(Fen.Split(' '));

            BoardStateData = new FenModel(PiecePlacementString, ActiveColorString, 
                                          CastlingAvailabilityString, EnPassantSquareString, 
                                          HalfmoveClockString, FullmoveNumberString);
        }
        #endregion
    }
}
