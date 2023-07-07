using ChessEngineClassLibrary.Models;
using ChessEngineClassLibrary.Pieces;
using ChessEngineClassLibrary.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

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

        #endregion

        #region Properties and Members

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
        /// The selected Cell, form which a Piece is moved
        /// </summary>
        private Cell sourceCell;


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
            Cell cell = (Cell)sender;
            
            // Make sure, cell is not empty 
            if (cell == null)
                return;

            // if no cell is selected and the cell is empty, exit
            if( sourceCell == null && cell.IsEmpty)
                return;

            // Set the source cell
            if (sourceCell == null && !cell.IsEmpty && (cell.GetPiece().PieceColor == CurrentPlayer) )
            {
                sourceCell = cell;

                // Set the Cell as selected
                cell.SetSelected(true);
            }
            // same cell is clicked again
            else if (sourceCell != null && sourceCell.Index == cell.Index)
            {
                sourceCell = null;

                // deselect the cell
                cell.SetSelected(false);
            }
            // Check for a possible new position
            else if (sourceCell != null) 
            {
                // Check if its a valid move
                if ( !sourceCell.GetPiece().CanMoveToDest(cell) )
                    return;

                // If the move is valid, move the piece
                Move move = new Move(sourceCell, cell);
                GetPlayer(CurrentPlayer).AddMove(move);

                // if target Cell was not empty, a kill was performed
                if (!cell.IsEmpty)
                {
                    move.PieceKilled = cell.GetPiece();

                    // Remove the capuret Pied
                    cell.RemovePiece();     
                }
                
                // Move the Piece from Source to Destination
                Piece? piece = sourceCell.GetPiece();
                piece.HasMoved = true;
                sourceCell.RemovePiece();
                cell.SetPiece(piece);
                sourceCell = null;

                // Change current player
                if ( (int)CurrentPlayer == (int)Piece.PColor.White )
                    CurrentPlayer = Piece.PColor.Black;
                else
                    CurrentPlayer = Piece.PColor.White;

                // If first move, set flag
                if (!FirstMoveDone)
                    FirstMoveDone = true;

                // Start Timer of the new player, stop Timer of old Player
                foreach(Player player in PlayerList)
                    player.SetCurrentPlayer(CurrentPlayer);
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

            // Initalize the board
            ChessBoard.RemoveAllPieces();
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
        /// Method to undo the players last move
        /// </summary>
        public void UndoLastMove()
        { 
            throw new NotImplementedException();    
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
        /// <param name="color"></param>
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
        /// Test, if the Game is over
        /// </summary>
        /// <returns>TRUE if Game is over</returns>
        //private bool IsGameOver()
        //{
        //    if (IsCheckmate(Piece.PColor.Black) || IsCheckmate(Piece.PColor.Black))
        //    {
        //        Debug.WriteLine("CHECKMATE");
        //        return true;
        //    }
        //    else if (!CanMove(CurrentPlayer))
        //    {
        //        Debug.WriteLine("STALEMATE");
        //        return true;
        //    }
        //    return false;
        //}


        /// <summary>
        /// Check for Checkmate
        /// </summary>
        /// <param name="color"></param>
        /// <returns>TRUE if Checkmate</returns>
        //public bool IsCheckmate(Piece.PColor color)
        //{
        //    if (IsKingInCheck(color))
        //    {
        //        if (!CanMove(color))
        //            return true;
        //    }
        //    return false;
        //}


        ///// <summary>
        ///// Checks, if the given Player has any valid Moves left
        ///// </summary>
        ///// <returns></returns>
        //private bool CanMove(Piece.PColor color)
        //{
        //    Cell oldCell;
        //    Piece target;
        //    List<Piece> checkPieces;

        //    if (color == Piece.PColor.Black)
        //        checkPieces = BlackPieces;
        //    else
        //        checkPieces = WhitePieces;

        //    // Test for each Cell, if a Piece of the given Player can move to
        //    foreach (Cell cell in ChessBoard.GetCells())
        //    {
        //        foreach (Piece piece in checkPieces)
        //        {
        //            if (piece.CanMoveToDest(cell))
        //            { 
        //                // Save the old cell
        //                oldCell = cell;
        //            }   
        //        }

        //    }



        //    for (int x = 0; x < chessBoard.getXDimension(); x++)
        //    {
        //        for (int y = 0; y < chessBoard.getYDimension(); y++)
        //        {
        //            // If any piece can move to this spot, move here
        //            // If king is still in check, then go to next location.
        //            for (Piece currentPiece : checkPieces)
        //            {
        //                if (currentPiece.canMoveTo(x, y))
        //                {
        //                    //System.out.println(x + ", " + y);
        //                    target = chessBoard.pieceAt(x, y);
        //                    oldX = currentPiece.getXLocation();
        //                    oldY = currentPiece.getYLocation();

        //                    currentPiece.moveTo(x, y);

        //                    if (!isKingInCheck(player))
        //                    {
        //                        currentPiece.moveTo(oldX, oldY);
        //                        if (target != null)
        //                            target.moveTo(x, y);
        //                        return true;
        //                    }
        //                    else
        //                    {
        //                        currentPiece.moveTo(oldX, oldY);
        //                        if (target != null)
        //                            target.moveTo(x, y);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return false;
        //}


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
            if(    Math.Abs( destCell.Location[0] - kingInQuestion.Location[0]) != 2 
                && destCell.Location[1] != kingInQuestion.Location[1] )
                return false;

            // Castling Queenside - no pieces in between and Rook has not moved and King will not be in Check
            if (GetPlayer(CurrentPlayer).CanQueensideCastle)
            {
                rookCell = ChessBoard.GetCell(0, kingInQuestion.Location[1]);
                kingPassingThroughCheck = IsKingInCheck(playerColor, ChessBoard.GetCell(destCell.Location[0], destCell.Location[1]))
                                          || IsKingInCheck(playerColor, ChessBoard.GetCell(destCell.Location[0] - 1, destCell.Location[1]));
            }
            else if (GetPlayer(CurrentPlayer).CanKingsideCastle)
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
            int yEndCoord = -1;

            if (GetPlayer(CurrentPlayer).Color == Piece.PColor.Black)
                yEndCoord = 0;
            else
                yEndCoord = 7;

            // not the end positioin
            if (destCell.Location[1] != yEndCoord)
                return false;

            return true;
        }

    }

    #endregion
}
