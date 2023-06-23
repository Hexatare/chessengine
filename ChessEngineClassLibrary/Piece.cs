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
        private Dictionary<int, string> imagePaths = new Dictionary<int, string>
        {
            { White | King, "whiteKing.png" },
            { White | Queen, "whiteQueen.png" },
            { White | Rook, "whiteRook.png" },
            { White | Bishop, "whiteBishop.png" },
            { White | Knight, "whiteKnight.png" },
            { White | Pawn, "whitePawn.png" },
            { Black | King, "blackKing.png" },
            { Black | Queen, "blackQueen.png" },
            { Black | Rook, "blackRook.png" },
            { Black | Bishop, "blackBishop.png" },
            { Black | Knight, "blackKnight.png" },
            { Black | Pawn, "blackPawn.png" }
        };

        // Property to hold the int of the piece
        public int PieceInt { get; set; }

        // Make Image public so that it can be accessed by the Image_MouseMove method
        public Image pieceImage;

        public Piece(int pieceInt)
        {
            PieceInt = pieceInt;
        }

        // Function to load the image
        public void LoadImage(int pieceAsInt, int cellIndex, Grid chessGrid)
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

        }

        // Method to handle the Drag of the Image
        public void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DataObject dataObject = new DataObject(DataFormats.Bitmap, pieceImage.Source);
                DragDrop.DoDragDrop(pieceImage, dataObject, DragDropEffects.Move);
            }
        }
    }
}
