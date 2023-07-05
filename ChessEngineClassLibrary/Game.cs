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

        #endregion



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
        }


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




        //            System.out.print("Which piece to move? X-loc: ");
        //            int nextX = userInput.nextInt();
        //            System.out.print("Y-loc: ");
        //            int nextY = userInput.nextInt();

        //            Piece target = chessBoard.pieceAt(nextX, nextY);
        //            if (target == null)
        //            {
        //                System.out.println("That location is invalid");
        //                continueGame = false;
        //            }
        //            else if (target.getColor() != currentPlayer)
        //            {
        //                System.out.println("That is not your piece");
        //                continueGame = false;
        //            }
        //            else
        //            {
        //                System.out.print("Where to move this piece? x-loc: ");
        //                nextX = userInput.nextInt();
        //                System.out.print("Y-loc: ");
        //                nextY = userInput.nextInt();

        //                if (target.canMoveTo(nextX, nextY))
        //                {
        //                    target.moveTo(nextX, nextY);
        //                }
        //                else
        //                {
        //                    System.out.println("Cannot move there");
        //                }
        //            }
        //        }
        //    }

        //    /**
        //     * Checks to see if game-ending situation has occurred
        //     * 
        //     * NOTE: few more game-ending situations should be added,
        //     * like 50 move rule, threefold repetition.
        //     * 
        //     * Added 'no legal move' draw
        //     * Added 'checkmate' end
        //     * @return - True if game is over
        //     */
        //    public boolean isGameOver()
        //    {
        //        if (isCheckmate(BLACK) || isCheckmate(WHITE))
        //        {
        //            System.out.println("CHECKMATE");
        //            return true;
        //        }
        //        else if (!canMove(currentPlayer))
        //        {
        //            System.out.println("STALEMATE");
        //            return true;
        //        }
        //        return false;
        //    }

        //    /**
        //     * Check to see if the given player
        //     * is in a checkmate situation
        //     * @param color - color of the player who may be in checkmate
        //     * @return - True if player is indeed in checkmate
        //     */
        //    public boolean isCheckmate(int color)
        //    {
        //        if (isKingInCheck(color))
        //        {
        //            if (!canMove(color))
        //                return true;
        //        }

        //        return false;
        //    }

        //    /**
        //     * Determines whether the given player has any valid
        //     * moves left to play
        //     * @param player - Player who's moves are being checked
        //     * @return - True if the player still has valid moves
        //     */
        //    public boolean canMove(int player)
        //    {
        //        int oldX, oldY;
        //        Piece target;
        //        LinkedList<Piece> checkPieces;

        //        if (player == BLACK)
        //            checkPieces = blackPieces;
        //        else
        //            checkPieces = whitePieces;

        //        for (int x = 0; x < chessBoard.getXDimension(); x++)
        //        {
        //            for (int y = 0; y < chessBoard.getYDimension(); y++)
        //            {
        //                // If any piece can move to this spot, move here
        //                // If king is still in check, then go to next location.
        //                for (Piece currentPiece : checkPieces)
        //                {
        //                    if (currentPiece.canMoveTo(x, y))
        //                    {
        //                        //System.out.println(x + ", " + y);
        //                        target = chessBoard.pieceAt(x, y);
        //                        oldX = currentPiece.getXLocation();
        //                        oldY = currentPiece.getYLocation();

        //                        currentPiece.moveTo(x, y);

        //                        if (!isKingInCheck(player))
        //                        {
        //                            currentPiece.moveTo(oldX, oldY);
        //                            if (target != null)
        //                                target.moveTo(x, y);
        //                            return true;
        //                        }
        //                        else
        //                        {
        //                            currentPiece.moveTo(oldX, oldY);
        //                            if (target != null)
        //                                target.moveTo(x, y);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        return false;
        //    }

        //    /**
        //     * Checks if a given player's king is in check
        //     * @param color - the color of the player's king being checked
        //     * @return - True if the specified king is in check.
        //     */
        //    public boolean isKingInCheck(int color)
        //    {
        //        boolean result = false;

        //        LinkedList<Piece> originalList;
        //        King kingInQuestion;

        //        if (color == BLACK)
        //        {
        //            originalList = whitePieces;
        //            kingInQuestion = blackKing;
        //        }
        //        else
        //        {
        //            originalList = blackPieces;
        //            kingInQuestion = whiteKing;
        //        }

        //        int xKingLoc = kingInQuestion.getXLocation();
        //        int yKingLoc = kingInQuestion.getYLocation();

        //        for (Piece currentPiece : originalList)
        //        {
        //            if (currentPiece.canMoveTo(xKingLoc, yKingLoc))
        //            {
        //                result = true;
        //            }
        //        }

        //        return result;
        //    }

        //    /**
        //     * Removes this piece from the game
        //     * 
        //     * ASSERT that the removed piece is already in game.
        //     * @param removeThisPiece the piece to remove.
        //     */
        //    public void removePiece(Piece removeThisPiece)
        //    {
        //        removeThisPiece.removePiece();
        //        int color = removeThisPiece.getColor();

        //        if (color == BLACK)
        //            blackPieces.remove(removeThisPiece);
        //        else
        //            whitePieces.remove(removeThisPiece);
        //    }

        //    public void switchPlayerTurn()
        //    {
        //        if (currentPlayer == WHITE)
        //            currentPlayer = BLACK;
        //        else currentPlayer = WHITE;
        //    }

        //    public Queen addQueen(int color, int xloc, int yloc)
        //    {
        //        Queen queen = new Queen(chessBoard, color, xloc, yloc);
        //        pieceToColorHelper(queen, color);

        //        return queen;
        //    }

        //    public Knight addKnight(int color, int xloc, int yloc)
        //    {
        //        Knight knight = new Knight(chessBoard, color, xloc, yloc);
        //        pieceToColorHelper(knight, color);

        //        return knight;
        //    }

        //    public Rook addRook(int color, int xloc, int yloc)
        //    {
        //        Rook rook = new Rook(chessBoard, color, xloc, yloc);
        //        pieceToColorHelper(rook, color);

        //        return rook;
        //    }

        //    public Bishop addBishop(int color, int xloc, int yloc)
        //    {
        //        Bishop bishop = new Bishop(chessBoard, color, xloc, yloc);
        //        pieceToColorHelper(bishop, color);

        //        return bishop;
        //    }

        //    public Pawn addPawn(int color, int xloc, int yloc)
        //    {
        //        Pawn pawn = new Pawn(chessBoard, color, xloc, yloc);
        //        pieceToColorHelper(pawn, color);

        //        return pawn;
        //    }

        //    private void pieceToColorHelper(Piece piece, int color)
        //    {
        //        if (color == BLACK)
        //            blackPieces.add(piece);
        //        else
        //            whitePieces.add(piece);
        //    }

    }
}
