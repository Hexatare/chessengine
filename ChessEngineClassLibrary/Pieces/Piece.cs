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
            Empty,
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
        protected bool HasMoved { get; set; }

        // Location of the piece on the board, [0] = x, [1] = y; 
        protected int[] Location { get; set; }


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
            Location = new int[] { -1, -1};

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