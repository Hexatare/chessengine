using ChessEngineClassLibrary.Resources;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ChessEngineClassLibrary.Pieces
{
    /// <summary>
    /// Class Piece. Represents one Piece on the Board
    /// </summary>
    public abstract class Piece
    {
        #region Enumeration

        // Color of the Piece 
        public enum PColor
        {
            White = 0,
            Black = 1
        }

        // Code von Isi Enumeration of Pieces
        public enum PType
        {
            Pawn,
            Knight,
            Bishop,
            Rook,
            Queen,
            King
        }

        #endregion

        #region Properties and private Members

        // Image of the Piece 
        public Image Image { get; }

        // Piece Type
        public PType PieceType { get; set; }

        // Piece Color
        public PColor PieceColor { get; set; }

        // Piece has been moved from start position
        public bool HasMoved { get; set; }

        // Current Postion of the Piece on the Board
        public int[] Location { get; set; }

        // Reference to the Board
        private Board chessBoard;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="chessboard">Reference to the Board</param>
        /// <param name="pColor"></param>
        /// <param name="imgName"></param>
        public Piece(Board chessboard, PColor pColor, string imgName)
        {
            // Set the Reference to the Board
            this.chessBoard = chessboard;

            // Set the piece color property
            PieceColor = pColor;

            // Initialize local information
            HasMoved = false;
            //Location = new int[] { -1, -1};

            // Get the assembly containing the resource
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Load Image for this Piece
            string fullResourceName = assembly.GetName().Name + "."
                + Resource1.ResourcePath + "." + imgName;
            Stream? stream = assembly.GetManifestResourceStream(fullResourceName);

            if (stream != null)
            {
                BitmapImage bImage = new BitmapImage();

                bImage.BeginInit();
                bImage.StreamSource = stream;
                bImage.CacheOption = BitmapCacheOption.OnLoad;
                bImage.EndInit();

                Image = new Image
                {
                    Source = bImage,
                    Width = int.Parse(Resource1.PieceWidth),
                    Height = int.Parse(Resource1.PieceHeight)
                };

                stream.Dispose();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets an new Position of the Piece on the Board
        /// </summary>
        /// <param name="cellIndex">Index of the Cell on the Board</param>
        public void SetNewPosition(int cellIndex)
        {
            // calculate the x and y coordinates
            Location = new int[] { (cellIndex % 8), (7 - (cellIndex / 8)) };
        }


        /// <summary>
        /// Sets an new Position of the Piece on the Board
        /// </summary>
        /// <param name="xIndex">x-Coordinate</param>
        /// <param name="yIndex">y-Coordinate</param>
        /// <returns></returns>
        public void SetNewPosition(int xIndex, int yIndex)
        {
            Location = new int[] { xIndex, yIndex };
        }

        #endregion

        #region Game Logic of the Piece and Helper Methods


        /// <summary>
        /// Checks if a piece can move to a certain spot. The spot must either be empty or be occupied by
        /// and enemy chess piece. Finally, for specific pieces, the move must be a valid
        /// style of movement (depending on the chess piece). This method should be overwritten.Only
        /// should be used in this form for the generic chess piece.
        /// </summary>
        /// <param name="destCell">Cell to move the Piece to</param>
        /// <returns>TRUE if possible, otherwise FALSE</returns>
        public abstract bool CanMoveToDest(Cell destCell);


        /// <summary>
        /// Helper function that checks whether it is possible,
        /// for a chess piece to move to a spot in a very generic way
        /// </summary>
        /// <param name="destCell"></param>
        /// <returns>true if possible</returns>
        public bool CanMoveToDestGeneric(Cell destCell)
        {
            Piece location = destCell.GetPiece();

            if (location == null)
                return true;

            if (location.PieceColor != this.PieceColor)
                return true;

            return false;
        }


        /// <summary>
        /// Helper Class the determin, if a Piece can move in a straight line
        /// <param name="destCell"></param>
        /// <returns>true if possible</returns>
        public bool CanMoveStraight(Cell destCell)
        {
            int[] destIndex = destCell.Location;
            int[] sourceIndex = Location;

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
                    if (!chessBoard.GetCell(currX, startIndex).IsEmpty)
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
                    if (!chessBoard.GetCell(startIndex, currY).IsEmpty)
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
        public bool CanMoveDiagonal(Cell destCell)
        {
            int rowInc, columInc;

            // Turn the start and end cell indexes into row and column numbers
            int bishopStartRow = Location[1];
            int bishopStartCol = Location[0];
            int bishopEndRow = destCell.Location[1];
            int bishopEndCol = destCell.Location[0];

            // Get the absolute difference between the start and end row and column numbers
            int bishopRowDiff = Math.Abs(bishopEndRow - bishopStartRow);
            int bishopColDiff = Math.Abs(bishopEndCol - bishopStartCol);

            // Use the if statement to check if the Bishop can move to the cell
            if (bishopRowDiff == bishopColDiff)
            {
                // Set the cell increments for row and colums
                columInc = (bishopEndCol > bishopStartCol) ? 1 : -1;
                rowInc = (bishopEndRow > bishopStartRow) ? 1 : -1;

                // Loop trough all Cells, adjust the start postion
                int currColum = bishopStartCol + columInc;
                int currRow = bishopStartRow + rowInc;

                for(int i1 = 0; i1 < (bishopColDiff - 1); i1++, currColum += columInc, currRow += rowInc)
                {
                    if (!chessBoard.GetCell(currColum, currRow).IsEmpty)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }


        #endregion

    }
}