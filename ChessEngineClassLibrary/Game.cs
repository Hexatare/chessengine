using ChessEngineClassLibrary.Models;
using ChessEngineClassLibrary.Pieces;
using System;
using System.Linq;
using System.Threading;
using System.Timers;


namespace ChessEngineClassLibrary
{
    /// <summary>
    /// Game Class that implements the Game Logic
    /// </summary>
    public class Game
    {
        #region Properties and Members

        /// <summary>
        /// Current Player, Black or White
        /// </summary>
        public Piece.PColor CurrentPlayer { get; set; }

        /// <summary>
        /// The game's halfmove counter, used to determine a draw.
        /// </summary>
        public int HalfMoveCounter { get; set; }

        /// <summary>
        /// The game's move number.
        /// </summary>
        public int FullMoveNumber { get; set; }

        /// <summary>
        /// Flag for Information about the first move is done
        /// </summary>
        public bool FirstMoveDone { get; set; } = false;

        /// <summary>
        /// Current State of the Game
        /// </summary>
        public GameState ActGameState { get; set; }

        /// <summary>
        /// Reason, why the Game has Ended
        /// </summary>
        public GameEndReason GameEnd { get; set; } = GameEndReason.None;

        /// <summary>
        /// Current Game Settings
        /// </summary>
        public GameSettings CurrGameSettings { get; set; }

        /// <summary>
        /// Eventhandler for Game Updates
        /// </summary>
        public EventHandler? GameUpdateEvent;

        /// <summary>
        /// Eventhandler for the Promotion Dialog Event
        /// </summary>
        public EventHandler? PromotionEvent;

        /// <summary>
        /// The selected Promotion Piece
        /// </summary>
        public Piece.PType promotionPiece = Piece.PType.Queen;

        /// <summary>
        /// Eventhandler to fire the End of the Game Event
        /// </summary>
        public EventHandler? EndGameEvent;

        /// <summary>
        /// Reference to the ChessBoard
        /// </summary>
        private readonly Board ChessBoard;

        /// <summary>
        /// List of Players, either can be black or white
        /// </summary>
        private readonly Player[] PlayerList;

        /// <summary>
        /// The selected Cell, form which a Piece is moved
        /// </summary
        private Cell? sourceCell;

        /// <summary>
        /// The Chess Engine, that implements the MinMax Algorithem
        /// </summary>
        private readonly Engine engine;

        /// <summary>
        /// Timer for cyclic Events during the Game
        /// </summary>
        private System.Timers.Timer timer;

        /// <summary>
        /// Object to synchronize, e.g. make the Method ShowEndGameDialog Thread save
        /// </summary>
        private Object EndGameDlgSynch = new();

        #endregion

        #region Contructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="chessBoard">Reference to the Board</param>
        public Game(Board chessBoard)
        {
            // Reference to the Chess Board
            ChessBoard = chessBoard;

            // Create Players
            PlayerList = new Player[2] { new Player(this), new Player(this) };

            // Set Default Names            
            PlayerList[0].Name = "Player 1";
            PlayerList[1].Name = "Player 2";

            // Set Default Colors
            PlayerList[0].Color = Piece.PColor.White;
            PlayerList[1].Color = Piece.PColor.Black;

            // Create the Engine
            engine = new Engine(this, ChessBoard);

            // Set the ActGameState
            ActGameState = GameState.None;

            // Set the Game Mode
            this.CurrGameSettings = new GameSettings(GameMode.Human, Piece.PColor.White, Difficulty.Easy, GameTime.Min_5);

            // Setup a Timer for Timerevents
            timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
        }

        #endregion

        #region Input methods for Moves

        /// <summary>
        /// The user has selected a Cell - Event from the GUI
        /// </summary>
        /// <param name="cellIndex">Index of the selected Cell</param>
        public void UserCellSelection(int cellIndex)
        {
            // Make sure, viewCell is not empty and a Game is Running
            if (ActGameState != GameState.Running)
                return;

            // Get the Board Cell
            Cell? cell = ChessBoard.GetCell(cellIndex);

            // if no viewCell is selected and the viewCell is empty, exit
            if (sourceCell == null && cell.IsEmpty)
                return;

            // Set the source viewCell
            if (sourceCell == null && !cell.IsEmpty && (cell.GetPiece().PieceColor == CurrentPlayer))
            {
                sourceCell = cell;

                // Set the Cell as selected
                cell.CurrCellBorderColor = Cell.CellBorderColor.Yellow;
                Move move = new(cell, cell, CurrentPlayer);

                // Get all possible Cells to move the Piece to
                foreach (Cell targetCell in ChessBoard.GetTargetMoveCells(move))
                    targetCell.CurrCellBorderColor = Cell.CellBorderColor.Green;
            }

            // Same viewCell is clicked again
            else if (sourceCell != null && sourceCell.Index == cell.Index)
            {
                cell.CurrCellBorderColor = Cell.CellBorderColor.None;
                Move move = new(cell, cell, CurrentPlayer);

                // Get all possible Cells to move the Piece to and remove selection
                foreach (Cell targetCell in ChessBoard.GetTargetMoveCells(move))
                    targetCell.CurrCellBorderColor = Cell.CellBorderColor.None;

                // deselect the source viewCell
                sourceCell.CurrCellBorderColor = Cell.CellBorderColor.None;

                sourceCell = null;

                // Test both Kings of Check Position
                SetKingInCheckColor();
            }
            // Check for a possible new position
            else if (sourceCell != null)
            {
                this.BoardAction(new Move(sourceCell, cell, CurrentPlayer));
                sourceCell = null;
            }

            // Update View
            ChessBoard.OnUpdateView();

        }


        /// <summary>
        /// A Interface Method for the Chess Engine to make a move
        /// </summary>
        /// <param name="move">The move to performe</param>
        public void EngineMove(Move move)
        {
            // Call the BoardAction
            this.BoardAction(move);

            // Update the Board
            this.ChessBoard.OnUpdateView();

            // Set Game State:
            ActGameState = GameState.Running;

        }


        /// <summary>
        /// This Method implements the Game State and ist Called, when either a Player or the Computer is Performing a move
        /// </summary>
        /// <param name="move">The move to make</param>
        private void BoardAction(Move move)
        {
            Piece? piece = move.Start.GetPiece();

            // Check for spezial Pawn movements
            if (piece != null && piece.PieceType == Piece.PType.Pawn)
            {
                // En Passant Move ?
                if (ChessBoard.IsEnPassantMove(move))
                {
                    Cell? pawnCell;

                    // Do the Move and remove the Opponents Pawn
                    pawnCell = ChessBoard.GetCell(move.End.Location[0], move.End.Location[1] - 1);

                    // Store the Pawn in the move and delete it on the cell
                    move.PieceKilled = pawnCell.GetPiece();
                    pawnCell.RemovePiece();

                    // Perform the move
                    this.PerformMove(move);
                    return;
                }

                if (ChessBoard.IsPromotionMove(move))
                {
                    Piece? newPiece = null;
                    Piece.PColor pColor = CurrentPlayer;

                    // Perform the move
                    this.PerformMove(move);

                    // Raise Event to show the Dialog
                    PromotionEvent?.Invoke(this, EventArgs.Empty);

                    // create the new Piece and replace whith the old one
                    // Use switch statement to update the array based on the Piece type
                    switch (this.promotionPiece)
                    {
                        case Piece.PType.Knight:

                            newPiece = new Knight(ChessBoard, pColor);
                            break;

                        case Piece.PType.Bishop:

                            newPiece = new Bishop(ChessBoard, pColor);
                            break;

                        case Piece.PType.Rook:

                            newPiece = new Rook(ChessBoard, pColor);
                            break;

                        case Piece.PType.Queen:

                            newPiece = new Queen(ChessBoard, pColor);
                            break;
                    }

                    // Remove the Piece from the destCell and set the new created Piece there
                    Piece? oldPawn = move.End.GetPiece();
                    move.End.RemovePiece();
                    move.PromotionMove = true;

                    if (newPiece != null)
                        move.End.SetPiece(newPiece);

                    return;
                }

            }

            // Check for special King movements
            if (piece != null && piece.PieceType == Piece.PType.King)
            {
                if (ChessBoard.IsCastlingMove(move))
                {
                    Cell? sourceRookCell;
                    Cell? destRookCell;

                    // Perform the move and move also the Rook
                    if (move.End.Location[0] < 4)
                    {
                        sourceRookCell = ChessBoard.GetCell(0, move.End.Location[1]);
                        destRookCell = ChessBoard.GetCell(move.End.Location[0] + 1, move.End.Location[1]);
                    }
                    else
                    {
                        sourceRookCell = ChessBoard.GetCell(7, move.End.Location[1]);
                        destRookCell = ChessBoard.GetCell(move.End.Location[0] - 1, move.End.Location[1]);
                    }

                    // Move the Piece from Source to Destination
                    Piece? rook = sourceRookCell.GetPiece();

                    if (rook != null)
                    {
                        rook.HasMoved = true;
                        sourceRookCell.RemovePiece();
                        destRookCell.SetPiece(rook);
                        move.CastlingMove = true;
                        move.RookLoc = sourceRookCell;
                    }

                    // Call the Move Method
                    this.PerformMove(move);
                    return;
                }

            }

            // Check if the current Players King is in Check and will remain in Check after the move
            if (!ChessBoard.IsCheckResolved(move, CurrentPlayer))
            {
                // Get all possible Cells to move the Piece to and remove selection
                foreach (Cell targetCell in ChessBoard.GetTargetMoveCells(move))
                    targetCell.CurrCellBorderColor = Cell.CellBorderColor.None;

                move.Start.CurrCellBorderColor = Cell.CellBorderColor.None;
                sourceCell = null;
                return;
            }
            else
            {
                if (piece != null && piece.CanMoveToDest(move.End))
                    this.PerformMove(move);
            }

            // If the Game is over
            if (ActGameState == GameState.End)
            {
                timer.Enabled = false;
                ShowGameEndDialog();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Starts a new Game, Settings to default
        /// </summary>
        public void SetNewGame()
        {
            // Initialize the Players
            PlayerList[0].NewGame(Piece.PColor.White);
            PlayerList[1].NewGame(Piece.PColor.Black);

            // Move Counters to zero
            HalfMoveCounter = FullMoveNumber = 0;

            // Reset first move done
            FirstMoveDone = false;

            // Other members to reset
            sourceCell = null;

            // Initalize the board
            ChessBoard.RemoveAllPieces();

            // Initialize the Engine
            engine.StartNewGame(CurrGameSettings);

            // Start the Timer
            timer.Enabled = true;
        }


        /// <summary>
        /// Sets up the board according to a FEN String
        /// </summary>
        /// <param name="fenString">FEN String</param>
        public void SetNewGame(string fenString)
        {
            // Initialize both players
            this.SetNewGame();

            // Parse the Fen String and create the Board
            FenParser fenParser = new(fenString);

            // Initialize the Game
            this.CurrentPlayer = fenParser.BoardStateData.ActivePlayerColor;
            this.FullMoveNumber = fenParser.BoardStateData.FullMoveNumber;
            this.HalfMoveCounter = fenParser.BoardStateData.HalfMoveCounter;

            // Setup to board with all pieces
            for (int row = 0; row < fenParser.BoardStateData.Ranks.GetLength(0); row++)
            {
                // retreive the first column
                var cValues = fenParser.BoardStateData.Ranks[row];

                // for each viewCell in this column, create a piece when necessary
                for (int column = 0; column < cValues.Length; column++)
                {
                    Piece? newPiece = null;

                    // Define Attributes of that will be given to the Piece instance
                    char pieceIdent = cValues[column].ToCharArray().First();
                    Piece.PColor pieceColor = (char.IsUpper(pieceIdent) ? Piece.PColor.White : Piece.PColor.Black);

                    // Use switch statement to update the array based on the Piece type
                    switch (char.ToLower(pieceIdent))
                    {
                        case 'p':

                            if (pieceColor == Piece.PColor.White)
                                newPiece = new Pawn(ChessBoard, Piece.PColor.White);
                            else
                                newPiece = new Pawn(ChessBoard, Piece.PColor.Black);
                            break;

                        case 'n':

                            if (pieceColor == Piece.PColor.White)
                                newPiece = new Knight(ChessBoard, Piece.PColor.White);
                            else
                                newPiece = new Knight(ChessBoard, Piece.PColor.Black);
                            break;

                        case 'b':

                            if (pieceColor == Piece.PColor.White)
                                newPiece = new Bishop(ChessBoard, Piece.PColor.White);
                            else
                                newPiece = new Bishop(ChessBoard, Piece.PColor.Black);
                            break;

                        case 'r':

                            if (pieceColor == Piece.PColor.White)
                                newPiece = new Rook(ChessBoard, Piece.PColor.White);
                            else
                                newPiece = new Rook(ChessBoard, Piece.PColor.Black);
                            break;

                        case 'q':

                            if (pieceColor == Piece.PColor.White)
                                newPiece = new Queen(ChessBoard, Piece.PColor.White);
                            else
                                newPiece = new Queen(ChessBoard, Piece.PColor.Black);
                            break;

                        case 'k':

                            if (pieceColor == Piece.PColor.White)
                            {
                                newPiece = new King(ChessBoard, Piece.PColor.White);
                                ((King)newPiece).CanQueensideCastle = fenParser.BoardStateData.CanQueensideCastle[(int)Piece.PColor.White];
                                ((King)newPiece).CanKingsideCastle = fenParser.BoardStateData.CanKingsideCastle[(int)Piece.PColor.White];
                            }

                            else
                            {
                                newPiece = new King(ChessBoard, Piece.PColor.Black);
                                ((King)newPiece).CanQueensideCastle = fenParser.BoardStateData.CanQueensideCastle[(int)Piece.PColor.Black];
                                ((King)newPiece).CanKingsideCastle = fenParser.BoardStateData.CanKingsideCastle[(int)Piece.PColor.Black];
                            }
                            break;
                    }

                    // Add Piece to the Board if not null
                    if (newPiece != null)
                    {
                        ChessBoard.PlacePieceOnBoard(newPiece, (row * 8) + column);
                    }
                }
            }

            // Update the view
            ChessBoard.OnUpdateView();
            ActGameState = GameState.Running;

            // If Game Mode Computer and Computer is White, do the first move
            if (CurrGameSettings.Mode == GameMode.Computer && CurrGameSettings.Color == Piece.PColor.White)
            {
                // Call the engine.DoMove Method to perform the best move
                // Do this in a new Thread to avoid blocking the UI
                Thread thread = new Thread(() => engine.DoMove());

                // Start the Thread
                thread.Start();
            }
        }


        /// <summary>
        /// Clears the Board and sets the Game State to None
        /// </summary>
        public void SetGameEnd()
        {
            this.SetNewGame();
            ActGameState = GameState.None;
            timer.Enabled = false;

            // Update the view
            ChessBoard.OnUpdateView();
        }


        /// <summary>
        /// Returns the FEN String for the current Play State 
        /// </summary>
        /// <returns></returns>
        public string GetFenString()
        {
            // Create new Generator
            FenGenerator fenGen = new FenGenerator(ChessBoard, PlayerList, CurrentPlayer, HalfMoveCounter, FullMoveNumber);

            return fenGen.GeneratFen();
        }


        /// <summary>
        /// This Method performs the acutal move and adjusts the board
        /// </summary>
        /// <param name="move">The move to be performed</param>
        private void PerformMove(Move move)
        {
            // Create a move and store it a the actual Player
            //Move move = new Move(sourceCell, destCell);
            GetPlayer(CurrentPlayer).AddMove(move);

            // Get the piece from the Start Position
            Piece? piece = move.Start.GetPiece();

            if (piece != null)
            {
                // Piece has moved
                piece.HasMoved = true;

                // If Pawn Movement, Reset the Halfmove Counter
                if (piece.PieceType == Piece.PType.Pawn)
                    this.HalfMoveCounter = 0;
            }

            // Perform the move
            ChessBoard.DoMove(move);

            // Update the GUI
            move.CheckMateMove = ChessBoard.IsCheckmate((CurrentPlayer == Piece.PColor.White) ? Piece.PColor.Black : Piece.PColor.White);
            this.UpdateGui(move.GetUciMoveNaming());

            // Remove all Cell selections
            foreach (Cell cell in ChessBoard.GetCells())
                cell.CurrCellBorderColor = Cell.CellBorderColor.None;

            // Test both Kings of Check Position
            SetKingInCheckColor();

            // Check if Game is over
            if (this.IsGameOver())
            {
                PlayerList[(int)Piece.PColor.White].SetEndGame();
                PlayerList[(int)Piece.PColor.Black].SetEndGame();

                timer.Enabled = false;
                ActGameState = GameState.End;
                GameEnd = GameEndReason.Checkmate;
                ShowGameEndDialog();
                return;
            }

            // If first move, set flag
            if (!FirstMoveDone)
                FirstMoveDone = true;

            // Switch Player
            this.SwitchCurrentPlayer();
        }


        /// <summary>
        /// Cyclic Timer Events
        /// </summary>
        /// <param name="sender">Sender for the Event, e.q. Timer</param>
        /// <param name="e">Event Arguments</param>
        /// <exception cref="NotImplementedException"></exception>
        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            TimeSpan maxTime = new TimeSpan(0, (int)CurrGameSettings.TimePlay, 0);

            // Check for Remaining Time;
            if (GetPlayer(CurrentPlayer).TimePlayed() > maxTime)
            {
                timer.Enabled = false;
                ActGameState = GameState.End;
                GameEnd = GameEndReason.ClockFlagged;
                ShowGameEndDialog();
            }

            // Check for 50 Move Rule
            if (GetPlayer(CurrentPlayer).NbrOfHalfMoves >= 49)
            {
                timer.Enabled = false;
                ActGameState = GameState.End;
                GameEnd = GameEndReason.FiftyMoveRule;
                ShowGameEndDialog();
            }

            // Update Gui Values -> to be done
            this.UpdateGui("");

        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Private Helpermethod that returns the player with the desired color
        /// </summary>
        /// <param name="color">Color of the desired player</param>
        /// <returns></returns>
        private Player GetPlayer(Piece.PColor color)
        {
            if (PlayerList[0].Color.Equals(color))
                return PlayerList[0];
            else
                return PlayerList[1];
        }


        /// <summary>
        /// This Methode switches the Current Player, called after a move
        /// </summary>
        private void SwitchCurrentPlayer()
        {
            // Change current player
            if (CurrentPlayer == (int)Piece.PColor.White)
            {
                CurrentPlayer = Piece.PColor.Black;
            }
            else
            {
                CurrentPlayer = Piece.PColor.White;
                this.FullMoveNumber++;
            }

            // Start Timer of the new player, stop Timer of old Player
            foreach (Player player in PlayerList)
                player.SetCurrentPlayer(CurrentPlayer);

            // If Game Mode Computer and Computer ist White, do the first move
            if (CurrGameSettings.Mode == GameMode.Computer
                && ((CurrGameSettings.Color == Piece.PColor.Black && CurrentPlayer == Piece.PColor.Black)
                    || (CurrGameSettings.Color == Piece.PColor.White && CurrentPlayer == Piece.PColor.White)))
            {
                // Set Game Mode
                ActGameState = GameState.Calculating;

                // Call the DoMove Method in a new Thread to avoid blocking the UI
                Thread thread = new Thread(() => engine.DoMove());

                // Start the Thread
                thread.Start();
            }
        }


        /// <summary>
        /// Updates the Gui with current Game State Information
        /// </summary>
        /// <param name="move"></param>
        private void UpdateGui(string move)
        {
            TimeSpan maxTime = new TimeSpan(0, (int)CurrGameSettings.TimePlay, 0);

            // Update Gui Values -> to be done
            GameStateEventArgs gameStateEventArgs = new GameStateEventArgs();
            gameStateEventArgs.MoveInfo = move;

            // If Move, switch the Player
            if (!string.IsNullOrEmpty(move))
            {
                if (CurrentPlayer == Piece.PColor.White)
                {
                    gameStateEventArgs.FullMoveNbr = this.FullMoveNumber;
                    gameStateEventArgs.CurrentPlayer = Piece.PColor.Black;
                    gameStateEventArgs.TimeLeft = (maxTime - GetPlayer(Piece.PColor.Black).TimePlayed()).ToString("hh\\:mm\\:ss");
                }
                else
                {
                    gameStateEventArgs.CurrentPlayer = Piece.PColor.White;
                    gameStateEventArgs.TimeLeft = (maxTime - GetPlayer(Piece.PColor.White).TimePlayed()).ToString("hh\\:mm\\:ss");
                }
            }
            else
            {
                gameStateEventArgs.CurrentPlayer = CurrentPlayer;
                gameStateEventArgs.TimeLeft = (maxTime - GetPlayer(CurrentPlayer).TimePlayed()).ToString("hh\\:mm\\:ss");

            }
            // Fire the Event
            GameUpdateEvent?.Invoke(this, gameStateEventArgs);
        }


        /// <summary>
        /// Invokes the Game End Dialog, collects the relevant Game Information
        /// </summary>
        private void ShowGameEndDialog()
        {
            lock (EndGameDlgSynch)
            {
                // Collect the Game Information
                GameEndEventArgs gameEndEventArgs = new();
                gameEndEventArgs.Winner = CurrentPlayer;
                gameEndEventArgs.Reason = GameEnd;
                gameEndEventArgs.TimePlayed = string.Format("{0:mm\\:ss}", GetPlayer(CurrentPlayer).TimePlayed());
                gameEndEventArgs.NbrOfMoves = GetPlayer(CurrentPlayer).GetNbrOfMoves();
                gameEndEventArgs.CapturedPieces = GetPlayer(CurrentPlayer).GetAllCapturedPieces();

                EndGameEvent?.Invoke(this, gameEndEventArgs);
            }
        }


        #endregion

        #region Board Logic    

        /// <summary>
        /// Test if the Game is over
        /// </summary>
        /// <returns>True if the Game is over</returns>
        private bool IsGameOver()
        {
            if (ChessBoard.IsCheckmate(Piece.PColor.White) || ChessBoard.IsCheckmate(Piece.PColor.Black))
            {
                ActGameState = GameState.End;
                GameEnd = GameEndReason.Checkmate;
                return true;
            }
            else if (!ChessBoard.IsMovePossible(CurrentPlayer))
            {
                ActGameState = GameState.End;
                GameEnd = GameEndReason.Stalemate;
                return true;
            }
            return false;
        }


        /// <summary>
        /// Method to Set a red Frame around the King if in Check
        /// </summary>
        private void SetKingInCheckColor()
        {
            King? king = (King?)ChessBoard.GetPieces(Piece.PColor.White).Find(obj => obj.PieceType == Piece.PType.King);

            if (king != null)
            {
                // Test both Kings of Check Position
                ChessBoard.GetCell(king.Location).CurrCellBorderColor =
                    ChessBoard.IsKingInCheck(Piece.PColor.White) ? Cell.CellBorderColor.Red : Cell.CellBorderColor.None;
            }

            king = (King?)ChessBoard.GetPieces(Piece.PColor.Black).Find(obj => obj.PieceType == Piece.PType.King);

            if (king != null)
            {
                // Test both Kings of Check Position
                ChessBoard.GetCell(king.Location).CurrCellBorderColor =
                    ChessBoard.IsKingInCheck(Piece.PColor.Black) ? Cell.CellBorderColor.Red : Cell.CellBorderColor.None; ;
            }
        }


        #endregion
    }
}
