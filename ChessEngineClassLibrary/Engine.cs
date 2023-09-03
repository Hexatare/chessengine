using ChessEngineClassLibrary.Models;
using ChessEngineClassLibrary.Pieces;
using System;
using System.Threading;

namespace ChessEngineClassLibrary
{
    /// <summary>
    /// This Class implements the Alpha-Beta-Pruning Algorithm for the Chess Engine
    /// </summary>
    public class Engine
    {
        #region Properties and Members

        /// <summary>
        /// Reference to the Game Class
        /// </summary>
        private readonly Game Game;

        /// <summary>
        /// Reference to the Chessboard
        /// </summary>
        private Board ChessBoard;

        /// <summary>
        /// The players
        /// </summary>
        private Player[] Players;

        /// <summary>
        /// Game Settings
        /// </summary>
        public GameSettings? ActGameSettings { get; set; }

        /// <summary>
        /// The color of the pieces of the engine
        /// </summary>
        private Piece.PColor color;

        /// <summary>
        /// Maximum amount of time the engine has to calculate the best move
        /// </summary>
        private int MaxTime;

        /// <summary>
        /// Property that holds the best move
        /// </summary>
        private Move? BestMove;

        /// <summary>
        /// A boolean indicating if the thread should be terminated
        /// Default is false
        /// </summary>
        private bool terminateThread = false;

        /// <summary>
        /// The maximum depth of the Engine
        /// This is just a safety measure, theoretically the engine should
        /// never hit this because of the time limit
        /// </summary>
        private int maxDepth = 5;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of the Class
        /// </summary>
        /// <param name="game">Reference to the Game Class</param>
        /// <param name="chessBoard">Reference to the Chessboard</param>
        /// <param name="maxTime">Maximum amount of time the engine has to calculate the best move. Defaults to 5000ms</param>
        public Engine(Game game, Board chessBoard, Player[] players, int maxTime=5000)
        {
            Game = game;
            ChessBoard = chessBoard;
            MaxTime = maxTime;
            Players = players;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Method that starts a new Game, sets the color of the engine
        /// </summary>
        /// <param name="actGameSettings">Game Settings</param>
        public void StartNewGame(GameSettings actGameSettings)
        {
            // Set the Game Settings
            ActGameSettings = actGameSettings;

            // Make sure the ActGameSettings are not null
            if(ActGameSettings != null)
            {
                // Set the color of the engine
                color = ActGameSettings.Color == Piece.PColor.White ? Piece.PColor.White : Piece.PColor.Black;
            }
        }


        /// <summary>
        /// This Method is called when the player has made a move
        /// </summary>
        public void DoMove()
        {
            // Set the terminateThread bool to false
            terminateThread = false;

            // Set the best move to null
            BestMove = null;

            // Create the Iterative Deepening Search in a new Thread
            Thread searchThread = new Thread(new ThreadStart(IterativeDeepeningSearch));

            // Start the search
            searchThread.Start();

            // Wait for 2 seconds ("Thread" in this case is the engine / main thread, not the search thread)
            Thread.Sleep(2000);

            // Set the terminateThread bool to true to stop the search
            terminateThread = true;

            // Join the search thread
            // This is necessary that board is returned to the state before the search
            searchThread.Join();
            
            // Make sure the best move is not null
            if (BestMove == null)
            {
                // If it is null, throw an exception
                throw new ArgumentNullException();
            }

            // Do the best move
            Game.EngineMove(BestMove);
        }


        /// <summary>
        /// Method that uses the iterative deepening search to calculate the best move
        /// All the code really does is call the AlphaBeta method with different depths
        /// and store the best move in the BestMove property
        /// </summary>
        private void IterativeDeepeningSearch()
        {
            // Create a loop that iterates through the depth
            for (int i = 1; i <= maxDepth; i++)
            {
                // Get the best move using the AlphaBeta algorithm
                BestMove = BestMoveUsingAlphaBeta(i, (color == Piece.PColor.Black), -10_000, 10_000);
            }
        }


        /// <summary>
        /// Method to calculate the best move using the AlphaBeta algorithm
        /// </summary>
        /// <param name="depth">Depth level according to the difficulty</param>
        /// <param name="maxValuePlayer">White = true, Black = false</param>
        /// <param name="alpha">Value for Alpha</param>
        /// <param name="beta">Value for Beta</param>
        /// <returns></returns>
        private Move BestMoveUsingAlphaBeta(int depth, bool maxValuePlayer, int alpha, int beta)
        {
            // Set the best move score to a very low value
            int bestMoveScore = -1_000_000;

            // Set the best move to null
            Move? bestMove = null;

            // Loop through all the possible moves
            foreach (Move possibleMove in ChessBoard.GetAllPossibleMoves(color))
            {
                // Do the move on the board
                ChessBoard.DoMove(possibleMove);

                // Get the score of the move
                int moveScore = AlphaBeta(depth, depth, maxValuePlayer, alpha, beta);

                // See if the score is better than the best move score
                int score = Math.Max(bestMoveScore, moveScore);

                // Undo the move
                ChessBoard.UndoMove(possibleMove);

                // Check if the score is better than the best move score
                if (score > bestMoveScore)
                {
                    // If it is, set the best move score to the score
                    bestMoveScore = score;

                    // Set the best move to the possible move
                    bestMove = possibleMove;
                }

                // Check if the thread should be terminated
                if (terminateThread)
                {
                    // If it should, break out of the loop
                    break;
                }
            }

            // Return the best move
            return bestMove;
        }

        #endregion

        #region Algorithm

        /// <summary>
        /// Evaluates the overall value on the board, white Pieces count positively, black pieces count negatively
        /// </summary>
        /// <returns>Integer representing how good the board is</returns>
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
        /// The Alpha-Beta-Pruning algorithm to calculate the possible best move
        /// </summary>
        /// <param name="maxDepth"></param>
        /// <param name="currentDepth"></param>
        /// <param name="maxValuePlayer"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <returns></returns>
        private
            int AlphaBeta(int maxDepth, int currentDepth, bool maxValuePlayer, int alpha, int beta)
        {
            // Check if the current depth is 0
            if (currentDepth == 0)
            {
                // If it is, return the evaluation of the board
                return EvaluateBoard();
            }

            // Evaluate for max Player
            if (maxValuePlayer)
            {
                // Set the best score to a very low value
                int bestScore = -100000;

                // Loop through all the possible moves
                foreach (Move possibleMove in ChessBoard.GetAllPossibleMoves(color))
                {
                    // Do the move on the board
                    ChessBoard.DoMove(possibleMove);

                    // calculating node score, if the current node will be the leaf node, then score will be
                    // calculated by static evaluation;
                    // score will be calculated by finding max value between node score and current best score.
                    int nodeScore = AlphaBeta(maxDepth, currentDepth - 1, false, alpha, beta);

                    bestScore = Math.Max(bestScore, nodeScore);

                    // undoing the last move, so as to explore new moves while backtracking
                    ChessBoard.UndoMove(possibleMove);

                    // calculating alpha for current MAX node
                    alpha = Math.Max(alpha, bestScore);

                    // Cut off beta
                    if (beta <= alpha)
                        return bestScore;

                }
                return bestScore;
            }

            // Evaluate for min Player
            else
            {
                // Set the best score to a very high value
                int bestScore = 100000;

                // Loop through all the possible moves
                foreach (Move possibleMove in ChessBoard.GetAllPossibleMoves(color))
                {
                    // Do the move on the board
                    ChessBoard.DoMove(possibleMove);

                    // calculating node score, if the current node will be the leaf node, then score will be
                    // calculated by static evaluation;
                    // score will be calculated by finding min value between node score and current best score.
                    int nodeScore = AlphaBeta(maxDepth, currentDepth - 1, true, alpha, beta);

                    bestScore = Math.Min(bestScore, nodeScore);
                    ChessBoard.UndoMove(possibleMove);

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