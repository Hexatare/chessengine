using ChessEngineClassLibrary.Pieces;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ChessEngineClassLibrary
{
    public class Player
    {
        #region Properties and Members        

        /// <summary>
        /// Referenz to the Game Object
        /// </summary>
        private Game Game; 

        /// <summary>
        /// Timer for the total time played 
        /// </summary>
        private Stopwatch Stopwatch;

        /// <summary>
        /// List of all performed Moves in the current game, by this player
        /// </summary>
        private List<Move> Moves;

        /// <summary>
        /// Name of the player
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// Color of this player (White or Black)
        /// </summary>
        public Piece.PColor Color { get; set; }

        /// <summary>
        /// Kingside castling availability.
        /// </summary>
        public bool CanKingsideCastle { get; set; }

        /// <summary>
        /// Queenside castling availability.
        /// </summary>
        public bool CanQueensideCastle { get; set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">Referenz to the Game Object</param>
        public Player(Game game) 
        {
            // Referenz to the Game Class
            Game = game;

            // Liste of all Moves
            Moves = new List<Move>();
            
            // Create a timer with an interval of 1000 milliseconds (1 second)
            Stopwatch = new Stopwatch();
        }


        /// <summary>
        /// Start a new Game
        /// </summary>
        /// <param name="color">Color of this Player</param>
        public void NewGame(Piece.PColor color)
        { 
            // Clear the List of all Moves
            Moves.Clear();

            // Reset the Time played 
            Stopwatch.Reset();

            // Castling possible
            CanKingsideCastle = CanQueensideCastle = true;
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
        /// Returns the last Move from this Player and removes this Move from the History
        /// </summary>
        /// <returns>Last Move</returns>
        public Move GetLastMove()
        {
            Move lastMove = Moves.Last();
            Moves.RemoveAt(Moves.Count - 1);
            return lastMove;
        }


        /// <summary>
        /// Sets this Player as the current Player to make a move. This starts the Timer
        /// </summary>
        /// <param name="color">Color of the Player with the next move</param>
        public void SetCurrentPlayer(Piece.PColor color)
        { 
            if(color.Equals(Color)) 
            {
                // Start the Timer
                Stopwatch.Start();
            }
            else
            {
                // Stop the Timer
                Stopwatch.Stop();
            }

        }


        /// <summary>
        /// Total Time the Player has used
        /// </summary>
        /// <returns>Total Time in Hours, Minutes and Seconds</returns>
        public string TimePlayed()
        {
            return string.Format("{0:hh\\:mm\\:ss}", Stopwatch.Elapsed);
        }
    }
}
