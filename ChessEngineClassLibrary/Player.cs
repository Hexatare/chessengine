using ChessEngineClassLibrary.Pieces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static ChessEngineClassLibrary.Pieces.Piece;

namespace ChessEngineClassLibrary
{
    /// <summary>
    /// The player class implements the functionality of a Player
    /// </summary>
    public class Player
    {
        #region Properties and Members        

        /// <summary>
        /// A reference to the game
        /// </summary>
        private Game Game;

        /// <summary>
        /// Timer for the total time played 
        /// </summary>
        private Stopwatch Stopwatch;

        /// <summary>
        /// List of all the performed moves in the current game made by this player
        /// </summary>
        private List<Move> Moves;

        /// <summary>
        /// The name of this player
        /// </summary>
        public string Name { set; get; } = string.Empty;

        /// <summary>
        /// Color of this player (either White or Black)
        /// </summary>
        public Piece.PColor Color { get; set; }

        /// <summary>
        /// Count of Half-Moves made by the Player
        /// </summary>
        public int NbrOfHalfMoves { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// The construtor for the player
        /// </summary>
        /// <param name="game">Reference to the Game Object</param>
        public Player(Game game)
        {
            Game = game;
            Moves = new List<Move>();
            Stopwatch = new Stopwatch();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Method to start a new Game
        /// </summary>
        /// <param name="color">The color of the Player</param>
        public void NewGame(Piece.PColor color)
        {
            // Clear the List of all Moves
            Moves.Clear();

            // Reset the Time played 
            Stopwatch.Reset();
        }


        /// <summary>
        /// Adds a Move from this Player to the History
        /// </summary>
        /// <param name="move"></param>
        public void AddMove(Move move)
        {
            Moves.Add(move);
        }


        /// <summary>
        /// Returns the last Move from this Player and removes it from the History
        /// </summary>
        /// <param name="remove">Boolean indicating whether the move should be removed from the history</param>
        /// <returns>The last Move</returns>
        public Move GetLastMove(bool remove)
        {
            if (Moves.Count == 0)
                return null;

            Move lastMove = Moves.Last();

            if (remove)
                Moves.RemoveAt(Moves.Count - 1);

            return lastMove;
        }


        /// <summary>
        /// Returns the Number of Moves of this Player
        /// </summary>
        /// <returns>The number of moves</returns>
        public int GetNbrOfMoves()
        {
            return Moves.Count;
        }


        /// <summary>
        /// Returns a List of all captures Pieces in the corresponding sequence
        /// </summary>
        /// <returns>A list of all captured pieces</returns>
        public List<string> GetAllCapturedPieces()
        {
            List<string> pieces = new List<string>();

            foreach (Move move in Moves)
            {
                if (move.PieceKilled != null)
                {
                    pieces.Add("Zug: " + move.GetUciMoveNaming() + " - "
                        + Enum.GetName(typeof(PType), (int)move.PieceKilled.PieceType));
                }
            }
            return pieces;
        }


        /// <summary>
        /// Sets this Player as the current Player to make a move. This starts the Timer
        /// </summary>
        /// <param name="color">Color of the Player with the next move</param>
        public void SetCurrentPlayer(Piece.PColor color)
        {
            if (color.Equals(Color))
            {
                Stopwatch.Start();
            }
            else
            {
                Stopwatch.Stop();
            }
        }


        /// <summary>
        /// Total Time the Player has used
        /// </summary>
        /// <returns>Total Time in Hours, Minutes and Seconds</returns>
        public TimeSpan TimePlayed()
        {
            return Stopwatch.Elapsed;
        }


        /// <summary>
        /// Set the End of the Game 
        /// </summary>
        public void SetEndGame()
        {
            Stopwatch.Stop();
        }

        #endregion
    }
}
