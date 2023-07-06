using ChessEngineClassLibrary.Models;
using ChessEngineClassLibrary.Pieces;
using ChessEngineClassLibrary.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessEngineClassLibrary
{
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

        private Cell SourceCell;


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
            if( SourceCell == null && cell.IsEmpty)
                return;

            // Set the source cell
            if (SourceCell == null && ((int)cell.GetPiece().PieceColor == (int)CurrentPlayer) && !cell.IsEmpty)
            {
                SourceCell = cell;

                // Set the Cell as selected
                cell.SetSelected(true);
            }
            // same cell is clicked again
            else if (SourceCell != null && SourceCell.Index == cell.Index)
            {
                SourceCell = null;

                // deselect the cell
                cell.SetSelected(false);
            }
            // Check for a possible new position
            else if (SourceCell != null) 
            {
                // Check if its a generic valid move
                if ( !CanMoveToDest(cell) )
                    return;

                // Check if its a valid move for the specific piece


                // If the move is valid, move the piece
                Move move = new Move(SourceCell, cell);
                GetPlayer(CurrentPlayer).AddMove(move);

                // if target Cell was not empty, the a kill was performed
                if (!cell.IsEmpty)
                    move.PieceKilled = cell.GetPiece();

                // Change current player
                if( (int)CurrentPlayer == (int)Piece.PColor.White )
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
                                newPiece = new Pawn(Piece.PColor.White, Resource1.wPawn);
                            else
                                newPiece = new Pawn(Piece.PColor.Black, Resource1.bPawn);
                            break;

                        case 'n':

                            if (pieceColor == Piece.PColor.White)
                                newPiece = new Knight(Piece.PColor.White, Resource1.wKnight);
                            else
                                newPiece = new Knight(Piece.PColor.Black, Resource1.bKnight);
                            break;

                        case 'b':

                            if (pieceColor == Piece.PColor.White)
                                newPiece = new Bishop(Piece.PColor.White, Resource1.wBishop);
                            else
                                newPiece = new Bishop(Piece.PColor.Black, Resource1.bBishop);
                            break;

                        case 'r':

                            if (pieceColor == Piece.PColor.White)
                                newPiece = new Rook(Piece.PColor.White, Resource1.wRook);
                            else
                                newPiece = new Rook(Piece.PColor.Black, Resource1.bRook);
                            break;

                        case 'q':

                            if (pieceColor == Piece.PColor.White)
                                newPiece = new Queen(Piece.PColor.White, Resource1.wQueen);
                            else
                                newPiece = new Queen(Piece.PColor.Black, Resource1.bQueen);
                            break;

                        case 'k':

                            if (pieceColor == Piece.PColor.White)
                                newPiece = new King(Piece.PColor.White, Resource1.wKing);
                            else
                                newPiece = new King(Piece.PColor.Black, Resource1.bKing);
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

        #region GameLogic for movements on the board

        /// <summary>
        /// Helper function that checks whether it is possible,
        /// for a chess piece to move to a spot.
        /// </summary>
        /// <param name="destCell"></param>
        /// <returns>true if possible</returns>
        public bool CanMoveToDest(Cell destCell)
        {
            Piece location = destCell.GetPiece();

            if (location == null)
                return true;

            if ( (int)location.PieceColor != (int)CurrentPlayer)
                return true;

            return false;
        }


        /// <summary>
        /// Movement logic of the Pawn
        /// </summary>
        /// <param name="startCell">actual position of the Pawn</param>
        /// <param name="destCell">destination postion of the Pawn</param>
        /// <returns>True, if movement is possible</returns>
        private bool PawnMovement(Cell startCell, Cell destCell)
        {
            int one_step;
            int two_step;

            if (startCell.GetPiece().PieceColor == Piece.PColor.Black)
            {
                one_step = 1;
                two_step = 2;
            }
            else
            {
                one_step = -1;
                two_step = -2;
            }

            // Moving one step forward
            if (destCell.Location[1] - startCell.Location[1] == one_step)
            {
                // Straight
                if ( (startCell.Location[1] == destCell.Location[1]) && destCell.IsEmpty)
                {
                    return true;
                }
                // Diagonally
                if (Math.Abs(startCell.Location[1] - destCell.Location[1]) == 1 && !destCell.IsEmpty)
                {
                    return true;
                }
            }
            // Two spaces
            else if (!startCell.GetPiece().HasMoved)
            {
                if (destCell.Location[1] - startCell.Location[1] == two_step)
                {
                    if (startCell.Location[1] == destCell.Location[1] && destCell.IsEmpty)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// Movement logic of the Rook
        /// </summary>
        /// <param name="startCell">actual position of the Pawn</param>
        /// <param name="destCell">destination postion of the Pawn</param>
        /// <returns></returns>
        private bool RookMovement(Cell startCell, Cell destCell)
        {
            return this.CanMoveStraight(startCell, destCell);
        }


        /// <summary>
        /// Movement logic of the Bishop
        /// </summary>
        /// <param name="startCell">actual position of the Pawn</param>
        /// <param name="destCell">destination postion of the Pawn</param>
        /// <returns></returns>
        private bool BishopMovement(Cell startCell, Cell destCell)
        { 
            return this.CanMoveDiagonal(startCell, destCell);
        }


        /// <summary>
        /// Movement logic of the Knight
        /// </summary>
        /// <param name="startCell">actual position of the Pawn</param>
        /// <param name="destCell">destination postion of the Pawn</param>
        /// <returns></returns>
        private bool KnightMovement(Cell startCell, Cell destCell)
        { 
      		if (   Math.Abs(startCell.Location[0] - destCell.Location[0]) == 2 
                && Math.Abs(startCell.Location[1] - destCell.Location[1]) == 1)
			    return true;
		
            if (   Math.Abs(startCell.Location[0] - destCell.Location[0]) == 1 
                && Math.Abs(startCell.Location[1] - destCell.Location[1]) == 2)
			    return true;
		
            return false;
        }


        /// <summary>
        /// Movement logic of the Queen
        /// </summary>
        /// <param name="startCell">actual position of the Pawn</param>
        /// <param name="destCell">destination postion of the Pawn</param>
        /// <returns></returns>
        private bool QueenMovement(Cell startCell, Cell destCell)
        {
            if (   this.CanMoveStraight(startCell, destCell)
                || this.CanMoveDiagonal(startCell, destCell))
                return true;
            return false;
        }


        /// <summary>
        /// Movement logic of the King
        /// </summary>
        /// <param name="startCell">actual position of the Pawn</param>
        /// <param name="destCell">destination postion of the Pawn</param>
        /// <returns></returns>
        private bool KingMovement(Cell startCell, Cell destCell)
        {
            int absoluteX = Math.Abs(destCell.Location[0] - startCell.Location[0]);
            int absoluteY = Math.Abs(destCell.Location[1] - startCell.Location[1]);

            if (absoluteX <= 1 && absoluteY <= 1)
            {
                if (absoluteX == 0 && absoluteY == 0)
                {
                    return false;
                }
                return true;
            }
            return false;
        }


        /// <summary>
        /// Helper Class the determin, if a Piece can move in a straight line
        /// <param name="sourceCell"></param>
        /// <param name="destCell"></param>
        /// <returns>true if possible</returns>
        public bool CanMoveStraight(Cell sourceCell, Cell destCell)
        {
            int[] destIndex = destCell.Location;
            int[] sourceIndex = sourceCell.Location;

            int currX = sourceIndex[0];
            int currY = sourceIndex[1];

            int startIndex;
            int endIndex;

            // X position remains the same
            if (currX == destIndex[0])
            {
                if (currY > destIndex[1])
                {
                    startIndex = destIndex[1];
                    endIndex = currY;
                }
                else if (currY < destIndex[1])
                {
                    startIndex = currY;
                    endIndex = destIndex[1];
                }
                else return false;

                // Loop from start to end position and check if no pieces is in between
                for (startIndex++; startIndex < endIndex; startIndex++)
                {
                    if (!ChessBoard.GetCell(currX, startIndex).IsEmpty)
                        return false;
                }
                return true;
            }

            // Y position remains the same
            if (currY == destIndex[1])
            {
                if (currX > destIndex[0])
                {
                    startIndex = destIndex[0];
                    endIndex = currX;
                }
                else if (currX < destIndex[0])
                {
                    startIndex = currX;
                    endIndex = destIndex[0];
                }
                else return false;

                // Loop from start to end position and check if no pieces is in between
                for (startIndex++; startIndex < endIndex; startIndex++)
                {
                    if (!ChessBoard.GetCell(startIndex, currY).IsEmpty)
                        return false;
                }
                return true;
            }
            return false;
        }


        /// <summary>
        /// Helper function for determining whether a piece can move in a diagonal line.
        /// </summary>
        /// <param name="sourceCell"></param>
        /// <param name="destCell"></param>
        /// <returns></returns>
        public bool CanMoveDiagonal(Cell sourceCell, Cell destCell)
        {
            int startIndex = 0;
            int endIndex = 0;
            int cellInc = 0;

            // Check for a diagonal move
            int cellMove = Math.Abs(sourceCell.Index - destCell.Index);

            if ((cellMove % 7) == 0)
            {
                if (sourceCell.Index < destCell.Index)
                {
                    startIndex = sourceCell.Index;
                    endIndex = destCell.Index;
                }
                else if (sourceCell.Index > destCell.Index)
                {
                    startIndex = destCell.Index;
                    endIndex = sourceCell.Index;
                }
                else
                    return false;
                cellInc = 7;
            }
            else if ((cellMove % 9) == 0)
            {
                if (sourceCell.Index < destCell.Index)
                {
                    startIndex = sourceCell.Index;
                    endIndex = destCell.Index;
                }
                else if (sourceCell.Index > destCell.Index)
                {
                    startIndex = destCell.Index;
                    endIndex = sourceCell.Index;
                }
                else
                    return false;
                cellInc = 9;
            }

            // Loop to see if any piece is in between
            for (startIndex += cellInc; startIndex < endIndex; startIndex += cellInc)
            {
                if (!ChessBoard.GetCell(startIndex).IsEmpty)
                {
                    return false;
                }
                return true;
            }
            return false;
        }


        //public bool IsKingInCheck(Piece.PColor color)
        //{
        //    bool result = false;

        //    List<Piece> originalList;
        //    King kingInQuestion;

        //    if (color == Piece.PColor.Black)
        //    {
        //        originalList = this.WhitePieces;
        //        kingInQuestion = this.BlackPieces.Find(obj => obj.PieceType == Piece.PType.King);
        //    }
        //    else
        //    {
        //        originalList = this.BlackPieces;
        //        kingInQuestion = whiteKing;
        //    }

        //    int xKingLoc = kingInQuestion.getXLocation();
        //    int yKingLoc = kingInQuestion.getYLocation();

        //    for (Piece currentPiece : originalList)
        //    {
        //        if (currentPiece.canMoveTo(xKingLoc, yKingLoc))
        //        {
        //            result = true;
        //        }
        //    }

        //    return result;
        //}


        //public Queen addQueen(int color, int xloc, int yloc)
        //{
        //    Queen queen = new Queen(chessBoard, color, xloc, yloc);
        //    pieceToColorHelper(queen, color);

        //    return queen;
        //}

        //public Knight addKnight(int color, int xloc, int yloc)
        //{
        //    Knight knight = new Knight(chessBoard, color, xloc, yloc);
        //    pieceToColorHelper(knight, color);

        //    return knight;
        //}

        //public Rook addRook(int color, int xloc, int yloc)
        //{
        //    Rook rook = new Rook(chessBoard, color, xloc, yloc);
        //    pieceToColorHelper(rook, color);

        //    return rook;
        //}

        //public Bishop addBishop(int color, int xloc, int yloc)
        //{
        //    Bishop bishop = new Bishop(chessBoard, color, xloc, yloc);
        //    pieceToColorHelper(bishop, color);

        //    return bishop;
        //}

        //public Pawn addPawn(int color, int xloc, int yloc)
        //{
        //    Pawn pawn = new Pawn(chessBoard, color, xloc, yloc);
        //    pieceToColorHelper(pawn, color);

        //    return pawn;
        //}

    }

    #endregion
}
