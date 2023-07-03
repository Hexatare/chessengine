using ChessEngineClassLibrary.Resources;
using System.IO;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

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


        /// <summary>
        /// Constructor
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