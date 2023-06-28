using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows;
using System.Reflection;
using System.Windows.Input;

namespace ChessEngineClassLibrary
{
    public class Piece
    {
        public string? PieceColor { get; set; }

        // Make Image public so that it can be accessed by the Image_MouseMove method
        public Image? PieceImage { get; set; }

        public int PieceCellIndex { get; set; }

        // Constructor
        public Piece(string pieceColor, int cellIndex)
        {
            // Color of the piece
            PieceColor = pieceColor;

            // The index of the cell that the piece is currently on
            PieceCellIndex = cellIndex;
        }

        // Method to get the image path
        public string? GetImagePath(string pieceType, string pieceColor)
        {
            // Create a dictionary to hold the image paths
            Dictionary<string, string> imagePaths = new Dictionary<string, string>
            {
                { "white" + "king", "whiteKing.png" },
                { "white" + "queen", "whiteQueen.png" },
                { "white" + "rook", "whiteRook.png" },
                { "white" + "bishop", "whiteBishop.png" },
                { "white" + "knight", "whiteKnight.png" },
                { "white" + "pawn", "whitePawn.png" },
                { "black" + "king", "blackKing.png" },
                { "black" + "queen", "blackQueen.png" },
                { "black" + "rook", "blackRook.png" },
                { "black" +  "bishop", "blackBishop.png" },
                { "black" + "knight", "blackKnight.png" },
                { "black" + "pawn", "blackPawn.png" }
            };

            // Get Path from dictionary
            imagePaths.TryGetValue(pieceColor + pieceType, out string? imagePath);

            // Return the image Path
            return imagePath;
        }   

        // Method to handle the Drag of the Image
        public void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DataObject dataObject = new DataObject(DataFormats.Bitmap, PieceImage.Source);
                DragDrop.DoDragDrop(PieceImage, dataObject, DragDropEffects.Move);
            }
        }
    }

    public class King : Piece
    {
        public King(string pieceColor, int cellIndex) : base(pieceColor, cellIndex)
        {
            // Get Path from dictionary
            string? imagePath = GetImagePath("king", PieceColor);

            if (imagePath == null)
            {
                throw new ArgumentNullException("Image Path is null");
            }   

            // Create Bitmap Image
            BitmapImage bitmap = new BitmapImage(new Uri($"C:\\Users\\Noël Mani\\Documents\\chessengine-main\\ChessEngine\\Resources\\Pictures\\{imagePath}"));

            // Create new image for the Piece
            PieceImage = new Image
            {
                Source = bitmap,
                Width = 100,
                Height = 100
            };

            // Add event handler to the image
            PieceImage.MouseMove += Image_MouseMove;
        }
    }

    public class Queen : Piece
    {
        public Queen(string pieceColor, int cellIndex) : base(pieceColor, cellIndex)
        {
            // Get Path from dictionary
            string? imagePath = GetImagePath("queen", PieceColor);

            if (imagePath == null)
            {
                throw new ArgumentNullException("Image Path is null");
            }

            // Create Bitmap Image
            BitmapImage bitmap = new BitmapImage(new Uri($"C:\\Users\\Noël Mani\\Documents\\chessengine-main\\ChessEngine\\Resources\\Pictures\\{imagePath}"));

            // Create new image for the Piece
            PieceImage = new Image
            {
                Source = bitmap,
                Width = 100,
                Height = 100
            };

            // Add event handler to the image
            PieceImage.MouseMove += Image_MouseMove;
        }
    }

    public class Rook : Piece
    {
        public Rook(string pieceColor, int cellIndex) : base(pieceColor, cellIndex)
        {
            // Get Path from dictionary
            string? imagePath = GetImagePath("rook", PieceColor);
            
            if (imagePath == null)
            {
                throw new ArgumentNullException("Image Path is null");
            }

            // Create Bitmap Image
            BitmapImage bitmap = new BitmapImage(new Uri($"C:\\Users\\Noël Mani\\Documents\\chessengine-main\\ChessEngine\\Resources\\Pictures\\{imagePath}"));

            // Create new image for the Piece
            PieceImage = new Image
            {
                Source = bitmap,
                Width = 100,
                Height = 100
            };

            // Add event handler to the image
            PieceImage.MouseMove += Image_MouseMove;
        }
    }

    public class Bishop : Piece
    {
        public Bishop(string pieceColor, int cellIndex) : base(pieceColor, cellIndex)
        {
            // Get Path from dictionary
            string? imagePath = GetImagePath("bishop", PieceColor);

            if (imagePath == null)
            {
                throw new ArgumentNullException("Image Path is null");
            }

            // Create Bitmap Image
            BitmapImage bitmap = new BitmapImage(new Uri($"C:\\Users\\Noël Mani\\Documents\\chessengine-main\\ChessEngine\\Resources\\Pictures\\{imagePath}"));

            // Create new image for the Piece
            PieceImage = new Image
            {
                Source = bitmap,
                Width = 100,
                Height = 100
            };

            // Add event handler to the image
            PieceImage.MouseMove += Image_MouseMove;
        }
    }

    public class Knight : Piece
    {
        public Knight(string pieceColor, int cellIndex) : base(pieceColor, cellIndex)
        {
            // Get Path from dictionary
            string? imagePath = GetImagePath("knight", PieceColor);

            if (imagePath == null)
            {
                throw new ArgumentNullException("Image Path is null");
            }

            // Create Bitmap Image
            BitmapImage bitmap = new BitmapImage(new Uri($"C:\\Users\\Noël Mani\\Documents\\chessengine-main\\ChessEngine\\Resources\\Pictures\\{imagePath}"));

            // Create new image for the Piece
            PieceImage = new Image
            {
                Source = bitmap,
                Width = 100,
                Height = 100
            };

            // Add event handler to the image
            PieceImage.MouseMove += Image_MouseMove;
        }
    }

    public class Pawn : Piece
    {
        public Pawn(string pieceColor, int cellIndex) : base(pieceColor, cellIndex)
        {
            // Get Path from dictionary
            string? imagePath = GetImagePath("pawn", PieceColor);

            if (imagePath == null)
            {
                throw new ArgumentNullException("Image Path is null");
            }

            // Create Bitmap Image
            BitmapImage bitmap = new BitmapImage(new Uri($"C:\\Users\\Noël Mani\\Documents\\chessengine-main\\ChessEngine\\Resources\\Pictures\\{imagePath}"));

            // Create new image for the Piece
            PieceImage = new Image
            {
                Source = bitmap,
                Width = 100,
                Height = 100
            };

            // Add event handler to the image
            PieceImage.MouseMove += Image_MouseMove;
        }
    }

    public class EmptyField : Piece
    {
        public EmptyField(int cellIndex) : base(null, cellIndex)
        {
        }
    }
}
