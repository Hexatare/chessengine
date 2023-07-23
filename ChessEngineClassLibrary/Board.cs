using ChessEngineClassLibrary.Models;
using ChessEngineClassLibrary.Pieces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ChessEngineClassLibrary
{
    /// <summary>
    /// Class Board implements the Chessboard with its 64 Cells.
    /// </summary>
    public class Board
    {
        #region Properties and Members

        /// <summary>
        /// Constant for En Passant Row where Black is possible
        /// </summary>
        private const int ENP_ROW_BLACK = 4;

        /// <summary>
        /// Constant for En Passant Row where White is possible
        /// </summary>
        private const int ENPL_ROW_WHITE = 5;

        // Number of Cells
        private const int NbrOfCells = 64;

        // Create an array of type Cell and call it CellsOnBoard
        private readonly Cell[] CellsOnBoard;

        /// <summary>
        /// List of all moves done on the board
        /// </summary>
        private readonly List<Move> allMoves;

        /// <summary>
        /// Eventhandler for updating the view
        /// </summary>
        public event EventHandler? UpdateViewEvent;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor. The constructor is what the backend uses as the chessboard. It calls the methods CreateCells and CreatePieces
        /// </summary>
        /// <param name="chessGrid">Reference to the Main Application</param>
        //public Board(Grid chessGrid)
        public Board()
        {
            // Create a new array with the length of 64
            CellsOnBoard = new Cell[NbrOfCells];

            // Create alls Cells
            for (int i = 0; i < NbrOfCells; i++)
            {
                int cellIndex = i + 1;

                // Check if the square is light or dark
                bool isLightSquare = (i / 8 + i % 8) % 2 == 0;

                // Set the color
                CellsOnBoard[i] = new Cell(this, isLightSquare ? CellColor.White : CellColor.Black, i);
            }

            // List of all moves
            allMoves = new List<Move>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Place a Piece on the Board with x and y Coordinate
        /// </summary>
        /// <param name="piece">Piece to be placed</param>
        /// <param name="xIndex">X Coordinate on the Board</param>
        /// <param name="yIndex">Y Coordinate on the Board</param>
        public void PlacePlieceOnBoard(Piece piece, int xIndex, int yIndex)
        {
            // Index, starting from 0 (a1) to 63 (h8), in a regular Chessboard
            int cellIndex = xIndex + ((yIndex % 7) * 8);

            // Set a piece on the board
            CellsOnBoard[cellIndex].SetPiece(piece);
        }


        /// <summary>
        /// Place a Piece on the Board with Board Array [0..63]
        /// </summary>
        /// <param name="piece">Piece to be placed</param>
        /// <param name="arrayIndex">Array Index of the Board [0..63]</param>
        public void PlacePlieceOnBoard(Piece piece, int arrayIndex)
        {
            // Set a piece on the board
            CellsOnBoard[arrayIndex].SetPiece(piece);
        }


        /// <summary>
        /// Removes all Pieces on the board
        /// </summary>
        public void RemoveAllPieces()
        {
            foreach (Cell cell in CellsOnBoard)
            {
                if (!cell.IsEmpty)
                    cell.RemovePiece();

                cell.CurrCellBorderColor = Cell.CellBorderColor.None;
            }
        }


        /// <summary>
        /// Returns the array index of a Cell on the board
        /// </summary>
        /// <param name="xIndex"></param>
        /// <param name="yIndex"></param>
        /// <returns></returns>
        public int GetArrayIndex(int xIndex, int yIndex)
        {
            // Index on the chess board, from 0 (a8) to 63 (h7)
            int index = xIndex + ((7 - (yIndex % 8)) * 8);
            Debug.Assert(index >= 0 || index <= 63);
            return xIndex + ((7 - (yIndex % 8) ) * 8);
        }


        /// <summary>
        /// Returns the coordinate (x and y) on the board
        /// </summary>
        /// <param name="arrayIndex"></param>
        /// <returns>array with [x,y] values</returns>
        public int[] GetBoardIndex(int arrayIndex)
        { 
            return new int[] { (arrayIndex % 8), (7 - (arrayIndex / 8)) };
        }


        /// <summary>
        /// Returns the Cell on Location [x,y]
        /// </summary>
        /// <param name="xIndex">x Coordinate</param>
        /// <param name="yIndex">y Coordinate</param>
        /// <returns>the requested Cell</returns>
        public Cell GetCell(int xIndex, int yIndex) 
        {
            return CellsOnBoard[GetArrayIndex(xIndex, yIndex)];
        }


        /// <summary>
        /// Returns the Cell at Position index
        /// </summary>
        /// <param name="arrayIndex">Index of the >Cell</param>
        /// <returns>the requested Cell</returns>
        public Cell GetCell(int arrayIndex)
        {
            return CellsOnBoard[arrayIndex];
        }


        /// <summary>
        /// Returns the Cell at Position index
        /// </summary>
        /// <param name="position">Coordinate [0] = x, [1] = y</param>
        /// <returns>the requested Cell</returns>
        public Cell GetCell(int[] position)
        {
            return CellsOnBoard[GetArrayIndex(position[0], position[1])];
        }


        /// <summary>
        /// Returns a List with all Cells of the Board
        /// </summary>
        /// <returns>List with all Cells of the Board</returns>
        public List<Cell> GetCells()
        {
            List<Cell> cells = new();
            foreach (Cell cell in CellsOnBoard)
                cells.Add(cell);

            return cells;
        }


        /// <summary>
        /// Moves a piece on the board, according to the Move parameter.
        /// If the move captures a piece, the piece will be removed from the board
        /// </summary>
        /// <param name="move">The move to be done</param>
        public void DoMove(Move moveToDo)
        {
            if(moveToDo != null)
            { 
                // Check that destCell not NULL
                if (moveToDo.Start == null || moveToDo.End == null)
                    return;

                // if target Cell was not empty, a kill was performed
                if (!moveToDo.End.IsEmpty)  
                {
                    Piece? killedPiece = moveToDo.End.GetPiece();

                    if(killedPiece != null)
                    {
                        // Remove the capuret Piec, from Cell and List
                        moveToDo.End.RemovePiece();
                        moveToDo.PieceKilled = killedPiece;
                    }

                }
                // Move the Piece from Source to Destination
                Piece? piece = moveToDo.Start.GetPiece();

                if(piece != null) 
                {
                    moveToDo.Start.RemovePiece();
                    moveToDo.End.SetPiece(piece);
                }
  
                // Add the move to the List
                allMoves.Add(moveToDo);
            }

        }


        /// <summary>
        /// Undo a move on the Board, according to the Move parameter, e.q. a invers Move.
        /// If the move to be undone involves a capture of a piece, the captured piece will be restored on the board.
        /// </summary>
        /// <param name="move">The move to be undone</param>
        public void UndoMove(Move moveToUndo)
        {
            if (moveToUndo != null)
            {
                // Check that destCell not NULL
                if (moveToUndo.Start == null || moveToUndo.End == null)
                    return;

                // Move the Pices from the End back to the start 
                Piece? piece = moveToUndo.End.GetPiece();

                if (piece != null)
                {
                    moveToUndo.End.RemovePiece();
                    moveToUndo.Start.SetPiece(piece);
                }

                // if the move contained a killed Piece, restore the Piece
                // in case of En Passant, get the killed Pawn location and restore there
                if(moveToUndo.PieceKilled !=  null)
                {
                    int[] pLoc = moveToUndo.PieceKilled.Location;
                    this.GetCell(pLoc).SetPiece(moveToUndo.PieceKilled);
                }

                 // Remove from List
                 allMoves.Remove(moveToUndo);
            }
        }


        /// <summary>
        /// Returns a List of all Pieces of a given Color from the Board
        /// </summary>
        /// <param name="color">Color of the pieces to receive</param>
        /// <returns>List with all Pieces of the requested color</returns>
        public List<Piece> GetPieces(Piece.PColor color)
        {
            List<Piece> pieces = new();

            foreach(Cell cell in CellsOnBoard)
            {
                if (!cell.IsEmpty)
                {
                    Piece? piece = cell.GetPiece();

                    if (piece != null && piece.PieceColor == color)
                    {
                        pieces.Add(piece);
                    }
                }
            }
            return pieces;
        }


        /// <summary>
        /// Methode that calls the View to update is, this after Pieces are moved
        /// </summary>
        public virtual void OnUpdateView()
        {
           // Raise Event to show the Dialog
           UpdateViewEvent?.Invoke(this, EventArgs.Empty);
        }
        

        /// <summary>
        /// Returns the last move that was performed on the Board. The move ist remains in the list
        /// </summary>
        /// <returns>Last move performed</returns>
        public Move? GetLastMove()
        {
            if (allMoves.Count == 0)
                return null;
            return allMoves[allMoves.Count - 1];
        }

        #endregion

        #region Boardlogic

        /// <summary>
        /// Check, if the King is in Check Position
        /// </summary>
        /// <param name="color">Color of the King be checked, if InCheck</param>
        /// <param name="cell">Cell, on which the King would be InCheck</param>
        /// <returns>TRUE if in Check</returns>
        public bool IsKingInCheck(Piece.PColor color, Cell? cell = null)
        {
            bool result = false;

            List<Piece> originalList;
            King? kingInQuestion;

            if (color == Piece.PColor.Black)
            {
                originalList = GetPieces(Piece.PColor.White);
                kingInQuestion = (King?)GetPieces(Piece.PColor.Black).Find(obj => obj.PieceType == Piece.PType.King);
            }
            else
            {
                originalList = GetPieces(Piece.PColor.Black);
                kingInQuestion = (King?)GetPieces(Piece.PColor.White).Find(obj => obj.PieceType == Piece.PType.King);
            }

            if (kingInQuestion != null)
            {
                // Get the Location of the King
                int[]? kingLoc = (cell == null) ? kingInQuestion.Location : cell.Location;

                // Check for each Piece of the opponent Player, if it can move to the King's position
                foreach (Piece currentPiece in originalList)
                {
                    if (currentPiece.CanMoveToDest(GetCell(kingLoc)))
                    {
                        result = true;
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// Method to check a Games Postion, e.q. if the King is still or new in Check after a Move has been performed
        /// </summary>
        /// <param name="move">The move to be performed</param>
        /// <param name="color">The color of the king to be checked/param>
        /// <returns></returns>
        public bool IsCheckResolved(Move move, Piece.PColor color)
        {
            // Perform the desired move and check, if any Piece of the opponent can still go to the Kings Position
            DoMove(move);

            // If any piece can move to this spot, move here, the king is still in check, then go to next location.
            bool isResolved = !IsKingInCheck(color);

            // Undo the last move
            UndoMove(move);

            return isResolved;
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
        /// <param name="color">Color of player to check</param>
        /// <returns>True, if move possible</returns>
        public bool IsMovePossible(Piece.PColor color)
        {
            bool movePossible;
            List<Piece> checkPieces;

            if (color == Piece.PColor.Black)
                checkPieces = GetPieces(Piece.PColor.Black);
            else
                checkPieces = GetPieces(Piece.PColor.White);

            // Test for each Cell, if a Piece of the given Player can move to
            foreach (Cell cell in GetCells())
            {
                // If any piece can move to this spot, move here
                // If king is still in check, then go to next location.
                foreach (Piece piece in checkPieces)
                {
                    // Check if move possible
                    Move move = new(GetCell(piece.Location), cell, color);
                    movePossible = this.IsMovePossible(move, piece);

                    if (movePossible)
                        return true;
                }
            }
            return false;
        }


        /// <summary>
        /// The Method checks, if a piece can move to the End Cell
        /// </summary>
        /// <param name="move">The move to be performed</param>
        /// <param name="piece">Piece that should be moved</param>
        /// <returns></returns>
        public bool IsMovePossible(Move move, Piece piece)
        {
            bool movePossible = false;

            // Test if the Piece can move to the destination Cell
            if (piece.CanMoveToDest(move.End))
            {
                // Perform the move on the Board
                DoMove(move);

                // no InCheck, take Move back and return true
                movePossible = !this.IsKingInCheck(piece.PieceColor);

                // Perform the move on the Board
                UndoMove(move);
            }
            return movePossible;
        }


        /// <summary>
        /// Gets a List with all possible Cells, a Piece can move to
        /// </summary>
        /// <param name="move">The move to be done</param>
        /// <returns>The List with possible Cells</returns>
        public List<Cell> GetTargetMoveCells(Move move)
        {
            List<Cell> cells = new();

            if (move.Start == null || move.Start.IsEmpty)
                return cells;

            // Check for the source Cell, where it can possible move to
            Piece? piece = move.Start.GetPiece();

            if (piece != null)
            {
                foreach (Cell cell in GetCells())
                {
                    Move pMove = new Move(GetCell(piece.Location), cell, move.PColor);
                    if (IsMovePossible(pMove, piece))
                        cells.Add(cell);

                    // Check for Castling, and add the Cell to the List
                    if (piece.PieceType == Piece.PType.King && IsCastlingMove(pMove))
                        cells.Add(cell);

                    // Check for En Passant, and add the Cell to the List
                    if (piece.PieceType == Piece.PType.Pawn && IsEnPassantMove(pMove))
                        cells.Add(cell);
                }
            }
            return cells;
        }


        /// <summary>
        /// Get a list of all possible moves of the given Player
        /// </summary>
        /// <param name="pColor">The color of the player</param>
        /// <returns>A List with all possible moves</returns>
        public List<Move> GetAllPossibleMoves(Piece.PColor pColor)
        {
            List<Move> moves = new List<Move>();

            // Get the List of all Pieces
            foreach (Piece? piece in GetPieces(pColor))
            {
                // Get the cell of the Piece
                Cell? cell = GetCell(piece.Location);

                // Create a new Move and get all possible moves
                Move move = new Move(cell, cell, pColor);

                foreach (Cell? targetCell in GetTargetMoveCells(move))
                {
                    move.End = targetCell;
                    moves.Add(move);
                }
            }
            return moves;
        }


        /// <summary>
        /// Check, if the King can do a castling move
        /// </summary>
        /// <param name="move">The move that was perfomed</param>
        /// <returns>TRUE if in Check</returns>
        public bool IsCastlingMove(Move move)
        {
            List<Piece> originalList;
            King? kingInQuestion;
            Cell? rookCell;
            bool kingPassingThroughCheck;

            // Current Color
            Piece.PColor playerColor = move.PColor;

            if (playerColor == Piece.PColor.Black)
            {
                originalList = GetPieces(Piece.PColor.Black);
                kingInQuestion = (King?)originalList.Find(obj => obj.PieceType == Piece.PType.King);
            }
            else
            {
                originalList = GetPieces(Piece.PColor.White);
                kingInQuestion = (King?)originalList.Find(obj => obj.PieceType == Piece.PType.King);
            }

            // a vaild King 
            if (kingInQuestion != null)
            {
                // Check the four conditions of Castling - King has not moved and is not in Check
                if (kingInQuestion.HasMoved || IsKingInCheck(playerColor))
                    return false;

                // Check movement of the King - max 2 moves
                int kingMovement = kingInQuestion.Location[0] - move.End.Location[0];

                if (Math.Abs(kingMovement) != 2
                    || move.End.Location[1] != kingInQuestion.Location[1])
                    return false;

                // Castling Queenside - no pieces in between and Rook has not moved and King will not be in Check
                if (kingInQuestion.CanQueensideCastle && kingMovement > 0)
                {
                    rookCell = GetCell(0, kingInQuestion.Location[1]);
                    kingPassingThroughCheck = IsKingInCheck(playerColor, GetCell(move.End.Location))
                                              || IsKingInCheck(playerColor, GetCell(move.End.Location[0] - 1, move.End.Location[1]));
                }
                else if (kingInQuestion.CanKingsideCastle && kingMovement < 0)
                {
                    rookCell = GetCell(7, kingInQuestion.Location[1]);
                    kingPassingThroughCheck = IsKingInCheck(playerColor, GetCell(move.End.Location))
                                              || IsKingInCheck(playerColor, GetCell(move.End.Location[0] + 1, move.End.Location[1]));
                }
                else
                    return false;

                Piece? rook = rookCell.GetPiece();
                if (rook != null && !rook.HasMoved && kingInQuestion.CanMoveStraight(rookCell) && !kingPassingThroughCheck)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Check if a Promotion Move has been done
        /// </summary>
        /// <param name="move">The move, that was performed</param>
        /// <returns>TRUE, if Promotion is possible</returns>
        public bool IsPromotionMove(Move move)
        {
            int yEndCoord;

            if (move.PColor == Piece.PColor.Black)
                yEndCoord = 0;
            else
                yEndCoord = 7;

            // not the end positioin
            if (move.End.Location[1] != yEndCoord)
                return false;

            return true;
        }


        /// <summary>
        /// Check if a Promotion Move was performed
        /// </summary>
        /// <param name="move">The move</param>
        /// <returns></returns>
        public bool IsEnPassantMove(Move move)
        {
            Move? lastMove;
            bool previousMoveOk;
            int yTargetPos;

            //get last move
            lastMove = GetLastMove();

            // If no moves so far
            if (lastMove == null)
                return false;

            yTargetPos = lastMove.End.Location[1] + 1;
            previousMoveOk =   (lastMove.GetYMovement() == 2) 
                            && (lastMove.End.Location[1] == ENP_ROW_BLACK || lastMove.End.Location[1] == ENPL_ROW_WHITE);

            // if last Move was a double move and the piece is on the right colum
            if (previousMoveOk
                && (move.End.Location[0] == lastMove.End.Location[0])
                && (move.Start.Location[1] == lastMove.End.Location[1])
                && (move.End.Location[1] == yTargetPos))
                return true;

            return false;
        }
    }

    #endregion
}
