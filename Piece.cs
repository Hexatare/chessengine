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
        public Image Image { get; set; }

        // Piece Type
        public PType PieceType { get; set; }

        // Piece Color
        public PColor PieceColor { get; set; }

        // Create constant ints to define the piece
        public const int Empty = 0;
        public const int King = 1;
        public const int Pawn = 2;
        public const int Knight = 3;
        public const int Bishop = 4;
        public const int Rook = 5;
        public const int Queen = 6;

        // Create two more constant ints to define the color of the piece
        public const int White = 8;
        public const int Black = 16;

        // Create a dictionary to hold the image paths
        //private Dictionary<PColor, string> imagePaths = new Dictionary<PColor, string>
        //{
        //    { PColor.White, "whiteKing.png" },
        //    { White | Queen, "whiteQueen.png" },
        //    { White | Rook, "whiteRook.png" },
        //    { White | Bishop, "whiteBishop.png" },
        //    { White | Knight, "whiteKnight.png" },
        //    { White | Pawn, "whitePawn.png" },
        //    { Black | King, "blackKing.png" },
        //    { Black | Queen, "blackQueen.png" },
        //    { Black | Rook, "blackRook.png" },
        //    { Black | Bishop, "blackBishop.png" },
        //    { Black | Knight, "blackKnight.png" },
        //    { Black | Pawn, "blackPawn.png" }
        //};

        // Property to hold the int of the piece
        public int PieceInt { get; set; }

        // Make Image public so that it can be accessed by the Image_MouseMove method
        // public Image pieceImage;

        public Piece()
        { }

        public Piece(int pieceInt)
        {
            PieceInt = pieceInt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pType"></param>
        /// <param name="pName"></param>
        public Piece(PColor pieceColor, string pName)
        {
            // Set the piece color property
            PieceColor = pieceColor;

            // Get the assembly containing the resource
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Load whitePawn
            string fullResourceName = assembly.GetName().Name + "."
                + Resource1.ResourcePath + "." + pName;
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

            //Uri uri = new Uri($"pack://application:,,,/YourAssemblyName;component/Resources/images/photo.png", UriKind.Absolute);
            //BitmapImage bitmap = new BitmapImage(uri);

        }

        // Function to load the image
        /*public void LoadImage(int pieceAsInt, int cellIndex, Grid chessGrid)
        {
            // Get Path from dictionary
            imagePaths.TryGetValue(pieceAsInt, out string imagePath);

            // Create Bitmap Image
            BitmapImage bitmap = new BitmapImage(new Uri($"C:\\Users\\noelm\\Documents\\Visual Studio\\ChessEngine\\Resources\\Pictures\\{imagePath}"));
            

            pieceImage = new Image
            {
                Source = bitmap,
                Width = 100,
                Height = 100
            };
            pieceImage.MouseMove += Image_MouseMove;
            pieceImage.DataContext = this;

            // Convert single index into two
            int rowIndex = cellIndex / 8;
            int columnIndex = cellIndex % 8;

            // Select Grid using the two indices, got this from ChatGPT
            Grid selectedGrid = chessGrid.Children
                .OfType<Grid>()
                .First(grid =>
                    Grid.GetRow(grid) == rowIndex &&
                    Grid.GetColumn(grid) == columnIndex);

            // Check if the grid exists
            if (selectedGrid != null)
            {
                // For testing purposes only, change later
                selectedGrid.Children.Add(pieceImage);
            }
            else
            {
                throw new ArgumentNullException();
            }

        }*/

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

    public class Pawn : Piece
    {
        public Pawn(PColor pColor, string pName) : base(pColor, pName)
        {
            PieceType = PType.Pawn;
        }
    }

    public class Knight : Piece
    {
        public Knight(PColor pColor, string pName) : base(pColor, pName)
        {
            PieceType = PType.Knight;
        }
    }

    public class Bishop : Piece
    {
        public Bishop(PColor pColor, string pName) : base(pColor, pName)
        {
            PieceType = PType.Bishop;
        }
    }

    public class Rook : Piece
    {
        public Rook(PColor pColor, string pName) : base(pColor, pName)
        {
            PieceType = PType.Rook;
        }
    }

    public class Queen : Piece
    {
        public Queen(PColor pColor, string pName) : base(pColor, pName)
        {
            PieceType = PType.Queen;
        }
    }

    public class King : Piece
    {
        public King(PColor pColor, string pName) : base(pColor, pName)
        {
            PieceType = PType.King;
        }
    }

}