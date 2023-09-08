using ChessEngineClassLibrary.Models;
using ChessEngineClassLibrary.Pieces;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using static System.Formats.Asn1.AsnWriter;


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
        private int maxDepth = 3;

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
            if (ActGameSettings != null)
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

            /*
            // Wait for 2 seconds ("Thread" in this case is the engine / main thread, not the search thread)
            Thread.Sleep(2000);

            // Set the terminateThread bool to true to stop the search
            terminateThread = true;
            */

            // Join the search thread
            // This is necessary that the board is returned to the state before the search
            searchThread.Join();
            
            // Make sure the best move is not null
            if (BestMove == null)
            {
                // If the best move is null, play a random move
                Game.EngineMove(ChessBoard.GetAllPossibleMoves(color)[new Random().Next(ChessBoard.GetAllPossibleMoves(color).Count)]);
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
                // Check if the thread should be terminated
                if (terminateThread)
                {
                    // If it should, break out of the loop
                    break;
                }

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

            // Get the length of the possible moves
            int possibleMovesLength = ChessBoard.GetAllPossibleMoves(color).Count;

            // Create a array to store the tasks
            Task<int>[] tasks = new Task<int>[possibleMovesLength -1]; // Why -1? Using possibleMovesLength without -1 results in an IndexOutOfRangeException

            // Loop through all the possible moves
            for (int i = 0; i < ChessBoard.GetAllPossibleMoves(color).Count -1; i++) // Here as well
            {
                // Create a copy of the Chessboard
                Board boardCopy = CopyBoard(ChessBoard);

                // Call the Alpha Beta thread method in a new thread and get the score
                // ----- Important! -----
                // Altough ChessBoard and boardCopy have the same position, their
                // moves are not interchangeable.
                // Even if the right ChessBoard is called, passing in the wrong
                // move will result in the other Board being changed
                tasks[i] = Task.Run(() => AlphaBetaThread(boardCopy, boardCopy.GetAllPossibleMoves(color)[i], bestMoveScore, depth, maxValuePlayer, alpha, beta)); // Can even throw an SystemOutOfRangeException when using possibleMovesLength -1
            }

            // Wait for all the tasks to finish
            Task.WaitAll(tasks);

            // Define the score variable
            int score;

            // Loop through all the tasks
            for (int j = 0; j < possibleMovesLength - 1; j++)
            {
                score = tasks[j].Result;

                // Check if the score is better than the best move score
                if (score > bestMoveScore)
                {
                    // If it is, set the best move score to the score
                    bestMoveScore = score;

                    // Set the best move to the possible move
                    bestMove = ChessBoard.GetAllPossibleMoves(color)[j];
                }
            }

            // Return the best move
            return bestMove;
        }

        #endregion

        #region Algorithm

        /// <summary>
        /// Evaluates the overall value on the board, white pieces count positively, black pieces count negatively
        /// </summary>
        /// <returns>Integer representing how good the board is</returns>
        private int EvaluateBoard(Board chessBoard)
        {
            // Set the evaluation value to 0
            int evaluationValue = 0;

            // Loop through all the cells
            foreach (Cell? cell in chessBoard.GetCells())
            {
                // Make sure the cell is not empty
                if (!cell.IsEmpty)
                { 
                    // Check if the cell has a piece
                    if (cell.GetPiece() !=  null)
                    {
                        // Get the value of the piece
                        int pieceValue = GetPieceValue(cell.GetPiece()!.PieceType);

                        // Add the value to the evaluation value
                        evaluationValue += cell.GetPiece()!.PieceColor == Piece.PColor.White ? pieceValue : -pieceValue;

                        // Additionally, favor the center of the board
                        if ((cell.Index >= 19 && cell.Index <= 22) ||
                            (cell.Index >= 43 && cell.Index <= 46) ||
                            cell.Index == 27 || 
                            cell.Index == 30 ||
                            cell.Index == 35 ||
                            cell.Index == 38) {
                            // If this is the case, it means that the piece is in the "outer ring" of the center
                            // Add 1 to the evaluation value
                            evaluationValue += cell.GetPiece()!.PieceColor == Piece.PColor.White ? 1 : -1;
                        }

                        else if ((cell.Index >= 28 && cell.Index <= 29) || (cell.Index >= 36 && cell.Index <= 37))
                        {
                            // If this is the case, it means that the piece is in the "inner ring" of the center
                            // Add 2 to the evaluation value
                            evaluationValue += cell.GetPiece()!.PieceColor == Piece.PColor.White ? 2 : -2;
                        }

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
        /// This method will be run in a seperate thread
        /// It implements the Alpha-Beta-Pruning algorithm
        /// </summary>
        private int AlphaBetaThread(Board chessBoard, Move possibleMove, int bestMoveScore, int depth, bool maxValuePlayer, int alpha, int beta)
        {
            // Do the move on the board
            
            chessBoard.DoMove(possibleMove);

            // Get the score of the move
            int moveScore = AlphaBeta(chessBoard, depth, depth, maxValuePlayer, alpha, beta);

            // See if the score is better than the best move score
            int score = Math.Max(bestMoveScore, moveScore);

            // Undo the move
            chessBoard.UndoMove(possibleMove);

            // Return the score
            return score;
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
        private int AlphaBeta(Board chessBoard, int maxDepth, int currentDepth, bool maxValuePlayer, int alpha, int beta)
        {
            // Check if the current depth is 0
            if (currentDepth == 0)
            {
                // If it is, return the evaluation of the board
                return EvaluateBoard(chessBoard);
            }

            // Evaluate for max Player
            if (maxValuePlayer)
            {
                // Set the best score to a very low value
                int bestScore = -100000;

                // Loop through all the possible moves
                foreach (Move possibleMove in chessBoard.GetAllPossibleMoves(color))
                {
                    // Do the move on the board
                    chessBoard.DoMove(possibleMove);

                    // calculating node score, if the current node will be the leaf node, then score will be
                    // calculated by static evaluation;
                    // score will be calculated by finding max value between node score and current best score.
                    int nodeScore = AlphaBeta(chessBoard, maxDepth, currentDepth - 1, false, alpha, beta);

                    bestScore = Math.Max(bestScore, nodeScore);

                    // undoing the last move, so as to explore new moves while backtracking
                    chessBoard.UndoMove(possibleMove);

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
                foreach (Move possibleMove in chessBoard.GetAllPossibleMoves(color))
                {
                    // Do the move on the board
                    chessBoard.DoMove(possibleMove);

                    // calculating node score, if the current node will be the leaf node, then score will be
                    // calculated by static evaluation;
                    // score will be calculated by finding min value between node score and current best score.
                    int nodeScore = AlphaBeta(chessBoard, maxDepth, currentDepth - 1, true, alpha, beta);

                    bestScore = Math.Min(bestScore, nodeScore);
                    chessBoard.UndoMove(possibleMove);

                    // calculating alpha for current MIN node
                    beta = Math.Min(beta, bestScore);

                    // beta cut off
                    if (beta <= alpha)
                        return bestScore;
                }
                return bestScore;
            }
        }
        

        /// <summary>
        /// Method to get a copy of the current Board
        /// </summary>
        public Board CopyBoard(Board originalBoard)
        {
            // Create a new board with the default constructor (assuming it creates an 8x8 board)
            Board copiedBoard = new Board();

            // Iterate through all cells on the current board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    // Get the cell from the current board
                    Cell currentCell = originalBoard.GetCell(row, col);

                    // Check if the current cell is empty
                    if (!currentCell.IsEmpty)
                    {
                        // Get the piece on the current cell
                        Piece currentPiece = currentCell.GetPiece()!;

                        // Create a new piece of the same type and color for the copied board
                        Piece? copiedPiece = null;
                        switch (currentPiece.PieceType)
                        {
                            case Piece.PType.Pawn:
                                copiedPiece = new Pawn(copiedBoard, currentPiece.PieceColor);
                                break;
                            case Piece.PType.Knight:
                                copiedPiece = new Knight(copiedBoard, currentPiece.PieceColor);
                                break;
                            case Piece.PType.Bishop:
                                copiedPiece = new Bishop(copiedBoard, currentPiece.PieceColor);
                                break;
                            case Piece.PType.Rook:
                                copiedPiece = new Rook(copiedBoard, currentPiece.PieceColor);
                                break;
                            case Piece.PType.Queen:
                                copiedPiece = new Queen(copiedBoard, currentPiece.PieceColor);
                                break;
                            case Piece.PType.King:
                                copiedPiece = new King(copiedBoard, currentPiece.PieceColor);
                                break;
                        }

                        // Set the copied piece on the corresponding cell of the copied board
                        copiedBoard.GetCell(row, col).SetPiece(copiedPiece);
                    }
                }
            }

            // Return the copied board
            return copiedBoard;
        }

        #endregion
    }
}