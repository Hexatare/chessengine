using ChessEngineClassLibrary;
using ChessEngineClassLibrary.Models;
using ChessEngineClassLibrary.Pieces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Policy;
using System.Threading;
using static System.Formats.Asn1.AsnWriter;

namespace ChessEngineClassLibrary
{
    /// <summary>
    /// This Class implements the MinMax Algorithmen for the Chess Play
    /// </summary>
    public class Engine
    {

        #region Properties and Members

        /// <summary>
        /// Algorithm Calc Depth, according to Grade of Difficulty
        /// </summary>
        private enum CalcDepth
        {
            Easy = 2,
            Medium = 3,
            Hard = 4
        };


        /// <summary>
        /// Referenz to the Game Class
        /// </summary>
        private readonly Game Game;

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
        public GameSettings? ActGameSettings { set; get; }

        /// <summary>
        /// The color of the engine
        /// </summary>
        private Piece.PColor color;

        /// <summary>
        /// The calculation depth of the Algorithm
        /// </summary>
        private int calDepth = 2;

        /// <summary>
        /// Dictionary for recording reasons only
        /// </summary>
        private Dictionary<int, int> nbrOfNodesPerDepth = new Dictionary<int, int>();

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
            if(ActGameSettings != null)
            {
                if (ActGameSettings.Color == Piece.PColor.White)
                    color = Piece.PColor.White;
                else
                    color = Piece.PColor.Black;

                // Setting the Depth according to the difficulty
                switch( ActGameSettings.Difficulty )
                {
                    case Difficulty.Easy:
                        this.calDepth = (int)CalcDepth.Easy;
                        break;

                        case Difficulty.Intermediate: 
                        this.calDepth = (int)CalcDepth.Medium; 
                        break;

                    case Difficulty.Hard: 
                        this.calDepth = (int)CalcDepth.Hard; 
                        break; 
                }
            }
        }


        /// <summary>
        /// This Method is called, when Human Player has performed his / her move
        /// </summary>
        public void DoMove()
        {
            // Thread soll kurz warten
            Thread.Sleep(500);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // if difficutly is ease, use minmax, otherwise alpha beta
            Move? bestMove;
            if( this.calDepth > (int)CalcDepth.Easy)
            {
                // get the best move from Alpha Beta
                bestMove = BestMoveUsingAlphaBeta(calDepth, (color == Piece.PColor.Black), -10000, 10000);
            }
            else
            {
                // get the best move from MinMax
                bestMove = BestMoveUsingMinMax(calDepth, (color == Piece.PColor.Black));
            }

            stopwatch.Stop();
            
            Game.EngineMove(bestMove);

            // Print out Debug Values
            Debug.WriteLine("Beste Move: " + bestMove.GetUciMoveNaming());
            Debug.WriteLine("Time for calculation: " + stopwatch.Elapsed);
            Debug.WriteLine("Nodes calculatet " + nbrOfNodesPerDepth.Keys.Count);
            foreach(int key in nbrOfNodesPerDepth.Keys)
            {
                Debug.WriteLine("Depth: " + key + " Value: " + nbrOfNodesPerDepth[key]);
            }
        }


        /// <summary>
        /// Method to calculate the best move using MinMax Algorithm
        /// </summary>
        /// <param name="depth">Depth level according to the difficulty</param>
        /// <param name="maxValuePlayer">White = true, Black = false</param>
        /// <returns></returns>
        private Move BestMoveUsingMinMax(int depth, bool maxValuePlayer)
        {
            int bestMoveScore = -1000000;
            Move? bestMove = null;

            foreach (Move possibleMoves in ChessBoard.GetAllPossibleMoves(color))
            {
                // Do the move on the board
                ChessBoard.DoMove(possibleMoves);

                int moveScore = MinMax(depth, depth, maxValuePlayer, nbrOfNodesPerDepth);

                int score = Math.Max(bestMoveScore, moveScore);
                ChessBoard.UndoMove(possibleMoves);

                if (score > bestMoveScore)
                {
                    bestMoveScore = score;
                    bestMove = possibleMoves;
                }
            }

            return bestMove;
        }


        /// <summary>
        /// Method to calculate the best move using Alpha-Beta Algorithm
        /// </summary>
        /// <param name="depth">Depth level according to the difficulty</param>
        /// <param name="maxValuePlayer">White = true, Black = false</param>
        /// <param name="alpha">Value for Alpha</param>
        /// <param name="beta">Value for Beta</param>
        /// <returns></returns>
        private Move BestMoveUsingAlphaBeta(int depth, bool maxValuePlayer, int alpha, int beta)
        {
            int bestMoveScore = -1000000;
            Move? bestMove = null;

            foreach (Move possibleMoves in ChessBoard.GetAllPossibleMoves(color))
            {
                // Do the move on the board
                ChessBoard.DoMove(possibleMoves);

                int moveScore = AlphaBeta(depth, depth, maxValuePlayer, alpha, beta, nbrOfNodesPerDepth);

                int score = Math.Max(bestMoveScore, moveScore);
                ChessBoard.UndoMove(possibleMoves);


                if (score > bestMoveScore)
                {
                    bestMoveScore = score;
                    bestMove = possibleMoves;
                }
            }
            return bestMove;
        }

        #endregion

        #region Algorithm

        /// <summary>
        /// Evaluates the overall value on the board, white Pieces count positiv, black pieces count negativ
        /// </summary>
        /// <returns></returns>
        private int EvaluateBoard()
        {
            int evaluationValue = 0;

            foreach (Cell? cell in ChessBoard.GetCells())
            {
                if (!cell.IsEmpty)
                { 
                    if( cell.GetPiece() !=  null )
                    {
                        int pieceValue = GetPieceValue(cell.GetPiece()!.PieceType);

                        evaluationValue += cell.GetPiece()!.PieceColor == Piece.PColor.White ? pieceValue : -pieceValue;
                    }
                }
            }
            return evaluationValue;
        }


        /// <summary>
        /// Returns the value of a specific Piece
        /// </summary>
        /// <param name="pType">Type of Piece</param>
        /// <returns>Value of the type of Piece</returns>
        private int GetPieceValue(Piece.PType pType)
        {
            int pieceValue = 0;

            switch (pType)
            {
                case Piece.PType.Pawn: pieceValue = 10; break;
                case Piece.PType.Knight: pieceValue = 30; break;
                case Piece.PType.Bishop: pieceValue = 30; break;
                case Piece.PType.Rook: pieceValue = 50; break;
                case Piece.PType.Queen: pieceValue = 90; break;
                case Piece.PType.King: pieceValue = 900; break;

                default:pieceValue = 0;  break;
            }

            if (Game.CurrentPlayer == Piece.PColor.White)
            {
                return pieceValue;
            }
            else
            {
                return -pieceValue;
            }
        }


        /// <summary>
        /// The MinMax Algoritm to calculate the best Move
        /// </summary>
        /// <param name="maxDepth"></param>
        /// <param name="currentDepth"></param>
        /// <param name="maxValuePlayer"></param>
        /// <param name="nodesPerDepth"></param>
        /// <returns></returns>
        private int MinMax(int maxDepth, int currentDepth, bool maxValuePlayer, Dictionary<int, int> nodesPerDepth)
        {
            // Just for Debug - Reason, store the number of nodes for each depth
            if (nbrOfNodesPerDepth.ContainsKey(maxDepth - currentDepth))
                nbrOfNodesPerDepth[maxDepth - currentDepth] += 1;
            else
                nbrOfNodesPerDepth[maxDepth - currentDepth] = 1;

            if (currentDepth == 0)
            {
                return EvaluateBoard();
            }

            // Evaluate for max Player
            if (maxValuePlayer)
            {
                int bestScore = -100000;

                foreach (Move possibleMoves in ChessBoard.GetAllPossibleMoves(color))
                {
                    // Do the move on the board
                    ChessBoard.DoMove(possibleMoves);

                    int nodeScore = MinMax(maxDepth, currentDepth - 1, false, nodesPerDepth);

                    bestScore = Math.Max(bestScore, nodeScore);

                    ChessBoard.UndoMove(possibleMoves);
                }
                return bestScore;
            }
            else
            {
                int bestScore = 100000;

                foreach (Move possibleMoves in ChessBoard.GetAllPossibleMoves(color))
                {
                    // Do the move on the board
                    ChessBoard.DoMove(possibleMoves);

                    int nodeScore = MinMax(maxDepth, currentDepth - 1, true, nodesPerDepth);

                    bestScore = Math.Min(bestScore, nodeScore);
                    ChessBoard.UndoMove(possibleMoves);
                }
                return bestScore;
            }
        }


        /// <summary>
        /// The Alpha, Beta Pruning Algorithm to calculate the possible best move
        /// </summary>
        /// <param name="maxDepth"></param>
        /// <param name="currentDepth"></param>
        /// <param name="maxValuePlayer"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="nodesPerDepth"></param>
        /// <returns></returns>
        private int AlphaBeta(int maxDepth, int currentDepth, bool maxValuePlayer, int alpha, int beta, Dictionary<int, int> nodesPerDepth)
        {
            // Just for Debug - Reason, store the number of nodes for each depth
            if (nbrOfNodesPerDepth.ContainsKey(maxDepth - currentDepth))
                nbrOfNodesPerDepth[maxDepth - currentDepth] += 1;
            else
                nbrOfNodesPerDepth[maxDepth - currentDepth] = 1;

            if (currentDepth == 0)
            {
                return EvaluateBoard();
            }

            // Evaluate for max Player
            if (maxValuePlayer)
            {
                int bestScore = -100000;

                foreach (Move possibleMoves in ChessBoard.GetAllPossibleMoves(color))
                {
                    // Do the move on the board
                    ChessBoard.DoMove(possibleMoves);

                    // calculating node score, if the current node will be the leaf node, then score will be
                    // calculated by static evaluation;
                    // score will be calculated by finding max value between node score and current best score.
                    int nodeScore = AlphaBeta(maxDepth, currentDepth - 1, false, alpha, beta, nodesPerDepth);

                    bestScore = Math.Max(bestScore, nodeScore);

                    // undoing the last move, so as to explore new moves while backtracking
                    ChessBoard.UndoMove(possibleMoves);

                    // calculating alpha for current MAX node
                    alpha = Math.Max(alpha, bestScore);

                    // Cut off beta
                    if (beta <= alpha)
                        return bestScore;

                }
                return bestScore;
            }
            else
            {
                int bestScore = 100000;

                foreach (Move possibleMoves in ChessBoard.GetAllPossibleMoves(color))
                {
                    // Do the move on the board
                    ChessBoard.DoMove(possibleMoves);

                    // calculating node score, if the current node will be the leaf node, then score will be
                    // calculated by static evaluation;
                    // score will be calculated by finding min value between node score and current best score.
                    int nodeScore = AlphaBeta(maxDepth, currentDepth - 1, true, alpha, beta, nodesPerDepth);

                    bestScore = Math.Min(bestScore, nodeScore);
                    ChessBoard.UndoMove(possibleMoves);

                    // calculating alpha for current MIN node
                    beta = Math.Min(beta, bestScore);

                    // beta cut off
                    if (beta <= alpha)
                        return bestScore;
                }
                return bestScore;
            }
        }

        #endregion
    }
}