using ChessEngine.Resources;
using ChessEngineClassLibrary;
using ChessEngineClassLibrary.Models;
using ChessEngineClassLibrary.Pieces;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ChessEngine.Models
{
    /// <summary>
    /// Implementation of the ViewModel for the Main View
    /// </summary>
    public class MainWindowViewModel
    {

        #region Properties an Members

        /// <summary>
        /// Dictionary with all Pictures of all Pieces
        /// </summary>
        private readonly Dictionary<string, BitmapImage> pieceImages;

        // Number of Cells
        private const int NbrOfCells = 64;

        // Create an array of type Cell and call it CellsOnBoard
        private readonly CellViewModel[] CellsOnBoard;

        /// <summary>
        /// Reference to the board
        /// </summary>
        private Board? board;

        /// <summary>
        /// Reference to the Game
        /// </summary>
        private Game? game;

        /// <summary>
        /// The Grid of the Main Window
        /// </summary>
        private Grid? grid;

        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MainWindowViewModel()
        {
            // Create the dictionary
            pieceImages = new Dictionary<string, BitmapImage>();

            // Create a new array with the length of 64
            CellsOnBoard = new CellViewModel[NbrOfCells];

            try
            {
                // Load the black images from Ressources

                // Black Pawn
                LoadImage("blackPawn.png", Piece.PType.Pawn.ToString() + Piece.PColor.Black.ToString());

                // Black Knight
                LoadImage("blackKnight.png", Piece.PType.Knight.ToString() + Piece.PColor.Black.ToString());

                //Black Bishop
                LoadImage("blackBishop.png", Piece.PType.Bishop.ToString() + Piece.PColor.Black.ToString());

                //Black Rook
                LoadImage("blackRook.png", Piece.PType.Rook.ToString() + Piece.PColor.Black.ToString());

                //Black Qeen
                LoadImage("blackQueen.png", Piece.PType.Queen.ToString() + Piece.PColor.Black.ToString());

                //Black King
                LoadImage("blackKing.png", Piece.PType.King.ToString() + Piece.PColor.Black.ToString());


                // Load the white images from Ressources

                // White Pawn
                LoadImage("whitePawn.png", Piece.PType.Pawn.ToString() + Piece.PColor.White.ToString());

                // White Knight
                LoadImage("whiteKnight.png", Piece.PType.Knight.ToString() + Piece.PColor.White.ToString());

                // White Bishop
                LoadImage("whiteBishop.png", Piece.PType.Bishop.ToString() + Piece.PColor.White.ToString());

                // White Rook
                LoadImage("whiteRook.png", Piece.PType.Rook.ToString() + Piece.PColor.White.ToString());

                // White Qeen
                LoadImage("whiteQueen.png", Piece.PType.Queen.ToString() + Piece.PColor.White.ToString());

                // White King
                LoadImage("whiteKing.png", Piece.PType.King.ToString() + Piece.PColor.White.ToString());

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Method to Create all View Cells and Register the Eventhandler of the Board for Updates on the View
        /// </summary>
        /// <param name="mainGrid">Referenz to the grid of the View</param>
        /// <param name="chessBoard">Referenz to the Board</param>
        public void CreateCells(Grid mainGrid, Board chessBoard, Game game)
        {
            // Reference to the Board
            board = chessBoard;

            // Referenz to the Main Grid
            grid = mainGrid;

            // Referenz to the Game
            this.game = game;

            // Create alls Cells
            for (int i = 0; i < NbrOfCells; i++)
            {
                int cellIndex = i + 1;

                // Check if the square is light or dark
                bool isLightSquare = (i / 8 + i % 8) % 2 == 0;

                // Set the color
                CellsOnBoard[i] = new CellViewModel(isLightSquare ? CellColor.White : CellColor.Black, i);

                // Add Cell to the grid
                mainGrid.Children.Add(CellsOnBoard[i].grid);
                Grid.SetRow(CellsOnBoard[i].grid, i / 8);
                Grid.SetColumn(CellsOnBoard[i].grid, i % 8);

                // Register the Cell selected Eventhandler
                CellsOnBoard[i].CellSelected += (sender, e) => 
                {
                    // Convert sender to Cell
                    if(sender != null) 
                    {
                        CellViewModel? cell = (CellViewModel)sender; 
                        game.UserCellSelection(cell.Index); 
                    }
                };
            }

            // Register the Eventhandler
            board.UpdateViewEvent += Board_UpdateViewEvent; 

            
        }


        /// <summary>
        /// EventHandler for BordView Update - the ChessEngine (Board) sends this Command
        /// The Update is registered to the UI Thread for Synchronization Reasons
        /// </summary>
        /// <param name="sender">Board Model</param>
        /// <param name="e">EventParamter</param>
        //private void Board_UpdateViewEvent(object? sender, EventArgs e)
        private void Board_UpdateViewEvent(object? sender, EventArgs e)
        {
            if(grid != null)
                grid.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() => { UpdateView(); }));

        }


        /// <summary>
        /// This Method updates the View of the UI
        /// </summary>
        private void UpdateView()
        {
            for (int i1 = 0; i1 < NbrOfCells; i1++)
            {
                // Get Piece and State of each Cell on the Boardmodell
                Cell? cell = board.GetCell(i1);

                // Remove old images from the Cell
                CellsOnBoard[i1].RemoveImge();

                if (!cell.IsEmpty)
                {
                    Piece? piece = cell.GetPiece();

                    // Set the corresponding Image on the ViewCell, first remove an possible old Image
                    if (piece != null)
                    {
                        string key = piece.PieceType.ToString() + piece.PieceColor.ToString();
                        CellsOnBoard[i1].SetImage(this.GetImage(key));
                    }
                }
                // Set the Cell Color
                CellsOnBoard[i1].SetBorderColor(cell.CurrCellBorderColor);
            }
        }


        /// <summary>
        /// Returns a corresponding Image from the Dictionary
        /// </summary>
        /// <param name="key">Key to identify the Image</param>
        /// <returns>the requested Image</returns>
        private System.Windows.Controls.Image GetImage(string key)
        {
            System.Windows.Controls.Image img = new()
            {
                Source = pieceImages[key],
                Width = int.Parse(Resource1.PieceWidth),
                Height = int.Parse(Resource1.PieceHeight)
            };

            return img;
        }


        /// </summary>
        /// <param name="filename">Name of the File</param>
        /// <param name="key">Key for the Dict</param>
        private void LoadImage(string filename, string key)
        {
            Uri uri = new Uri($"pack://application:,,,/Resources/Pictures/{filename}");
            BitmapImage bImage = new BitmapImage(uri);

            if (bImage != null)
                pieceImages.Add(key, bImage);
        }
        
        #endregion


    }
}
