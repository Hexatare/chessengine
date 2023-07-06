using ChessEngineClassLibrary.Resources;
using System.IO;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System;
using System.Windows;

namespace ChessEngineClassLibrary.Pieces
{
    /// <summary>
    /// Class Piece. Represents one Piece on the Board
    /// </summary>
    public abstract class Piece
    {
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


        // Image of the Piece 
        public Image Image { get; }

        // Piece Type
        public PType PieceType { get; set; }

        // Piece Color
        public PColor PieceColor { get; set; }

        // Piece has been moved from start position
        public bool HasMoved { get; set; }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pType"></param>
        /// <param name="imgName"></param>
        public Piece(PColor pieceColor, string imgName)
        {
            // Set the piece color property
            PieceColor = pieceColor;

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


        ///// <summary>
        ///// Sets an new Position of the Piece on the Board
        ///// </summary>
        ///// <param name="cellIndex">Index of the Cell on the Board</param>
        //public void SetNewPosition(int cellIndex)
        //{
        //    // calculate the x and y coordinates
        //    Location = new int[] { (CellIndex % 8), (7 - (CellIndex / 8)) };

        //    CellIndex = cellIndex;
        //}



        ///// <summary>
        ///// Sets an new Position of the Piece on the Board
        ///// </summary>
        ///// <param name="xIndex">x-Coordinate</param>
        ///// <param name="yIndex">y-Coordinate</param>
        ///// <returns></returns>
        //public void SetNewPosition(int xIndex, int yIndex)
        //{
        //    // Index on the chess board, from 0 (a8) to 63 (h7)
        //    CellIndex = xIndex + ((yIndex % 7) * 8);

        //    Location = new int[] { xIndex, yIndex };
        //}


        // Method to handle the Drag of the Image
        public void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //DataObject dataObject = new DataObject(DataFormats.Bitmap, pieceImage.Source);
                //DragDrop.DoDragDrop(pieceImage, dataObject, DragDropEffects.Move);
            }
        }
    }
}