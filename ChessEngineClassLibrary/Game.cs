﻿using ChessEngineClassLibrary.Models;
using ChessEngineClassLibrary.Pieces;
using ChessEngineClassLibrary.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Numerics;

namespace ChessEngineClassLibrary
{
    /// <summary>
    /// Game Class that implements the Game Logic
    /// </summary>
    public class Game
    {
        #region Enumerators

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
        public enum Difficutlty
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
            Check,
            Resignation,
            Stalemate,
            ClockFlagged,
            Aggrement,
            FiftyMoveRule,
            Insufficient_Material,
            TimeOutVsInsufficient_Material,
            Checkmate
        }

        #endregion

        #region Properties and Members

        /// <summary>
        /// Constant for En Passant Row where Black is possible
        /// </summary>
        private const int ENP_ROW_BLACK = 4;

        /// <summary>
        /// Constant for En Passant Row where White is possible
        /// </summary>
        private const int ENPL_ROW_WHITE = 5;

        /// <summary>
        /// Current Player, Black or White
        /// </summary>
        public Piece.PColor CurrentPlayer { get; set; }

        /// <summary>
        /// Reference to the ChessBoard
        /// </summary>
        private Board ChessBoard;

        /// <summary>
        /// List of Players, either can be black or white
        /// </summary>
        private Player[] PlayerList;

        /// <summary>
        /// The selected Cell, form which a Piece is moved
        /// </summary>
        private Cell? sourceCell;

        /// <summary>
        /// List of all black Pieces
        /// </summary>
        public List<Piece> BlackPieces { get; set; }

        /// <summary>
        /// List of all white Pieces
        /// </summary>        
        public List<Piece> WhitePieces { get; set; }

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
        public GameState gameState { get; set; }

        #endregion

        #region Contructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="chessBoard">Referenz to the Board</param>
        public Game(Board chessBoard)
        {
            // Referenz to the Chess Board
            ChessBoard = chessBoard;

            // Create Players
            PlayerList = new Player[2] { new Player(this), new Player(this) };

            // Set Default Names            
            PlayerList[0].Name = "Player 1";
            PlayerList[1].Name = "Player 2";

            // Set Default Colors
            PlayerList[0].Color = Piece.PColor.White;
            PlayerList[1].Color = Piece.PColor.Black;

            // List with all Pieces for each Color
            BlackPieces = new List<Piece>();
            WhitePieces = new List<Piece>();

            // Register Eventhandler for Cell selection
            foreach (Cell cell in ChessBoard.GetCells())
            {
                cell.CellSelected += Cell_CellSelected;
            }

            // Set the GameState
            gameState = GameState.None;
        }

        #endregion

        #region Eventhandler

        /// <summary>
        /// Eventhandler for cell selection
        /// </summary>
        /// <param name="sender">Selected cell</param>
        /// <param name="e"></param>
        private void Cell_CellSelected(object? sender, EventArgs e)
        {
            // Convert sender to Cell
            Cell? cell = (Cell)sender;

            // Make sure, cell is not empty 
            if (cell == null)
                return;

            // if no cell is selected and the cell is empty, exit
            if (sourceCell == null && cell.IsEmpty)
                return;

            // Set the source cell
            if (sourceCell == null && !cell.IsEmpty && (cell.GetPiece().PieceColor == CurrentPlayer))
            {
                sourceCell = cell;

                // Set the Cell as selected
                cell.SetSelected(true);

                // Get all possible Cells to move the Piece to
                foreach (Cell targetCell in this.GetTargetMoveCells())
                    targetCell.SetSelected(true, 1);
            }

            // same cell is clicked again
            else if (sourceCell != null && sourceCell.Index == cell.Index)
            {
                // Get all possible Cells to move the Piece to and remove selection
                foreach (Cell targetCell in this.GetTargetMoveCells())
                    targetCell.SetSelected(false);

                // deselect the source cell
                sourceCell.SetSelected(false);

                sourceCell = null;

            }

            // Check for a possible new position
            else if (sourceCell != null)
            {
                // If Pawn, check for En Passant and for Promotion
                if (sourceCell.GetPiece().PieceType == Piece.PType.Pawn)
                {
                    // Check for En Passant
                    if (this.IsEnPassantMove(cell))
                    {
                        Cell? pawnToRemove;
                        Move? lastMove;

                        // Do the Move and remove the Opponents Pawn
                        if (GetPlayer(CurrentPlayer).Color == Piece.PColor.White)
                        { 
                            pawnToRemove = ChessBoard.GetCell(cell.Location[0], cell.Location[1] - 1);
                            lastMove = this.PlayerList[(int)Piece.PColor.Black].GetLastMove(false);
                            this.BlackPieces.Remove(pawnToRemove.GetPiece());
                        }
                        else
                        {
                            pawnToRemove = ChessBoard.GetCell(cell.Location[0], cell.Location[1] + 1);
                            lastMove = this.PlayerList[(int)Piece.PColor.White].GetLastMove(false);
                            this.WhitePieces.Remove(pawnToRemove.GetPiece());
                        }

                        GetPlayer(CurrentPlayer).GetLastMove(false).PieceKilled = pawnToRemove.GetPiece();

                        // Remove the capured Piec, from Cell
                        pawnToRemove.RemovePiece();

                        // Call the Move Method
                        this.PerformMove(cell);

                        return;
                    }
                    // Check for Promotion
                    else if (this.IsPromotionMove(cell))
                    {
                        // Select the new Piece and remove the old one
                        // Call the select Piece Dialog

                        // Perform the move
                        this.PerformMove(cell);

                        // Greate the new Piece and replace whith the old one

                        return;
                    }
                }

                // If King -> test for castling
                if (sourceCell.GetPiece().PieceType == Piece.PType.King && this.IsCastlingMove(cell))
                {
                    Cell? sourceRookCell;
                    Cell? destRookCell;

                    // Perform the move and move also the Rook
                    if (cell.Location[0] < 4)
                    {
                        sourceRookCell = ChessBoard.GetCell(0, cell.Location[1]);
                        destRookCell = ChessBoard.GetCell(cell.Location[0] + 1, cell.Location[1]);
                    }
                    else
                    { 
                        sourceRookCell = ChessBoard.GetCell(7, cell.Location[1]);
                        destRookCell = ChessBoard.GetCell(cell.Location[0] - 1, cell.Location[1]);
                    }

                    // Move the Piece from Source to Destination
                    Piece? piece = sourceRookCell.GetPiece();
                    piece.HasMoved = true;
                    sourceRookCell.RemovePiece();
                    destRookCell.SetPiece(piece);

                    // Call the Move Method
                    this.PerformMove(cell);

                    return;
                }

                // Check if the current Player King is in Check and will remain in Check after the move
                if (!IsCheckResolved(CurrentPlayer, cell))
                {
                    // Get all possible Cells to move the Piece to and remove selection
                    foreach (Cell targetCell in this.GetTargetMoveCells())
                        targetCell.SetSelected(false);

                    sourceCell.SetSelected(false);
                    sourceCell = null;
                    return;
                }
                else
                {
                    if (sourceCell.GetPiece().CanMoveToDest(cell))
                        this.PerformMove(cell);
                }
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
            gameState = GameState.Running;

            // Initalize the board
            ChessBoard.RemoveAllPieces();
            WhitePieces.Clear();
            BlackPieces.Clear();
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
            FenParser fenParser = new FenParser(fenString);

            // Setup the Player
            foreach (var player in PlayerList)
            {
                player.CanQueensideCastle = fenParser.BoardStateData.CanQueensideCastle[((int)player.Color)];
                player.CanKingsideCastle = fenParser.BoardStateData.CanKingsideCastle[((int)player.Color)]; 
            }

            // Initialize the Game
            this.CurrentPlayer = fenParser.BoardStateData.ActivePlayerColor;
            this.FullMoveNumber = fenParser.BoardStateData.FullMoveNumber;
            this.HalfMoveCounter = fenParser.BoardStateData.HalfMoveCounter;
            
            // Setup to board with all pieces
            for(int row = 0; row < fenParser.BoardStateData.Ranks.GetLength(0); row++) 
            {
                // retreive the first column
                var cValues = fenParser.BoardStateData.Ranks[row];

                // for each cell in this column, create a piece when necessary
                for (int column = 0; column < cValues.Length;  column++) 
                {
                    Piece newPiece = null;

                    // Define Attributes of that will be given to the Piece instance
                    char pieceIdent = cValues[column].ToCharArray().First();
                    Piece.PColor pieceColor = (char.IsUpper(pieceIdent) ? Piece.PColor.White : Piece.PColor.Black);

                    // Use switch statement to update the array based on the Piece type
                    switch (char.ToLower(pieceIdent))
                    {
                        case 'p':

                            if (pieceColor == Piece.PColor.White)
                                newPiece = new Pawn(ChessBoard, Piece.PColor.White, Resource1.wPawn);
                            else
                                newPiece = new Pawn(ChessBoard, Piece.PColor.Black, Resource1.bPawn);
                            break;

                        case 'n':

                            if (pieceColor == Piece.PColor.White)
                                newPiece = new Knight(ChessBoard, Piece.PColor.White, Resource1.wKnight);
                            else
                                newPiece = new Knight(ChessBoard, Piece.PColor.Black, Resource1.bKnight);
                            break;

                        case 'b':

                            if (pieceColor == Piece.PColor.White)
                                newPiece = new Bishop(ChessBoard, Piece.PColor.White, Resource1.wBishop);
                            else
                                newPiece = new Bishop(ChessBoard, Piece.PColor.Black, Resource1.bBishop);
                            break;

                        case 'r':

                            if (pieceColor == Piece.PColor.White)
                                newPiece = new Rook(ChessBoard, Piece.PColor.White, Resource1.wRook);
                            else
                                newPiece = new Rook(ChessBoard, Piece.PColor.Black, Resource1.bRook);
                            break;

                        case 'q':

                            if (pieceColor == Piece.PColor.White)
                                newPiece = new Queen(ChessBoard, Piece.PColor.White, Resource1.wQueen);
                            else
                                newPiece = new Queen(ChessBoard, Piece.PColor.Black, Resource1.bQueen);
                            break;

                        case 'k':

                            if (pieceColor == Piece.PColor.White)
                                newPiece = new King(ChessBoard, Piece.PColor.White, Resource1.wKing);
                            else
                                newPiece = new King(ChessBoard, Piece.PColor.Black, Resource1.bKing);
                            break;
                    }

                    // Add Piece to the Board if not null
                    if (newPiece != null)
                    {
                        ChessBoard.PlacePlieceOnBoard(newPiece, (row * 8) + column);

                        // Add the Piece to the List of all Pieces
                        if(newPiece.PieceColor.Equals(Piece.PColor.White))
                            WhitePieces.Add(newPiece);
                        else
                            BlackPieces.Add(newPiece);

                        // Set newPiece to NULL
                        newPiece = null;
                    }
                }
            }
        }


        /// <summary>
        /// Clears the Board and sets the Game State to None
        /// </summary>
        public void EndGame()
        {
            this.SetNewGame();
            gameState = GameState.None;
        }


        /// <summary>
        /// Method to undo the players last move
        /// </summary>
        public void UndoLastMove()
        { 
            throw new NotImplementedException();    
        }

        /// <summary>
        /// This Method performs the acutal move and adjusts the board
        /// </summary>
        /// <param name="destCell">Target Cell for the Piece</param>
        private void PerformMove(Cell? destCell)
        {
            // Check that destCell not NULL
            if (destCell == null)
                return;

            // Get all possible Cells to move the Piece to and remove selection
            foreach (Cell targetCell in this.GetTargetMoveCells())
                targetCell.SetSelected(false);

            // Create a move and store it a the actual Player
            Move move = new Move(sourceCell, destCell);
            GetPlayer(CurrentPlayer).AddMove(move);

            // if target Cell was not empty, a kill was performed
            if (!destCell.IsEmpty)
            {
                move.PieceKilled = destCell.GetPiece();

                // Remove the capuret Piec, from Cell and List
                destCell.RemovePiece();

                if (GetPlayer(CurrentPlayer).Color == Piece.PColor.White)
                    this.BlackPieces.Remove(move.PieceKilled);
                else
                    this.WhitePieces.Remove(move.PieceKilled);

                // Reset Halfmove Counter
                this.HalfMoveCounter = 0;
            }

            // Move the Piece from Source to Destination
            Piece? piece = sourceCell.GetPiece();
            piece.HasMoved = true;
            sourceCell.RemovePiece();
            destCell.SetPiece(piece);
            sourceCell = null;

            // Check if Game is over
            if (this.IsGameOver())
            {
                // Terminate the Game and show a Message Box
                Debug.WriteLine("Game Over");
            }

            // If Pawn Movement, Reset the Halfmove Counter
            if (piece.PieceType == Piece.PType.Pawn)
            this.HalfMoveCounter = 0;

            // Change current player
            if ((int)CurrentPlayer == (int)Piece.PColor.White)
            {
                CurrentPlayer = Piece.PColor.Black;
            }
            else
            {
                CurrentPlayer = Piece.PColor.White;
                this.FullMoveNumber++;
            }

            // If first move, set flag
            if (!FirstMoveDone)
                FirstMoveDone = true;

            // Start Timer of the new player, stop Timer of old Player
            foreach (Player player in PlayerList)
                player.SetCurrentPlayer(CurrentPlayer);

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


        #endregion

        #region GameLogic            

        /// <summary>
        /// Check, if the King is in Check Position
        /// </summary>
        /// <param name="color">Color of the King be checked, if InCheck</param>
        /// <param name="cell">Cell, on whtich the King would be InCheck</param>
        /// <returns>TRUE if in Check</returns>
        private bool IsKingInCheck(Piece.PColor color, Cell? cell = null)
        {
            bool result = false;

            List<Piece> originalList;
            King? kingInQuestion;

            if (color == Piece.PColor.Black)
            {
                originalList = WhitePieces;
                kingInQuestion = (King?)BlackPieces.Find(obj => obj.PieceType == Piece.PType.King);
            }
            else
            {
                originalList = BlackPieces;
                kingInQuestion = (King?)WhitePieces.Find(obj => obj.PieceType == Piece.PType.King);
            }

            // Get the Location of the King
            int xKingLoc = (cell == null) ? kingInQuestion.Location[0] : cell.Location[0];
            int yKingLoc = (cell == null) ? kingInQuestion.Location[1] : cell.Location[1];


            // Check for each Piece of the opponent Player, if it can move to the King's position
            foreach(Piece currentPiece in originalList)
            {
                if (currentPiece.CanMoveToDest(this.ChessBoard.GetCell(xKingLoc, yKingLoc)))
                {
                    result = true;
                }
            }
            return result;
        }


        /// <summary>
        /// Method to check a Games Postion, e.q. if the King is still or new in Check after a Move has been performed
        /// </summary>
        /// <param name="color">Color of the current player</param>
        /// <param name="cell">Target Cell, where to move to</param>
        /// <returns></returns>
        private bool IsCheckResolved(Piece.PColor color, Cell cell)
        {
            bool isResolved = false;
            List<Piece> checkPieces;

            // Select all of the opponents pieces 
            if (color == Piece.PColor.Black)
                checkPieces = WhitePieces;
            else
                checkPieces = BlackPieces;

            // Perform the desired move and check, if any Piece of the opponent can still go to the Kings Position

            // Save Piece on Cell and Save the old coordinates of the piece
            Piece? pieceOnSourceCell = sourceCell.GetPiece();
            Piece? pieceOnDestCell = cell.GetPiece();

            // Move the Piece to the new Cell, remove from old cell before AND remove from List of all Pieces
            sourceCell.RemovePiece();
            cell.RemovePiece();
            if (pieceOnDestCell != null)
                checkPieces.Remove(pieceOnDestCell);
            cell.SetPiece(pieceOnSourceCell);

            // If any piece can move to this spot, move here, the king is still in check, then go to next location.
            isResolved = !IsKingInCheck(color);

            // Restore old positions and return true AND add the removed Piece back in the List of all Pieces
            cell.RemovePiece();
            sourceCell.SetPiece(pieceOnSourceCell);
            if (pieceOnDestCell != null)
            {
                cell.SetPiece(pieceOnDestCell);
                checkPieces.Add(pieceOnDestCell);
            }

            return isResolved;
        }


        /// <summary>
        /// Test, if the Game is over
        /// </summary>
        /// <returns>TRUE if Game is over</returns>
        private bool IsGameOver()
        {
            if (IsCheckmate(Piece.PColor.White) || IsCheckmate(Piece.PColor.Black))
            {
                Debug.WriteLine("CHECKMATE");
                return true;
            }
            else if (!IsMovePossible(CurrentPlayer))
            {
                Debug.WriteLine("STALEMATE");
                return true;
            }
            return false;
        }


        /// <summary>
        /// Check for Checkmate
        /// </summary>
        /// <param name="color"></param>
        /// <returns>TRUE if Checkmate</returns>
        public bool IsCheckmate(Piece.PColor color)
        {
            if (IsKingInCheck(color))
            {
                if (!IsMovePossible(color))
                    return true;
            }
            return false;
        }


        /// <summary>
        /// Checks, if the given Player has any valid Moves left
        /// </summary>
        /// <returns></returns>
        private bool IsMovePossible(Piece.PColor color)
        {
            bool movePossible;
            Cell oldCell;
            List<Piece> checkPieces;

            if (color == Piece.PColor.Black)
                checkPieces = BlackPieces;
            else
                checkPieces = WhitePieces;

            // Test for each Cell, if a Piece of the given Player can move to
            foreach (Cell cell in ChessBoard.GetCells())
            {
                // If any piece can move to this spot, move here
                // If king is still in check, then go to next location.
                foreach (Piece piece in checkPieces)
                {
                    if (piece.CanMoveToDest(cell))
                    {
                        // Save Piece on Cell and Save the old coordinates of the piece
                        Piece? oldPieceOnCell = cell.GetPiece();
                        int[] oldLocation = piece.Location;
                        oldCell = ChessBoard.GetCell(oldLocation[0], oldLocation[1]);

                        // Move the Piece to the new Cell, remove from old cell before
                        oldCell.RemovePiece();
                        cell.RemovePiece();
                        if (oldPieceOnCell != null)
                            checkPieces.Remove(oldPieceOnCell);
                        cell.SetPiece(piece);

                        // no InCheck, take Move back and return true
                        movePossible = !this.IsKingInCheck(color);

                        // Restore old positions
                        cell.RemovePiece();
                        oldCell.SetPiece(piece);
                        if (oldPieceOnCell != null)
                        {
                            cell.SetPiece(oldPieceOnCell);
                            checkPieces.Add(oldPieceOnCell);
                        }

                        return movePossible;
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// Gets a List with all possible Cells, a Piece can move to
        /// </summary>
        /// <returns>The List with possible Cells</returns>
        private List<Cell> GetTargetMoveCells()
        {
            List<Cell> cells = new List<Cell>();

            if (sourceCell == null || sourceCell.IsEmpty)
                return cells;

            // Check for the source Cell, where it can possible move to
            foreach (Cell cell in ChessBoard.GetCells())
            { 
                if(sourceCell.GetPiece().CanMoveToDest(cell))
                    cells.Add(cell);

                // Check for Castling, and add the Cell to the List
                //if (sourceCell.GetPiece().PieceType == Piece.PType.King && IsCastlingMove(cell) )
                //    cells.Add(cell);
            }
            return cells;
        }


        /// <summary>
        /// Check, if the King can do a castling move
        /// </summary>
        /// <param name="destCell"></param>
        /// <returns>TRUE if in Check</returns>
        private bool IsCastlingMove(Cell destCell)
        {
            List<Piece> originalList;
            King? kingInQuestion;
            Cell? rookCell;
            bool kingPassingThroughCheck;

            // Current Color
            Piece.PColor playerColor = GetPlayer(CurrentPlayer).Color;

            if (playerColor == Piece.PColor.Black)
            {
                originalList = BlackPieces;
                kingInQuestion = (King?)originalList.Find(obj => obj.PieceType == Piece.PType.King);
            }
            else
            {
                originalList = WhitePieces;
                kingInQuestion = (King?)originalList.Find(obj => obj.PieceType == Piece.PType.King);
            }

            // Check the four conditions of Castling - King has not moved and is not in Check
            if ( kingInQuestion.HasMoved || IsKingInCheck(playerColor) )
                return false;

            // Check movement of the King - max 2 moves
            int kingMovement = kingInQuestion.Location[0] - destCell.Location[0];

            if (    Math.Abs( kingMovement ) != 2 
                && destCell.Location[1] != kingInQuestion.Location[1] )
                return false;

            // Castling Queenside - no pieces in between and Rook has not moved and King will not be in Check
            if (GetPlayer(CurrentPlayer).CanQueensideCastle && kingMovement > 0)
            {
                rookCell = ChessBoard.GetCell(0, kingInQuestion.Location[1]);
                kingPassingThroughCheck = IsKingInCheck(playerColor, ChessBoard.GetCell(destCell.Location[0], destCell.Location[1]))
                                          || IsKingInCheck(playerColor, ChessBoard.GetCell(destCell.Location[0] - 1, destCell.Location[1]));
            }
            else if (GetPlayer(CurrentPlayer).CanKingsideCastle && kingMovement < 0)
            {
                rookCell = ChessBoard.GetCell(7, kingInQuestion.Location[1]);
                kingPassingThroughCheck = IsKingInCheck(playerColor, ChessBoard.GetCell(destCell.Location[0], destCell.Location[1]))
                                          || IsKingInCheck(playerColor, ChessBoard.GetCell(destCell.Location[0] + 1, destCell.Location[1]));
            }
            else
                return false;
            
            // Rook has not moved and the way to the Rook is empty
            if (    rookCell.GetPiece() != null 
                && !rookCell.GetPiece().HasMoved
                &&  kingInQuestion.CanMoveStraight(rookCell)
                && !kingPassingThroughCheck)
            {
                return true;                
            }
            return false;
        }


        /// <summary>
        /// Check if a Promotion Move has been done
        /// </summary>
        /// <param name="destCell">destion cell</param>
        /// <returns>TRUE, if Promotion is possible</returns>
        private bool IsPromotionMove(Cell destCell)
        {
            int yEndCoord;

            if (GetPlayer(CurrentPlayer).Color == Piece.PColor.Black)
                yEndCoord = 0;
            else
                yEndCoord = 7;

            // not the end positioin
            if (destCell.Location[1] != yEndCoord)
                return false;

            return true;
        }


        /// <summary>
        /// Check if a Promotion Move has been done
        /// </summary>
        /// <param name="destCell"></param>
        /// <returns></returns>
        private bool IsEnPassantMove(Cell destCell)
        {
            Move? lastMove;
            bool previousMoveOk;

            //get last move from opponent player
            if (GetPlayer(CurrentPlayer).Color == Piece.PColor.White)
            {
                lastMove = GetPlayer(Piece.PColor.Black).GetLastMove(false);

                // If no moves so far
                if (lastMove == null)
                    return false;

                previousMoveOk = (lastMove.GetYMovement() == 2)
                                  && (lastMove.End.Location[1] == ENP_ROW_BLACK);
            }
            else
            {
                lastMove = GetPlayer(Piece.PColor.Black).GetLastMove(false);

                // If no moves so far
                if (lastMove == null)
                    return false;

                previousMoveOk = (lastMove.GetYMovement() == 2)
                                  && (lastMove.End.Location[1] == ENPL_ROW_WHITE);
            }

            // if last Move was a double move and the piece is on the right colum
            if (previousMoveOk && (destCell.Location[0] == lastMove.End.Location[0])
                && (sourceCell.Location[1] == lastMove.End.Location[1]))
                return true;

            return false;
        }
 
    }

    #endregion
}
