using ChessEngineClassLibrary.Pieces;
using System;

namespace ChessEngineClassLibrary.Models
{
    /// <summary>
    /// Data Class with Settings for one Game
    /// </summary>
    public class GameSettings
    {

        /// <summary>
        /// The current Game Mode
        /// </summary>
        public GameMode Mode { get; set; }

        /// <summary>
        /// The Color of the Player, e.q. Computer has opposit color
        /// </summary>
        public Piece.PColor Color { get; set; }

        /// <summary>
        /// Difficulty when in Computer Mode
        /// </summary>
        public Difficulty Difficulty { get; set; }

        /// <summary>
        /// Aviable Time for the Game
        /// </summary>
        public GameTime TimePlay { get; set; }

        /// <summary>
        /// Max Time for each Player
        /// </summary>
        public TimeSpan MaxTime { get; set; }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mode">The mode of the Game</param>
        /// <param name="color">The Color of the Player</param>
        /// <param name="difficulty">The Difficulty in Computer Mode</param>
        /// <param name="timePlay">The aviable Time for the Game</param>
        public GameSettings(GameMode mode, Piece.PColor color, Difficulty difficulty, GameTime timePlay)
        {
            Mode = mode;
            Color = color;
            Difficulty = difficulty;
            TimePlay = timePlay;

            MaxTime = new TimeSpan(0, (int)TimePlay, 0);
        }

    }
}
