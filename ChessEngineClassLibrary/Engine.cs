using ChessEngineClassLibrary.Models;
using System;

namespace ChessEngineClassLibrary
{
    /// <summary>
    /// This Class implements the MinMax Algorithmen for the Chess Play
    /// </summary>
    public class Engine
    {

        #region Properties and Members

        /// <summary>
        /// Referenz to the Game Class
        /// </summary>
        private Game Game;

        /// <summary>
        /// Referenz to the Chessboard
        /// </summary>
        private Board ChessBoard;

        /// <summary>
        /// The Players
        /// </summary>
        private Player[] Players;

        /// <summary>
        /// Current Difficulty Setting for the Game
        /// </summary>
        public GameSettings ActGameSettings { set; get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of the Class
        /// </summary>
        /// <param name="game">Reference to the Game Class</param>
        /// <param name="chessBoard">Reference to the Chessboard</param>
        /// <param name="players">Referenz to the Players</param>
        public Engine(Game game, Board chessBoard, Player[] players)
        {
            Game = game;
            ChessBoard = chessBoard;
            Players = players;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Method that starts a new Game
        /// </summary>
        /// <param name="difficulty">Selected Difficultiy</param>
        /// <param name="actColor">Color of the Computer</param>
        public void StartNewGame(GameSettings actGameSettings)
        {
            ActGameSettings = actGameSettings;

            // Initialize the Algorithmen

        }


        /// <summary>
        /// This Method is called, when Human Player has performed his / her move
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void DoMove()
        {
            throw new NotImplementedException();
        }


        #endregion

    }
}
