using ChessEngineClassLibrary.Resources;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using static ChessEngineClassLibrary.Piece;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Windows.Controls.Image;

namespace ChessEngineClassLibrary
{
    /// <summary>
    /// Class Piece. Represents one Piece on the Board
    /// </summary>
    public class Piece
    {
        // Color of the Piece 
        public enum PColor
        {
            White,
            Black
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

        //// Create constant ints to define the piece
        //public const int Empty = 0;
        //public const int King = 1;
        //public const int Pawn = 2;
        //public const int Knight = 3;
        //public const int Bishop = 4;
        //public const int Rook = 5;
        //public const int Queen = 6;

        //// Create two more constant ints to define the color of the piece
        //public const int White = 8;
        //public const int Black = 16;


        // Property to hold the int of the piece
        //public int PieceInt { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pType"></param>
        /// <param name="imgName"></param>
        public Piece(PColor pieceColor, string imgName)
        {
            // Set the piece color property
            PieceColor = pieceColor;

            // Get the assembly containing the resource
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Load whitePawn
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

    class Pawn : Piece
    {
        public Pawn(PColor pColor, string pName) : base(pColor, pName)
        {
            PieceType = PType.Pawn;
        }

    }

    class Knight : Piece
    {
        public Knight(PColor pColor, string pName) : base(pColor, pName)
        {
            PieceType = PType.Knight;
        }
    }

    class Bishop : Piece
    {
        public Bishop(PColor pColor, string pName) : base(pColor, pName)
        {
            PieceType = PType.Bishop;
        }
    }

    class Rook : Piece
    {
        public Rook(PColor pColor, string pName) : base(pColor, pName)
        {
            PieceType = PType.Rook;
        }
    }

    class Queen : Piece
    {
        public Queen(PColor pColor, string pName) : base(pColor, pName)
        {
            PieceType = PType.Queen;
        }
    }

    class King : Piece
    {
        public King(PColor pColor, string pName) : base(pColor, pName)
        {
            PieceType = PType.King;
        }
    }

}