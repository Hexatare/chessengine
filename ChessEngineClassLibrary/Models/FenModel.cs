using ChessEngineClassLibrary.Pieces;
using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace ChessEngineClassLibrary.Models
{
    /// <summary>
    /// Class to hold a Model of a FEN String to Setup a Chess Play
    /// This code was written by Alex Fredrickson and adapted for this purpose
    /// URL: https://github.com/alexqfredrickson/FenParser/tree/master
    /// </summary>
    public class FenModel
    {
        #region Fields
        /// <summary>
        /// The color of the active player.
        /// </summary>
        public Piece.PColor ActivePlayerColor;

        /// <summary>
        /// The square which is currently available for "en passant" capture ('-' if a square is not available).  
        /// </summary>
        public string EnPassantSquare { get; set; }

        /// <summary>
        /// A list of ranks (reversed; from rank #8 to rank #1)
        /// </summary>
        public string[][] Ranks = new string[8][];

        /// <summary>
        /// White's kingside castling availability.
        /// </summary>
        public bool[] CanKingsideCastle { get; set; }

        /// <summary>
        /// White's queenside castling availability.
        /// </summary>
        public bool[] CanQueensideCastle { get; set; }

        /// <summary>
        /// The game's halfmove counter, used to determine a draw.
        /// </summary>
        public int HalfMoveCounter { get; set; }

        /// <summary>
        /// The game's move number.
        /// </summary>
        public int FullMoveNumber { get; set; }

        #endregion

        #region Construtors

        /// <summary>
        /// Constructor of the Calss
        /// </summary>
        /// <param name="piecePlacementString"></param>
        /// <param name="activeColorString"></param>
        /// <param name="castlingAvailabilityString"></param>
        /// <param name="enPassantSquareString"></param>
        /// <param name="halfmoveClockString"></param>
        /// <param name="fullmoveNumberString"></param>
        public FenModel(string piecePlacementString, string activeColorString, string castlingAvailabilityString,
                string enPassantSquareString, string halfmoveClockString, string fullmoveNumberString)
        {
            ParseRanks(piecePlacementString);
            ParseActiveColor(activeColorString);
            ParseCastlingAvailability(castlingAvailabilityString);
            ParseEnPassantSquare(enPassantSquareString);
            ParseHalfMoveCounter(halfmoveClockString);
            ParseFullmoveNumber(fullmoveNumberString);
        }

        #endregion


        #region Methods
        /// <summary>
        /// Parses a FEN substring containing piece placement data into a matrix of ranks.
        /// </summary>
        /// <param name="piecePlacementString"></param>
        /// <returns></returns>
        private void ParseRanks(string piecePlacementString)
        {
            string[] piecePlacementRanksArray = piecePlacementString.Split('/');

            for (int i = 0; i < piecePlacementRanksArray.Length; i++)
            {
                piecePlacementRanksArray[i] = SanitizeRank(piecePlacementRanksArray[i]);
            }

            for (int i = 0; i < piecePlacementRanksArray.Length; i++)
            {
                string[] allRanks = Array.ConvertAll(piecePlacementRanksArray[i].ToCharArray(), x => x.ToString());
                string[] newRank = new string[8];

                for (int j = 0; j < allRanks.Length; j++)
                {
                    string thisSquare = allRanks[j];

                    int n;
                    if (int.TryParse(thisSquare, out n))
                    {
                        int nullSquareCount = int.Parse(thisSquare);

                        for (int k = 0; k < nullSquareCount; k++)
                        {
                            newRank[j] = String.Empty;
                        }
                    }
                    else
                    {
                        newRank[j] = thisSquare;
                    }
                }

                Ranks[i] = newRank;
            }
        }

        /// <summary>
        /// Sanitizes a rank string by replacing instances of integers with same-length blank substrings.
        /// </summary>
        /// <returns></returns>
        private string SanitizeRank(string rank)
        {
            Regex r = new Regex(@"[\d]+");
            Match m = r.Match(rank);

            while (m.Success)
            {
                StringBuilder sb = new StringBuilder(rank);

                int index = m.Index;
                int nullSquareCount = int.Parse(m.Value);
                string newSubstring = String.Empty;

                for (int j = 0; j < nullSquareCount; j++)
                {
                    newSubstring += " ";
                }

                sb.Remove(index, 1);
                sb.Insert(index, newSubstring);
                rank = sb.ToString();
                m = r.Match(rank);
            }

            return rank;
        }

        /// <summary>
        /// Parses a FEN substring containing active player data.
        /// </summary>
        /// <param name="activeColorSubstring"></param>
        /// <returns></returns>
        private void ParseActiveColor(string activeColorSubstring)
        {
            if (activeColorSubstring.ToLower() == "b")
            {
                ActivePlayerColor = Piece.PColor.Black;
            }
            else if (activeColorSubstring.ToLower() == "w")
            {
                ActivePlayerColor = Piece.PColor.White;
            }
        }

        /// <summary>
        /// Parses a FEN substring containing black's and white's queenside/kingside castling availability.
        /// </summary>
        /// <param name="castlingAvailabilityString"></param>
        private void ParseCastlingAvailability(string castlingAvailabilityString)
        {
            CanKingsideCastle = new bool[] { false, false };
            CanQueensideCastle = new bool[] { false, false };

            if (castlingAvailabilityString == "-")
            {
                return; // nobody can castle
            }
            else
            {
                if (castlingAvailabilityString.Contains("K"))
                {
                    CanKingsideCastle[(int)Piece.PColor.White] = true;
                }

                if (castlingAvailabilityString.Contains("k"))
                {
                    CanKingsideCastle[(int)Piece.PColor.Black] = true;
                }

                if (castlingAvailabilityString.Contains("Q"))
                {
                    CanQueensideCastle[(int)Piece.PColor.White] = true;
                }

                if (castlingAvailabilityString.Contains("q"))
                {
                    CanQueensideCastle[(int)Piece.PColor.Black] = true;
                }
            }
        }

        /// <summary>
        /// Parses a FEN substring containing data about the potentially eligible en-passant square.
        /// </summary>
        /// <param name="enPassantSquareString"></param>
        private void ParseEnPassantSquare(string enPassantSquareString)
        {
            if (enPassantSquareString == "-")
            {
                EnPassantSquare = String.Empty;
            }
            else if (!String.IsNullOrEmpty(enPassantSquareString))
            {
                EnPassantSquare = enPassantSquareString;
            }
        }

        /// <summary>
        /// Parses a FEN substring containing data about the halfmove counter.
        /// </summary>
        /// <param name="halfmoveClockString"></param>
        private void ParseHalfMoveCounter(string halfmoveClockString)
        {
            HalfMoveCounter = int.Parse(halfmoveClockString);
        }

        /// <summary>
        /// Parses a FEN substring containing data about the move number.
        /// </summary>
        /// <param name="fullmoveNumberString"></param>
        private void ParseFullmoveNumber(string fullmoveNumberString)
        {
            FullMoveNumber = int.Parse(fullmoveNumberString);
        }

        /// <summary>
        /// Prints a graphical representation of the board state to the console.
        /// </summary>
        public void ToConsole()
        {
            Debug.WriteLine("-----------------");

            for (int i = 0; i < 8; i++)
            {
                string[] currentRank = Ranks[i];

                Debug.WriteLine(
                      "|" + currentRank[0] + "|" + currentRank[1] + "|" + currentRank[2] + "|" + currentRank[3]
                    + "|" + currentRank[4] + "|" + currentRank[5] + "|" + currentRank[6] + "|" + currentRank[7] + "|");
                Debug.WriteLine("-----------------");
            }

            Debug.WriteLine("");
            Debug.WriteLine("The active player is " + this.ActivePlayerColor + ".");

            if (this.CanKingsideCastle[(int)Piece.PColor.White] && this.CanQueensideCastle[(int)Piece.PColor.White])
            {
                Debug.Write("White can castle both king-side and queen-side, and ");
            }
            else if (this.CanKingsideCastle[(int)Piece.PColor.White])
            {
                Debug.Write("White can only castle on the king-side, and ");
            }
            else if (this.CanQueensideCastle[(int)Piece.PColor.White])
            {
                Debug.Write("White can only castle on the queen-side, and ");
            }
            else
            {
                Debug.Write("White cannot castle, and ");
            }

            if (this.CanKingsideCastle[(int)Piece.PColor.Black] && this.CanQueensideCastle[(int)Piece.PColor.Black])
            {
                Debug.Write("Black can castle both king-side and queen-side.");
            }
            else if (this.CanKingsideCastle[(int)Piece.PColor.Black])
            {
                Debug.Write("Black can only castle on the king-side.");
            }
            else if (this.CanQueensideCastle[(int)Piece.PColor.Black])
            {
                Debug.Write("Black can only castle on the queen-side.");
            }
            else
            {
                Debug.Write("Black cannot castle.");
            }

            Debug.WriteLine("");

            if (String.IsNullOrEmpty(this.EnPassantSquare))
            {
                Debug.WriteLine("There are no squares eligible for en-passant capture.");
            }
            else
            {
                Debug.WriteLine(this.EnPassantSquare + " is eligible for en-passant capture.");
            }

            if (this.HalfMoveCounter != null)
            {
                if (this.HalfMoveCounter == 1)
                {
                    Debug.WriteLine("There has been 1 halfmove since the last pawn advance or capture.");
                }
                else if (this.HalfMoveCounter == 0)
                {
                    Debug.WriteLine("This is the first halfmove since the last pawn advance or capture.");
                }
                else
                {
                    Debug.WriteLine("There have been " + this.HalfMoveCounter + " moves since the last pawn advance or capture.");
                }
            }

            if (this.FullMoveNumber != null)
            {
                if (this.FullMoveNumber == 1)
                {
                    Debug.WriteLine("This is the first move.");
                }
                else
                {
                    Debug.WriteLine("This is move number " + this.FullMoveNumber + ".");
                }
            }

        }

        #endregion

     
    }
}
