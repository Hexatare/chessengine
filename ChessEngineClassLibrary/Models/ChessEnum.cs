using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClassLibrary.Models
{

    #region Enumerators

    /// <summary>
    /// The Color of one Cell on the Board
    /// </summary>
    public enum CellColor
    {
        White = 0,
        Black = 1
    }

    /// <summary>
    /// Mode of the Game, Default is Human Mode
    /// </summary>
    public enum GameMode
    {
        Human,
        Computer
    }

    /// <summary>
    /// Difficulty of the Game in Computer Mode 
    /// </summary>
    public enum Difficulty
    {
        Easy,
        Intermediate,
        Hard
    }

    /// <summary>
    /// All possible states of the Game
    /// </summary>
    public enum GameState
    {
        None,
        Running,
        Calculating,
        End
    }

    /// <summary>
    /// Resasons for Game Ending
    /// </summary>
    public enum GameEndReason
    {
        None,
        Resignation,
        Stalemate,
        ClockFlagged,
        Aggrement,
        FiftyMoveRule,
        Insufficient_Material,
        TimeOutVsInsufficient_Material,
        Checkmate
    }

    /// <summary>
    /// All possible Time Values for one Play
    /// </summary>
    public enum GameTime
    {
        Min_1 = 1,
        Min_3 = 3,
        Min_5 = 5,
        Min_10 = 10,
        Min_15 = 15,
        Min_30 = 30,
        Min_60 = 60
    }



    #endregion
}
